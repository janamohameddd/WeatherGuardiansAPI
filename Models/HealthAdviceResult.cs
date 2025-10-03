namespace WeatherGuardiansAPI.Models;

public class HealthAdviceResult
{
	public HealthGroup Group { get; set; }
	public DateOnly Date { get; set; }

	// Inputs used for advice
	public double TemperatureC { get; set; }
	public double HumidityPercent { get; set; }
	public int Aqi { get; set; }
	public double TemperatureChangeC { get; set; }

	// Advice
	public string Summary { get; set; } = string.Empty;
	public string Recommendation { get; set; } = string.Empty;
	public GuardianStatus RiskStatus { get; set; } = GuardianStatus.Green;
}


