namespace WeatherGuardiansAPI.Services;

using WeatherGuardiansAPI.Models;

public interface IDrizzleService
{
	GuardianResult GetDrizzleForecast(DateOnly date);
}

public class DrizzleService : IDrizzleService
{
	public GuardianResult GetDrizzleForecast(DateOnly date)
	{
		var seed = date.DayNumber * 17 + 23;
		var random = new Random(seed);

		var probability = Math.Round(random.NextDouble(), 2);
		var intensity = random.Next(0, 3);

		string condition = intensity switch
		{
			0 => probability < 0.3 ? "No Rain" : "Isolated Showers",
			1 => "Light Rain",
			_ => "Moderate Rain"
		};

		var status = probability >= 0.7 || intensity >= 2 ? GuardianStatus.Yellow : GuardianStatus.Green;
		var recommendation = status == GuardianStatus.Green
			? "Carry on. Optional umbrella if out for long."
			: "Keep an umbrella or raincoat handy; watch for slick roads.";

		return new GuardianResult
		{
			GuardianName = "Drizzle",
			Date = date,
			PredictedValue = probability,
			Unit = "%",
			PredictedCondition = condition,
			Confidence = Math.Round(0.6 + random.NextDouble() * 0.4, 2),
			Description = "Rain probability and intensity estimate",
			Recommendation = recommendation,
			Status = status
		};
	}
}


