using BikeFleet.IngestionService.Services;
using Microsoft.Extensions.DependencyInjection;
var services = new ServiceCollection();

services.AddHttpClient();

var kafkaAddress =
    Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS")
    ?? "localhost:9092";

services.AddSingleton<KafkaProducerService>(
    new KafkaProducerService(kafkaAddress));

services.AddTransient<StationInformationService>();
services.AddTransient<StationStatusService>();
services.AddTransient<VehicleTypesService>();

var serviceProvider = services.BuildServiceProvider();

var stationInformationService =
    serviceProvider.GetRequiredService<StationInformationService>();

await stationInformationService.GetStationsAsync();

var stationstatusservice = serviceProvider.GetRequiredService<StationStatusService>();


await stationstatusservice.GetStationStatusAsync();

var vehicleTypesservice = serviceProvider.GetRequiredService<VehicleTypesService>();


await vehicleTypesservice.GetVehicleTypesAsync();


var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

var lastReferenceUpdate = DateTime.Now;

while (await timer.WaitForNextTickAsync())
{

    await stationstatusservice.GetStationStatusAsync();

    if (DateTime.Now - lastReferenceUpdate >= TimeSpan.FromHours(1))
    {
        await stationInformationService.GetStationsAsync();
        await vehicleTypesservice.GetVehicleTypesAsync();

        lastReferenceUpdate = DateTime.Now;
    }
    
}