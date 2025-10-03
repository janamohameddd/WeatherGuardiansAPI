namespace WeatherGuardiansAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using WeatherGuardiansAPI.Models;
using WeatherGuardiansAPI.Services;

[ApiController]
[Route("api/drizzle")]
public class DrizzleController : ControllerBase
{
	private readonly IDrizzleService _drizzleService;

	public DrizzleController(IDrizzleService drizzleService)
	{
		_drizzleService = drizzleService;
	}

	// GET api/drizzle/{date}
	[HttpGet("{date}")]
	public ActionResult<GuardianResult> GetByDate([FromRoute] DateOnly date)
	{
		var result = _drizzleService.GetDrizzleForecast(date);
		return Ok(result);
	}
}


