using System;

namespace WeatherGuardiansAPI.Services
{
	public interface IBlazeService
	{
		BlazePrediction GetBlazePrediction(DateTime date);
	}

	public sealed class BlazeService : IBlazeService
	{
		public BlazePrediction GetBlazePrediction(DateTime date)
		{
			DateTime targetDate = date.Date;
			double seasonalBaselineC = CalculateSeasonalBaseline(targetDate);
			double variationC = GetDeterministicDailyVariation(targetDate);
			double anomalyC = GetSeasonalAnomaly(targetDate);

			double temperatureC = ClampTemperature(seasonalBaselineC + variationC + anomalyC);
			double temperatureF = CelsiusToFahrenheit(temperatureC);
			string recommendation = BuildRecommendation(temperatureC);

			return new BlazePrediction(targetDate, temperatureC, temperatureF, recommendation);
		}

		private static double CalculateSeasonalBaseline(DateTime date)
		{
			const double annualMeanC = 15.0;
			const double amplitudeC = 15.0;
			const int peakDayOfYear = 196;

			int year = date.Year;
			int dayOfYear = date.DayOfYear;

			int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
			double radians = 2.0 * Math.PI * (dayOfYear - peakDayOfYear) / daysInYear;

			return annualMeanC + amplitudeC * Math.Cos(radians);
		}

		private static double GetDeterministicDailyVariation(DateTime date)
		{
			int seed = CreateDateSeed(date);
			var random = new Random(seed);
			double baseVariation = (random.NextDouble() * 2.0 - 1.0) * 4.0;
			double weekendBias = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) ? 0.5 : 0.0;
			return baseVariation + weekendBias;
		}

		private static double GetSeasonalAnomaly(DateTime date)
		{
			int seed = unchecked(CreateDateSeed(date) * 31 + 7);
			var random = new Random(seed);
			int month = date.Month;

			if (month is 6 or 7 or 8)
			{
				double roll = random.NextDouble();
				if (roll < 0.15)
				{
					return 2.0 + random.NextDouble() * 4.0;
				}
			}

			if (month is 12 or 1 or 2)
			{
				double roll = random.NextDouble();
				if (roll < 0.12)
				{
					return -(2.0 + random.NextDouble() * 4.0);
				}
			}

			return 0.0;
		}

		private static double ClampTemperature(double temperatureC)
		{
			return Math.Max(-25.0, Math.Min(45.0, temperatureC));
		}

		private static double CelsiusToFahrenheit(double celsius)
		{
			return celsius * 9.0 / 5.0 + 32.0;
		}

		private static string BuildRecommendation(double temperatureC)
		{
			if (temperatureC < 0)
			{
				return "Frigid. Bundle up and limit time outside.";
			}
			if (temperatureC < 10)
			{
				return "Chilly. Layer up if heading out.";
			}
			if (temperatureC < 20)
			{
				return "Mild. A light jacket should do.";
			}
			if (temperatureC < 27)
			{
				return "Warm and pleasant. Great for a walk—use light sunscreen.";
			}
			if (temperatureC < 32)
			{
				return "Hot. Stay hydrated and apply sunscreen.";
			}
			if (temperatureC < 38)
			{
				return "Very hot. Limit strenuous activity and seek shade midday.";
			}

			return "Extreme heat. Avoid midday sun, hydrate frequently, and check on others.";
		}

		private static int CreateDateSeed(DateTime date)
		{
			int seed = date.Year * 10000 + date.Month * 100 + date.Day;
			return seed;
		}
	}

public readonly record struct BlazePrediction(
	DateTime Date,
	double TemperatureC,
	double TemperatureF,
	string Recommendation
);
}


