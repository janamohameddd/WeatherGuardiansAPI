namespace WeatherGuardiansAPI.Models;

public enum GuardianStatus
{
	Green,
	Yellow,
	Red
}

public class GuardianResult
{
	public string GuardianName { get; set; } = string.Empty;

	// Date for which the prediction applies
	public DateOnly Date { get; set; }

	// Generic numeric value (e.g., temperature, probability %, AQI index)
	public double? PredictedValue { get; set; }

	// Unit for the numeric value (e.g., "C", "%", "AQI")
	public string? Unit { get; set; }

	// Human-readable predicted condition/category (e.g., "Light Rain", "Unhealthy")
	public string? PredictedCondition { get; set; }

	// Confidence score in range [0,1]
	public double? Confidence { get; set; }

	// Short description of the prediction context
	public string? Description { get; set; }

	// Actionable recommendation/advice
	public string? Recommendation { get; set; }

	// Overall status signal
	public GuardianStatus Status { get; set; } = GuardianStatus.Green;
}


