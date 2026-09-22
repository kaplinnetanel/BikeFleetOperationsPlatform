using BikeFleet.ProcessingService.Data;
using BikeFleet.ProcessingService.Models;
using BikeFleet.ProcessingService.Service;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StackExchange.Redis;







var services = new ServiceCollection();


services.AddTransient<StationInformationHandler>();
services.AddTransient<StationStatusHandler>();
services.AddTransient<VehicleTypesHandler>();



//redis
var redisAddress =
    Environment.GetEnvironmentVariable("REDIS_CONNECTION")
    ?? "localhost:6379";
var redis = ConnectionMultiplexer.Connect(redisAddress);
var database = redis.GetDatabase();
services.AddSingleton<IDatabase>(database);


//Mongo 
var mongoAddress =
    Environment.GetEnvironmentVariable("MONGO_CONNECTION")
    ?? "mongodb://localhost:27017";
var mongoClient = new MongoClient(mongoAddress);
var mongoDatabase = mongoClient.GetDatabase("BikeFleet");
services.AddSingleton<IMongoDatabase>(mongoDatabase);

//Mysql
var mysqlConnection =
    Environment.GetEnvironmentVariable("MYSQL_CONNECTION")
    ?? "Server=localhost;Port=3306;Database=BikeFleet;User=root;Password=Secret";

services.AddDbContext<DBcontext>(options =>
    options.UseMySql(
        mysqlConnection,
        new MySqlServerVersion(new Version(8, 0, 0))));
//kafka
var serviceProvider = services.BuildServiceProvider();


//יצירת הטבלאות ב MYSQL
using (var scope = serviceProvider.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DBcontext>();
    await db.Database.EnsureCreatedAsync();
}



var consumerConfig = new ConsumerConfig
{
    BootstrapServers = 
    Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS")
    ?? "localhost:9092",

    GroupId = "bike-fleet-processing",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false
};





var topic1 = "bike.station-information";
var topic2 =  "bike.station-status";
var topic3 =  "bike.vehicle-types";
using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
consumer.Subscribe(new[] { topic1, topic2,topic3 });

while (true)
{
    try
    {
        var result = consumer.Consume(TimeSpan.FromSeconds(10));
        if (result == null || result.Message.Value == null)
        {
            continue;
        }

        using var scope = serviceProvider.CreateScope();

        if (result.Topic == topic1)
        {

            var processingService = scope.ServiceProvider.GetRequiredService<StationInformationHandler>();
            if (await processingService.ProcessEventAsync(result.Message.Value))
            {
                consumer.Commit(result);
                Console.WriteLine("Traffic message processed successfully.");
            }
        }
        else if (result.Topic == topic2)
        {
            var processingService = scope.ServiceProvider.GetRequiredService<StationStatusHandler>();
            if (await processingService.ProcessEventAsync(result.Message.Value))
            {
                consumer.Commit(result);
                Console.WriteLine("Weather message processed successfully.");
            }
        }
        else if (result.Topic == topic3)
        {
            var processingService = scope.ServiceProvider.GetRequiredService<VehicleTypesHandler>();
            if (await processingService.ProcessEventAsync(result.Message.Value))
            {
                consumer.Commit(result);
                Console.WriteLine("Weather message processed successfully.");
            }
        }
    }
    catch (ConsumeException ex)
    {
        Console.WriteLine($"Kafka is not ready yet: {ex.Error.Reason}");
        await Task.Delay(2000);
    }
}
