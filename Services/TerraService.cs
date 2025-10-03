using System;
using WeatherGuardiansAPI.Models;

namespace WeatherGuardiansAPI.Services
{
	public interface ITerraService
	{
		TerraPrediction GetTerraPrediction(DateTime date);
	}

	public sealed class TerraService : ITerraService
	{
		private readonly IBlazeService _blazeService;
		private readonly IGaleService _galeService;
		private readonly IDrizzleService _drizzleService;
		private readonly IHazeService _hazeService;

		public TerraService(
			IBlazeService blazeService,
			IGaleService galeService,
			IDrizzleService drizzleService,
			IHazeService hazeService)
		{
			_blazeService = blazeService;
			_galeService = galeService;
			_drizzleService = drizzleService;
			_hazeService = hazeService;
		}

		public TerraPrediction GetTerraPrediction(DateTime date)
		{
			DateTime targetDate = date.Date;
			DateOnly dateOnly = DateOnly.FromDateTime(targetDate);

			BlazePrediction blaze = _blazeService.GetBlazePrediction(targetDate);
			GalePrediction gale = _galeService.GetGalePrediction(targetDate);
			GuardianResult drizzle = _drizzleService.GetDrizzleForecast(dateOnly);
			GuardianResult haze = _hazeService.GetHazeForecast(dateOnly);

			string finalRecommendation = BuildFinalRecommendation(blaze, gale, drizzle, haze);

			return new TerraPrediction(
				targetDate,
				blaze,
				gale,
				drizzle,
				haze,
				finalRecommendation
			);
		}

		private static string BuildFinalRecommendation(
			BlazePrediction blaze,
			GalePrediction gale,
			GuardianResult drizzle,
			GuardianResult haze)
		{
			bool extremeHeat = blaze.TemperatureC >= 38.0;
			bool veryHot = blaze.TemperatureC >= 32.0;
			bool cold = blaze.TemperatureC <= 0.0;
			bool highWinds = gale.AverageWindSpeedKmH >= 50.0 || gale.GustKmH >= 70.0;
			bool wet = drizzle.Status >= GuardianStatus.Yellow && drizzle.PredictedValue >= 0.5;
			bool poorAir = haze.Status >= GuardianStatus.Yellow && haze.PredictedValue >= 100;

			int riskScore = 0;
			riskScore += extremeHeat ? 3 : veryHot ? 2 : 0;
			riskScore += cold ? 2 : 0;
			riskScore += highWinds ? 2 : 0;
			riskScore += wet ? 1 : 0;
			riskScore += poorAir ? 2 : 0;

			if (riskScore >= 6)
			{
				return "High risk conditions. Consider staying indoors and monitor local advisories.";
			}
			if (riskScore >= 3)
			{
				return "Mixed conditions. Plan cautiously: hydrate, secure items, and consider a mask.";
			}
			return "Favorable conditions. Enjoy the day with standard sun protection.";
		}
	}

	public readonly record struct TerraPrediction(
		DateTime Date,
		BlazePrediction BlazePrediction,
		GalePrediction GalePrediction,
		GuardianResult DrizzlePrediction,
		GuardianResult HazePrediction,
		string FinalRecommendation
	);
}


