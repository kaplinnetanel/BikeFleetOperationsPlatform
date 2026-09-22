using BikeFleet.IngestionService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace BikeFleet.IngestionService.Services;

public class StationStatusService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly KafkaProducerService _producerService;

    public StationStatusService(
        IHttpClientFactory httpClientFactory,
        KafkaProducerService producerService)
    {
        _httpClientFactory = httpClientFactory;
        _producerService = producerService;
    }
    public async Task GetStationStatusAsync()
    {
        const string url =
            "https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_status.json";

        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<StationStatusResponse>(json);

        var stations = result?.Data?.Stations ?? new List<StationStatus>();

        foreach (var station in stations)
        {
            if (string.IsNullOrWhiteSpace(station.StationId))
            {
                continue;
            }

            if (station.AvailableBikes < 0)
            {
                continue;
            }

            if (station.AvailableDocks < 0)
            {
                continue;
            }

            if (station.IsRenting != 0 && station.IsRenting != 1)
            {
                continue;
            }

            if (station.IsReturning != 0 && station.IsReturning != 1)
            {
                continue;
            }
            
    Console.WriteLine($"Processing station status: {station.StationId}");
            string stationJson = JsonSerializer.Serialize(station);

            await _producerService.SendMessageAsync(
                "bike.station-status",
                station.StationId,
                stationJson);
        }


    }
}


