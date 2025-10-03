using System;

namespace WeatherGuardiansAPI.Services
{
	/// <summary>
	/// Defines wind prediction capabilities for the Gale guardian.
	/// </summary>
	public interface IGaleService
	{
		/// <summary>
		/// Returns a simulated wind prediction for the specified date.
		/// The simulation is deterministic per date so the same date yields the same output.
		/// </summary>
		/// <param name="date">Target date (local). Time component is ignored.</param>
		/// <returns>Wind prediction including average speed, gusts, and recommendation.</returns>
		GalePrediction GetGalePrediction(DateTime date);
	}

	/// <summary>
	/// Provides realistic-but-simple wind predictions:
	/// - Seasonal baseline (windier in late fall/winter, calmer in summer)
	/// - Deterministic daily variation for stability per date
	/// - Occasional gusts for realism
	/// </summary>
	public sealed class GaleService : IGaleService
	{
		/// <inheritdoc />
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

		/// <summary>
		/// Calculates a seasonal baseline wind speed (km/h) using a sine curve across the year.
		/// Wind tends to be stronger in late fall/winter and calmer mid-summer (Northern Hemisphere).
		/// </summary>
		private static double CalculateSeasonalBaselineKmH(DateTime date)
		{
			// Annual baseline parameters
			const double annualMeanKmH = 16.0;   // Average daily mean wind speed
			const double amplitudeKmH = 8.0;     // Seasonal swing (+/-)
			const int peakDayOfYear = 25;        // ~Late January for peak winds

			int year = date.Year;
			int dayOfYear = date.DayOfYear;
			int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

			double radians = 2.0 * Math.PI * (dayOfYear - peakDayOfYear) / daysInYear;
			// Use cosine to align peak around the chosen day
			return annualMeanKmH + amplitudeKmH * Math.Cos(radians);
		}

		/// <summary>
		/// Produces a deterministic daily variation (km/h) in approximately [-6, +6] range.
		/// </summary>
		private static double GetDeterministicDailyVariationKmH(DateTime date)
		{
			int seed = CreateDateSeed(date);
			var random = new Random(seed);
			double baseVariation = (random.NextDouble() * 2.0 - 1.0) * 6.0; // [-6, +6]

			// Slight mid-week bump (commuter traffic-induced urban turbulence narrative for fun)
			double weekdayBias = (date.DayOfWeek is DayOfWeek.Tuesday or DayOfWeek.Wednesday or DayOfWeek.Thursday) ? 0.8 : 0.0;
			return baseVariation + weekdayBias;
		}

		/// <summary>
		/// Computes a gust speed (km/h) based on the average and deterministic chance.
		/// Gusts are at least the average and can exceed by 20-60% on gusty days.
		/// </summary>
		private static double GetGustKmH(DateTime date, double averageKmH)
		{
			int seed = unchecked(CreateDateSeed(date) * 37 + 11);
			var random = new Random(seed);

			// Base gust factor: always at least 10% above average
			double gustFactor = 1.10 + random.NextDouble() * 0.25; // 1.10 - 1.35

			// Seasonal gustiness: more likely in shoulder seasons (Mar-Apr, Oct-Nov)
			bool shoulderSeason = date.Month is 3 or 4 or 10 or 11;
			if (shoulderSeason && random.NextDouble() < 0.35)
			{
				gustFactor += 0.2 + random.NextDouble() * 0.25; // add 0.2 - 0.45
			}

			double gust = averageKmH * gustFactor;
			// Ensure some minimum spread between average and gust for UX clarity
			double minGust = Math.Max(averageKmH + 4.0, averageKmH * 1.12);
			gust = Math.Max(gust, minGust);

			return ClampGustSpeed(gust);
		}

		/// <summary>
		/// Builds a recommendation string based on average and gust wind speeds.
		/// </summary>
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

		/// <summary>
		/// Caps average wind speeds to a reasonable range for UI.
		/// </summary>
		private static double ClampAverageSpeed(double kmh)
		{
			return Math.Max(0.0, Math.Min(80.0, kmh));
		}

		/// <summary>
		/// Caps gust wind speeds to a reasonable range for UI.
		/// </summary>
		private static double ClampGustSpeed(double kmh)
		{
			return Math.Max(0.0, Math.Min(120.0, kmh));
		}

		/// <summary>
		/// Creates a stable seed from the date for deterministic randomness.
		/// </summary>
		private static int CreateDateSeed(DateTime date)
		{
			int seed = date.Year * 10000 + date.Month * 100 + date.Day;
			return seed;
		}
	}

	/// <summary>
	/// Result object for Gale predictions.
	/// </summary>
	public readonly record struct GalePrediction(
		DateTime Date,
		double AverageWindSpeedKmH,
		double GustKmH,
		string Recommendation
	);
}


