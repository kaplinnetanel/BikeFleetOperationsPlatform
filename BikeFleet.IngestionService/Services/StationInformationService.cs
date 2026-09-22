using BikeFleet.IngestionService.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BikeFleet.IngestionService.Services;

public class StationInformationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly KafkaProducerService _producerService;

    public StationInformationService(IHttpClientFactory httpClientFactory, KafkaProducerService producerService)
    {
        _httpClientFactory = httpClientFactory;
        _producerService = producerService;
    }

    public async Task GetStationsAsync()
    {
        const string url =
            "https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_information.json";

        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<StationInformationResponse>(json);

        var stations = result?.Data?.Stations ?? new List<StationInformation>();


        var validStations = 0;
        foreach (var station in stations)
        {
            if (string.IsNullOrWhiteSpace(station.StationId))
            {
             
                continue;
            }
            if (station.Latitude < -90 || station.Latitude > 90)
            {
               
                continue;
            }
            if (station.Longitude < -180 || station.Longitude > 180)
            {
               
                continue;
            }
            if (station.Capacity < 0 )
            {
                continue;
            }
            validStations++;
            string stationJson = JsonSerializer.Serialize(station);

            await _producerService.SendMessageAsync(
                "bike.station-information",
                station.StationId,
                stationJson);
        }
      
    }

}


  