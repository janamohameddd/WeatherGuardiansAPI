using System;

namespace WeatherGuardiansAPI.Services
{
	public interface IGaleService
	{
		GalePrediction GetGalePrediction(DateTime date);
	}

	public sealed class GaleService : IGaleService
	{
		public GalePrediction GetGalePrediction(DateTime date)
		{
			DateTime targetDate = date.Date;

			double baselineKmH = CalculateSeasonalBaselineKmH(targetDate);
			double variationKmH = GetDeterministicDailyVariationKmH(targetDate);
			double averageKmH = ClampAverageSpeed(baselineKmH + variationKmH);

			double gustKmH = GetGustKmH(targetDate, averageKmH);
			string recommendation = BuildRecommendation(averageKmH, gustKmH);

			return new GalePrediction(targetDate, averageKmH, gustKmH, recommendation);
		}

		private static double CalculateSeasonalBaselineKmH(DateTime date)
		{
			const double annualMeanKmH = 16.0;
			const double amplitudeKmH = 8.0;
			const int peakDayOfYear = 25;

			int year = date.Year;
			int dayOfYear = date.DayOfYear;
			int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
			double radians = 2.0 * Math.PI * (dayOfYear - peakDayOfYear) / daysInYear;
			return annualMeanKmH + amplitudeKmH * Math.Cos(radians);
		}

		private static double GetDeterministicDailyVariationKmH(DateTime date)
		{
			int seed = CreateDateSeed(date);
			var random = new Random(seed);
			double baseVariation = (random.NextDouble() * 2.0 - 1.0) * 6.0;
			double weekdayBias = (date.DayOfWeek is DayOfWeek.Tuesday or DayOfWeek.Wednesday or DayOfWeek.Thursday) ? 0.8 : 0.0;
			return baseVariation + weekdayBias;
		}

		private static double GetGustKmH(DateTime date, double averageKmH)
		{
			int seed = unchecked(CreateDateSeed(date) * 37 + 11);
			var random = new Random(seed);
			double gustFactor = 1.10 + random.NextDouble() * 0.25;
			bool shoulderSeason = date.Month is 3 or 4 or 10 or 11;
			if (shoulderSeason && random.NextDouble() < 0.35)
			{
				gustFactor += 0.2 + random.NextDouble() * 0.25;
			}

			double gust = averageKmH * gustFactor;
			double minGust = Math.Max(averageKmH + 4.0, averageKmH * 1.12);
			gust = Math.Max(gust, minGust);

			return ClampGustSpeed(gust);
		}

		private static string BuildRecommendation(double averageKmH, double gustKmH)
		{
			if (averageKmH < 5)
			{
				return "Calm, perfect for walking.";
			}
			if (averageKmH < 15)
			{
				return "Light breeze. Great day for outdoor activities.";
			}
			if (averageKmH < 30)
			{
				return gustKmH > 45 ? "Breezy with gusts—secure loose items." : "Moderate breeze. Enjoy, but be mindful of gusts.";
			}
			if (averageKmH < 50)
			{
				return "Strong winds. Limit outdoor activities and use caution.";
			}
			if (averageKmH < 70)
			{
				return "Very strong winds. Avoid exposed areas and secure belongings.";
			}
			return "Damaging winds possible. Avoid outdoor activities and follow local guidance.";
		}

		private static double ClampAverageSpeed(double kmh)
		{
			return Math.Max(0.0, Math.Min(80.0, kmh));
		}

		private static double ClampGustSpeed(double kmh)
		{
			return Math.Max(0.0, Math.Min(120.0, kmh));
		}

		private static int CreateDateSeed(DateTime date)
		{
			int seed = date.Year * 10000 + date.Month * 100 + date.Day;
			return seed;
		}
	}

public readonly record struct GalePrediction(
	DateTime Date,
	double AverageWindSpeedKmH,
	double GustKmH,
	string Recommendation
);
}


