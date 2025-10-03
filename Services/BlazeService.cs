using System;

namespace WeatherGuardiansAPI.Services
{
	/// <summary>
	/// Provides heat/sun predictions for a given date.
	/// </summary>
	public interface IBlazeService
	{
		/// <summary>
		/// Returns a simulated heat prediction and recommendation for the specified date.
		/// The simulation is deterministic per date so the same date yields the same output.
		/// </summary>
		/// <param name="date">Target date (local). Time component is ignored.</param>
		/// <returns>Blaze prediction including temperature in °C/°F and a recommendation.</returns>
		BlazePrediction GetBlazePrediction(DateTime date);
	}

	/// <summary>
	/// Default implementation of heat/sun predictions using a simple, realistic model:
	/// - Seasonal baseline via a sinusoidal annual temperature curve (Northern Hemisphere)
	/// - Deterministic daily variation so predictions are stable for a given date
	/// - Chance of summer heatwaves and winter cold snaps
	/// </summary>
	public sealed class BlazeService : IBlazeService
	{
		/// <inheritdoc />
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

		/// <summary>
		/// Calculates a seasonal temperature baseline (°C) using a sine curve across the year.
		/// Peaks in mid-July and is coldest in mid-January (Northern Hemisphere assumption).
		/// </summary>
		private static double CalculateSeasonalBaseline(DateTime date)
		{
			// Annual parameters
			const double annualMeanC = 15.0;    // Average annual temperature
			const double amplitudeC = 15.0;     // Seasonal swing (+/-)
			const int peakDayOfYear = 196;      // ~July 15th

			int year = date.Year;
			int dayOfYear = date.DayOfYear;

			// 2π over days in this year
			int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
			double radians = 2.0 * Math.PI * (dayOfYear - peakDayOfYear) / daysInYear;

			return annualMeanC + amplitudeC * Math.Cos(radians);
		}

		/// <summary>
		/// Produces a deterministic daily variation (°C) for realism without randomness changing between runs.
		/// Range roughly [-4, +4] °C, shaped by the date-based seed.
		/// </summary>
		private static double GetDeterministicDailyVariation(DateTime date)
		{
			int seed = CreateDateSeed(date);
			var random = new Random(seed);
			double baseVariation = (random.NextDouble() * 2.0 - 1.0) * 4.0; // [-4, +4]

			// Slight weekday/weekend bias: weekends trend a touch warmer for "outdoor vibes"
			double weekendBias = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) ? 0.5 : 0.0;
			return baseVariation + weekendBias;
		}

		/// <summary>
		/// Adds occasional seasonal anomalies (°C) like heatwaves in summer and cold snaps in winter.
		/// </summary>
		private static double GetSeasonalAnomaly(DateTime date)
		{
			int seed = unchecked(CreateDateSeed(date) * 31 + 7);
			var random = new Random(seed);
			int month = date.Month;

			// Summer (Jun-Aug): ~15% chance of a heatwave +2 to +6 °C
			if (month is 6 or 7 or 8)
			{
				double roll = random.NextDouble();
				if (roll < 0.15)
				{
					return 2.0 + random.NextDouble() * 4.0;
				}
			}

			// Winter (Dec-Feb): ~12% chance of cold snap -2 to -6 °C
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

		/// <summary>
		/// Limits implausible values to a safe range for UI.
		/// </summary>
		private static double ClampTemperature(double temperatureC)
		{
			return Math.Max(-25.0, Math.Min(45.0, temperatureC));
		}

		/// <summary>
		/// Converts Celsius to Fahrenheit.
		/// </summary>
		private static double CelsiusToFahrenheit(double celsius)
		{
			return celsius * 9.0 / 5.0 + 32.0;
		}

		/// <summary>
		/// Builds a user-friendly recommendation string based on temperature (°C).
		/// </summary>
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

		/// <summary>
		/// Creates a stable seed from the date for deterministic randomness.
		/// </summary>
		private static int CreateDateSeed(DateTime date)
		{
			// Use YYYYMMDD folded into a 32-bit range; ignore time of day
			int seed = date.Year * 10000 + date.Month * 100 + date.Day;
			return seed;
		}
	}

	/// <summary>
	/// Result object for Blaze predictions.
	/// </summary>
	public readonly record struct BlazePrediction(
		DateTime Date,
		double TemperatureC,
		double TemperatureF,
		string Recommendation
	);
}


