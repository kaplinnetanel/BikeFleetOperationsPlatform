using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BikeFleet.Api;

public class StationInformation
{
    [Key]
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
