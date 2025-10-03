namespace WeatherGuardiansAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using WeatherGuardiansAPI.Models;
using WeatherGuardiansAPI.Services;

[ApiController]
[Route("haze")]
public class HazeController : ControllerBase
{
	private readonly IHazeService _hazeService;

	public HazeController(IHazeService hazeService)
	{
		_hazeService = hazeService;
	}

	[HttpGet("today")]
	public ActionResult<GuardianResult> GetToday()
	{
		var result = _hazeService.GetHazeForecast(DateOnly.FromDateTime(DateTime.Today));
		return Ok(result);
	}

	[HttpGet("future/{year:int}/{month:int}/{day:int}")]
	public ActionResult<GuardianResult> GetFuture(int year, int month, int day)
	{
		try
		{
			var date = new DateTime(year, month, day);
			var result = _hazeService.GetHazeForecast(DateOnly.FromDateTime(date));
			return Ok(result);
		}
		catch (ArgumentOutOfRangeException)
		{
			return BadRequest("Invalid date. Please provide a valid year, month, and day.");
		}
	}
}

