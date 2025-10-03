namespace WeatherGuardiansAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using WeatherGuardiansAPI.Models;
using WeatherGuardiansAPI.Services;

[ApiController]
[Route("api/haze")]
public class HazeController : ControllerBase
{
	private readonly IHazeService _hazeService;

	public HazeController(IHazeService hazeService)
	{
		_hazeService = hazeService;
	}

	// GET api/haze/{date}
	[HttpGet("{date}")]
	public ActionResult<GuardianResult> GetByDate([FromRoute] DateOnly date)
	{
		var result = _hazeService.GetHazeForecast(date);
		return Ok(result);
	}
}


