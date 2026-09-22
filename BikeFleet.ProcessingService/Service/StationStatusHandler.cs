using BikeFleet.ProcessingService.Models;
using MongoDB.Driver;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BikeFleet.ProcessingService.Service;

public class StationStatusHandler
{
    private readonly IDatabase _redis;
    private readonly IMongoDatabase _mongoDatabase;
   public StationStatusHandler(IDatabase redis ,IMongoDatabase mongoDatabase)
    {
        _redis = redis;
        _mongoDatabase = mongoDatabase;
    }
    public async Task<bool> ProcessEventAsync(string jsonMessage)
    {
        var station = JsonSerializer.Deserialize<StationStatus>(jsonMessage);

        if (station == null)
        {
            return false;
        }
//collection in mongo 
        var collection =_mongoDatabase.GetCollection<StationStatus>("station_status_history");
       

        var key = $"station:{station.StationId}";

        var oldState = await _redis.StringGetAsync(key);
        if (!oldState.HasValue)
        {
            var stationJson = JsonSerializer.Serialize(station);
            await _redis.StringSetAsync(key, stationJson);

            return true;
        }
        var oldStation = JsonSerializer.Deserialize<StationStatus>(oldState!);
        if (oldStation == null)
        {
            return false;
        }
        if (station.AvailableBikes != oldStation.AvailableBikes ||
          station.AvailableDocks != oldStation.AvailableDocks ||
          station.IsRenting != oldStation.IsRenting ||
          station.IsReturning != oldStation.IsReturning)
        {
            Console.WriteLine("Station status changed");

            var stationJson = JsonSerializer.Serialize(station);

            await _redis.StringSetAsync(key, stationJson);
            //save to mongo 
            await collection.InsertOneAsync(station);
        }
        Console.WriteLine($"Station: {station.StationId} - {station}");

            return true;
        
    }
   
}
