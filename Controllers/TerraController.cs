using System;
using Microsoft.AspNetCore.Mvc;
using WeatherGuardiansAPI.Services;

namespace WeatherGuardiansAPI.Controllers
{
	[ApiController]
	[Route("terra")]
	[Produces("application/json")]
	public sealed class TerraController : ControllerBase
	{
		private readonly ITerraService _terraService;

		public TerraController(ITerraService terraService)
		{
			_terraService = terraService;
		}

		[HttpGet("today")]
		public ActionResult<TerraPrediction> GetToday()
		{
			var prediction = _terraService.GetTerraPrediction(DateTime.Today);
			return Ok(prediction);
		}

		[HttpGet("future/{year:int}/{month:int}/{day:int}")]
		public ActionResult<TerraPrediction> GetFuture(int year, int month, int day)
		{
			try
			{
				var date = new DateTime(year, month, day);
				var prediction = _terraService.GetTerraPrediction(date);
				return Ok(prediction);
			}
			catch (ArgumentOutOfRangeException)
			{
				return BadRequest("Invalid date. Please provide a valid year, month, and day.");
			}
		}
	}
}

