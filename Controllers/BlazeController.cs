using System;
using Microsoft.AspNetCore.Mvc;
using WeatherGuardiansAPI.Services;

namespace WeatherGuardiansAPI.Controllers
{
	/// <summary>
	/// Controller for Blaze, the heat/sun guardian. Exposes endpoints to get today's
	/// and future heat predictions.
	/// </summary>
	[ApiController]
	[Route("blaze")]
	[Produces("application/json")]
	public sealed class BlazeController : ControllerBase
	{
		private readonly IBlazeService _blazeService;

		/// <summary>
		/// Creates a new instance of <see cref="BlazeController"/>.
		/// </summary>
		/// <param name="blazeService">Injected heat/sun prediction service.</param>
		public BlazeController(IBlazeService blazeService)
		{
			_blazeService = blazeService;
		}

		/// <summary>
		/// Returns Blaze's heat/sun prediction for today.
		/// </summary>
		/// <returns>Prediction including date, temperatures in °C/°F, and a recommendation.</returns>
		[HttpGet("today")]
		public ActionResult<BlazePrediction> GetToday()
		{
			var prediction = _blazeService.GetBlazePrediction(DateTime.Today);
			return Ok(prediction);
		}

		/// <summary>
		/// Returns Blaze's heat/sun prediction for a specific future (or past) date.
		/// </summary>
		/// <param name="year">Four-digit year.</param>
		/// <param name="month">Month (1-12).</param>
		/// <param name="day">Day (1-31, validated per month/year).</param>
		/// <returns>Prediction including date, temperatures in °C/°F, and a recommendation.</returns>
		[HttpGet("future/{year:int}/{month:int}/{day:int}")]
		public ActionResult<BlazePrediction> GetFuture(int year, int month, int day)
		{
			try
			{
				var date = new DateTime(year, month, day);
				var prediction = _blazeService.GetBlazePrediction(date);
				return Ok(prediction);
			}
			catch (ArgumentOutOfRangeException)
			{
				return BadRequest("Invalid date. Please provide a valid year, month, and day.");
			}
		}
	}
}


