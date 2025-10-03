namespace WeatherGuardiansAPI.Models;

public enum TerraForecastOutcome
{
	Clear,
	RainLikely,
	HeatAdvisory,
	HazeLikely,
	Mixed
}

public class TerraResult
{
	public List<GuardianResult> GuardianResults { get; set; } = new();

	public TerraForecastOutcome FinalForecast { get; set; } = TerraForecastOutcome.Clear;

	public string FinalRecommendation { get; set; } = string.Empty;
}


