using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeFleet.IngestionService.Models;
using System.Text.Json;

namespace BikeFleet.IngestionService.Services;

public class VehicleTypesService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly KafkaProducerService _producerService;

    public VehicleTypesService(
        IHttpClientFactory httpClientFactory,
        KafkaProducerService producerService)
    {
        _httpClientFactory = httpClientFactory;
        _producerService = producerService;
    }

    public async Task GetVehicleTypesAsync()
    {
        const string url =
            "https://gbfs.lyft.com/gbfs/2.3/bkn/en/vehicle_types.json";

        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var result =
            JsonSerializer.Deserialize<VehicleTypesResponse>(json);

        var vehicleTypes =
            result?.Data?.VehicleTypes ?? new List<VehicleType>();

        foreach (var vehicleType in vehicleTypes)
        {
            if (string.IsNullOrWhiteSpace(vehicleType.VehicleTypeId))
            {
                continue;
            }

            string vehicleTypeJson =
                JsonSerializer.Serialize(vehicleType);

            await _producerService.SendMessageAsync(
                "bike.vehicle-types",
                vehicleType.VehicleTypeId,
                vehicleTypeJson);
        }
    }
}