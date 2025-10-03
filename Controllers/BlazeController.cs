namespace WeatherGuardiansAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using WeatherGuardiansAPI.Models;

[ApiController]
[Route("api/blaze")]
public class BlazeController : ControllerBase
{
    // GET api/blaze/{date}
    [HttpGet("{date}")]
    public ActionResult<GuardianResult> GetByDate([FromRoute] DateOnly date)
    {
        // Mock logic: deterministic pseudo-random based on date
        var seed = date.DayNumber;
        var random = new Random(seed);

        var temperatureC = Math.Round(random.NextDouble() * 25 + 5, 1); // 5C to 30C
        var status = temperatureC >= 28 ? GuardianStatus.Red : GuardianStatus.Green;
        var description = status == GuardianStatus.Green
            ? "Comfortable temperatures expected."
            : "High heat risk. Stay hydrated and limit strenuous activity.";

        var result = new GuardianResult
        {
            GuardianName = "Blaze",
            Date = date,
            PredictedValue = temperatureC,
            Unit = "C",
            Description = description,
            Status = status
        };

        return Ok(result);
    }
}



