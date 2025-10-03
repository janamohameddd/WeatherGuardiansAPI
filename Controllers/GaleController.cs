using System;
using Microsoft.AspNetCore.Mvc;
using WeatherGuardiansAPI.Services;

namespace WeatherGuardiansAPI.Controllers
{
	/// <summary>
	/// Controller for Gale, the wind guardian. Exposes endpoints to get today's
	/// and future wind predictions.
	/// </summary>
	[ApiController]
	[Route("gale")]
	[Produces("application/json")]
	public sealed class GaleController : ControllerBase
	{
		private readonly IGaleService _galeService;

		/// <summary>
		/// Creates a new instance of <see cref="GaleController"/>.
		/// </summary>
		/// <param name="galeService">Injected wind prediction service.</param>
		public GaleController(IGaleService galeService)
		{
			_galeService = galeService;
		}

		/// <summary>
		/// Returns Gale's wind prediction for today.
		/// </summary>
		/// <returns>Prediction including date, average wind, gusts, and a recommendation.</returns>
		[HttpGet("today")]
		public ActionResult<GalePrediction> GetToday()
		{
			var prediction = _galeService.GetGalePrediction(DateTime.Today);
			return Ok(prediction);
		}

		/// <summary>
		/// Returns Gale's wind prediction for a specific future (or past) date.
		/// </summary>
		/// <param name="year">Four-digit year.</param>
		/// <param name="month">Month (1-12).</param>
		/// <param name="day">Day (1-31, validated per month/year).</param>
		/// <returns>Prediction including date, average wind, gusts, and a recommendation.</returns>
		[HttpGet("future/{year:int}/{month:int}/{day:int}")]
		public ActionResult<GalePrediction> GetFuture(int year, int month, int day)
		{
			try
			{
				var date = new DateTime(year, month, day);
				var prediction = _galeService.GetGalePrediction(date);
				return Ok(prediction);
			}
			catch (ArgumentOutOfRangeException)
			{
				return BadRequest("Invalid date. Please provide a valid year, month, and day.");
			}
		}
	}
}


