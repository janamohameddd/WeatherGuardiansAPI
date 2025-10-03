namespace WeatherGuardiansAPI.Services;

using WeatherGuardiansAPI.Models;

public interface IHealthService
{
	HealthAdviceResult GetAdvice(HealthGroup group, DateOnly date);
}

public class HealthService : IHealthService
{
	public HealthAdviceResult GetAdvice(HealthGroup group, DateOnly date)
	{
		// Deterministic pseudo-random for reproducible advice per day
		var seed = date.DayNumber * 97 + (group == HealthGroup.Respiratory ? 11 : 19);
		var random = new Random(seed);

		var temperatureC = Math.Round(random.NextDouble() * 25 + 0, 1); // 0-25C baseline
		var humidity = Math.Round(random.NextDouble() * 70 + 20, 0); // 20-90%
		var aqi = random.Next(25, 201); // 25-200
		var tempChange = Math.Round((random.NextDouble() - 0.5) * 10, 1); // -5C to +5C

		var status = GuardianStatus.Green;
		string summary;
		string recommendation;

		if (group == HealthGroup.Respiratory)
		{
			// Respiratory: focus on AQI and humidity
			(string cat, GuardianStatus st, string adv) = aqi switch
			{
				<= 50 => ("Good", GuardianStatus.Green, "Normal activities are fine."),
				<= 100 => ("Moderate", GuardianStatus.Green, "Monitor symptoms if sensitive."),
				<= 150 => ("USG", GuardianStatus.Yellow, "Limit prolonged outdoor exertion."),
				<= 200 => ("Unhealthy", GuardianStatus.Yellow, "Wear a mask outdoors if needed."),
				_ => ("Very Unhealthy", GuardianStatus.Red, "Avoid outdoor activities; use air purifier.")
			};
			status = st;
			summary = $"AQI {aqi} ({cat}), Humidity {humidity}%";
			recommendation = adv + (humidity >= 70 ? " Keep inhalers handy; consider dehumidifier." : "");
		}
		else
		{
			// Cardiac: focus on extreme temps and rapid changes
			bool heatRisk = temperatureC >= 30;
			bool coldRisk = temperatureC <= 5;
			bool changeRisk = Math.Abs(tempChange) >= 4;
			status = (heatRisk || coldRisk || changeRisk) ? GuardianStatus.Yellow : GuardianStatus.Green;
			if (heatRisk || coldRisk)
			{
				recommendation = heatRisk
					? "Hydrate, avoid midday exertion, rest in cool areas."
					: "Dress warmly, limit exposure, warm up gradually.";
			}
			else if (changeRisk)
			{
				recommendation = "Sudden temperature shift: adjust activity; monitor symptoms.";
			}
			else
			{
				recommendation = "Conditions stable; maintain usual care plan.";
			}
			summary = $"Temp {temperatureC}C (Δ {tempChange}C), Humidity {humidity}%";
		}

		return new HealthAdviceResult
		{
			Group = group,
			Date = date,
			TemperatureC = temperatureC,
			HumidityPercent = humidity,
			Aqi = aqi,
			TemperatureChangeC = tempChange,
			Summary = summary,
			Recommendation = recommendation,
			RiskStatus = status
		};
	}
}


