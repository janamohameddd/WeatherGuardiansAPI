namespace WeatherGuardiansAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using WeatherGuardiansAPI.Models;
using WeatherGuardiansAPI.Services;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
	private readonly IHealthService _healthService;

	public HealthController(IHealthService healthService)
	{
		_healthService = healthService;
	}

	// GET api/health/{group}/{date}
	[HttpGet("{group}/{date}")]
	public ActionResult<HealthAdviceResult> GetAdvice([FromRoute] HealthGroup group, [FromRoute] DateOnly date)
	{
		var result = _healthService.GetAdvice(group, date);
		return Ok(result);
	}
}


