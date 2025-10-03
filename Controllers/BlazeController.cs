using System;
using Microsoft.AspNetCore.Mvc;
using WeatherGuardiansAPI.Services;
using WeatherGuardiansAPI.Models;

namespace WeatherGuardiansAPI.Controllers
{
	[ApiController]
	[Route("blaze")]
	[Produces("application/json")]
	public sealed class BlazeController : ControllerBase
	{
		private readonly IBlazeService _blazeService;

		public BlazeController(IBlazeService blazeService)
		{
			_blazeService = blazeService;
		}

		[HttpGet("today")]
		public ActionResult<GuardianResult> GetToday()
		{
			var prediction = _blazeService.GetBlazePrediction(DateTime.Today);
			var result = MapToGuardianResult("Blaze", prediction);
			return Ok(result);
		}

		[HttpGet("future/{year:int}/{month:int}/{day:int}")]
		public ActionResult<GuardianResult> GetFuture(int year, int month, int day)
		{
			try
			{
				var date = new DateTime(year, month, day);
				var prediction = _blazeService.GetBlazePrediction(date);
				var result = MapToGuardianResult("Blaze", prediction);
				return Ok(result);
			}
			catch (ArgumentOutOfRangeException)
			{
				return BadRequest("Invalid date. Please provide a valid year, month, and day.");
			}
		}

		private static GuardianResult MapToGuardianResult(string guardianName, BlazePrediction prediction)
		{
			var dateOnly = DateOnly.FromDateTime(prediction.Date);
			double value = prediction.TemperatureC;
			string unit = "°C";
			string condition = GetTemperatureCondition(prediction.TemperatureC);
			double confidence = GetDeterministicConfidence(dateOnly);
			string description = "Temperature forecast (daily mean)";
			string recommendation = prediction.Recommendation;
			GuardianStatus status = GetTemperatureStatus(prediction.TemperatureC);

			return new GuardianResult
			{
				GuardianName = guardianName,
				Date = dateOnly,
				PredictedValue = Math.Round(value, 2),
				Unit = unit,
				PredictedCondition = condition,
				Confidence = Math.Round(confidence, 2),
				Description = description,
				Recommendation = recommendation,
				Status = status
			};
		}

		private static string GetTemperatureCondition(double c)
		{
			if (c < 0) return "Frigid";
			if (c < 10) return "Chilly";
			if (c < 20) return "Mild";
			if (c < 27) return "Warm";
			if (c < 32) return "Hot";
			if (c < 38) return "Very Hot";
			return "Extreme Heat";
		}

		private static GuardianStatus GetTemperatureStatus(double c)
		{
			if (c >= 38) return GuardianStatus.Red;
			if (c >= 32 || c <= 0) return GuardianStatus.Yellow;
			return GuardianStatus.Green;
		}

		private static double GetDeterministicConfidence(DateOnly date)
		{
			int seed = date.DayNumber * 13 + 5;
			var r = new Random(seed);
			return 0.6 + r.NextDouble() * 0.35;
		}
	}
}

