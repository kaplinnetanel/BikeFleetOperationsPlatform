using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace BikeFleet.IngestionService.Models;

public class StationInformation
{
    [JsonPropertyName("station_id")]
    public string StationId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lon")]
    public double Longitude { get; set; }

    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }
}
public class StationInformationResponse
{
    [JsonPropertyName("data")]
    public StationInformationData Data { get; set; } = new();
}

public class StationInformationData
{
    [JsonPropertyName("stations")]
    public List<StationInformation> Stations { get; set; } = new();
}