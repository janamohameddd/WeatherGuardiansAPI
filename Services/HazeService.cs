namespace WeatherGuardiansAPI.Services;

using WeatherGuardiansAPI.Models;

public interface IHazeService
{
	GuardianResult GetHazeForecast(DateOnly date);
}

public class HazeService : IHazeService
{
	public GuardianResult GetHazeForecast(DateOnly date)
	{
		// Deterministic pseudo-random based on date
		var seed = date.DayNumber * 31 + 7;
		var random = new Random(seed);

		// Simulate AQI 0-300
		var aqi = random.Next(25, 201);
		(string category, GuardianStatus status, string advice) = aqi switch
		{
			<= 50 => ("Good", GuardianStatus.Green, "Enjoy outdoor activities."),
			<= 100 => ("Moderate", GuardianStatus.Green, "Sensitive groups should monitor symptoms."),
			<= 150 => ("Unhealthy for Sensitive Groups", GuardianStatus.Yellow, "Limit prolonged outdoor exertion if sensitive."),
			<= 200 => ("Unhealthy", GuardianStatus.Yellow, "Consider wearing a mask; reduce outdoor activities."),
			<= 300 => ("Very Unhealthy", GuardianStatus.Red, "Avoid outdoor exertion; use air purifiers indoors."),
			_ => ("Hazardous", GuardianStatus.Red, "Stay indoors; follow local health advisories.")
		};

		return new GuardianResult
		{
			GuardianName = "Haze",
			Date = date,
			PredictedValue = aqi,
			Unit = "AQI",
			PredictedCondition = category,
			Confidence = Math.Round(0.55 + random.NextDouble() * 0.4, 2),
			Description = "Air quality index forecast",
			Recommendation = advice,
			Status = status
		};
	}
}


