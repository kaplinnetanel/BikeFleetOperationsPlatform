using BikeFleet.Api.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BikeFleet.Api.Controllers;

[ApiController]
[Route("api")]
public class StationsController : ControllerBase
{
    private readonly StationRepository _stationRepository;

    public StationsController(StationRepository stationRepository)
    {
        _stationRepository = stationRepository;
    }

    [HttpGet("stations")]
    public async Task<IActionResult> GetAllStations()
    {
        var stations = await _stationRepository.GetAllStationsAsync();

        return Ok(stations);
    }

    [HttpGet("stations{id}")]
    public async Task<ActionResult<StationInformation?>> GetStationsByIdAsync(string id)
    {
        var result = await _stationRepository.GetStationsByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
    [HttpGet("stations/{id}/status")]
    public async Task<ActionResult> GetStationStatusAsync(string id)
    {
        var result = await _stationRepository.GetStationStatusAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}

