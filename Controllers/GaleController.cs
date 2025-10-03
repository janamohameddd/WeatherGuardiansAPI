using System;
using Microsoft.AspNetCore.Mvc;
using WeatherGuardiansAPI.Services;
using WeatherGuardiansAPI.Models;

namespace WeatherGuardiansAPI.Controllers
{
	[ApiController]
	[Route("gale")]
	[Produces("application/json")]
	public sealed class GaleController : ControllerBase
	{
		private readonly IGaleService _galeService;

		public GaleController(IGaleService galeService)
		{
			_galeService = galeService;
		}

		[HttpGet("today")]
		public ActionResult<GuardianResult> GetToday()
		{
			var prediction = _galeService.GetGalePrediction(DateTime.Today);
			var result = MapToGuardianResult("Gale", prediction);
			return Ok(result);
		}

		[HttpGet("future/{year:int}/{month:int}/{day:int}")]
		public ActionResult<GuardianResult> GetFuture(int year, int month, int day)
		{
			try
			{
				var date = new DateTime(year, month, day);
				var prediction = _galeService.GetGalePrediction(date);
				var result = MapToGuardianResult("Gale", prediction);
				return Ok(result);
			}
			catch (ArgumentOutOfRangeException)
			{
				return BadRequest("Invalid date. Please provide a valid year, month, and day.");
			}
		}

		private static GuardianResult MapToGuardianResult(string guardianName, GalePrediction prediction)
		{
			var dateOnly = DateOnly.FromDateTime(prediction.Date);
			double value = prediction.AverageWindSpeedKmH;
			string unit = "km/h";
			string condition = GetWindCondition(prediction.AverageWindSpeedKmH, prediction.GustKmH);
			double confidence = GetDeterministicConfidence(dateOnly);
			string description = "Average wind speed forecast";
			string recommendation = prediction.Recommendation;
			GuardianStatus status = GetWindStatus(prediction.AverageWindSpeedKmH, prediction.GustKmH);

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

		private static string GetWindCondition(double avg, double gust)
		{
			if (avg < 5) return "Calm";
			if (avg < 15) return "Light breeze";
			if (avg < 30) return gust > 45 ? "Breezy, gusty" : "Moderate breeze";
			if (avg < 50) return "Strong";
			if (avg < 70) return "Very strong";
			return "Damaging";
		}

		private static GuardianStatus GetWindStatus(double avg, double gust)
		{
			if (avg >= 50 || gust >= 70) return GuardianStatus.Red;
			if (avg >= 30) return GuardianStatus.Yellow;
			return GuardianStatus.Green;
		}

		private static double GetDeterministicConfidence(DateOnly date)
		{
			int seed = date.DayNumber * 29 + 17;
			var r = new Random(seed);
			return 0.55 + r.NextDouble() * 0.4;
		}
	}
}

