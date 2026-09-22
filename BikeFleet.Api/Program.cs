using BikeFleet.Api.Data;
using BikeFleet.Api.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MongoDB.Driver;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
//mysql
var mysqlConnection =
    Environment.GetEnvironmentVariable("MYSQL_CONNECTION")
    ?? "Server=localhost;Port=3306;Database=BikeFleet;User=root;Password=Secret";

builder.Services.AddDbContext<DBcontext>(options =>
    options.UseMySql(
        mysqlConnection,
        new MySqlServerVersion(new Version(8, 0, 0))));


var redisAddress =
    Environment.GetEnvironmentVariable("REDIS_CONNECTION")
    ?? "localhost:6379";
var redis = ConnectionMultiplexer.Connect(redisAddress);
var database = redis.GetDatabase();
builder.Services.AddSingleton<StackExchange.Redis.IDatabase>(database);

var mongoAddress =
    Environment.GetEnvironmentVariable("MONGO_CONNECTION")
    ?? "mongodb://localhost:27017";
var mongoClient = new MongoClient(mongoAddress);
var mongoDatabase = mongoClient.GetDatabase("BikeFleet");
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<StationRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
