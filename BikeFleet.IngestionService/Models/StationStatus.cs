using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BikeFleet.IngestionService.Models;

public class StationStatus
{
    [JsonPropertyName("station_id")]
    public string StationId { get; set; } = string.Empty;

    [JsonPropertyName("num_bikes_available")]
    public int AvailableBikes { get; set; }

    [JsonPropertyName("num_docks_available")]
    public int AvailableDocks { get; set; }

    [JsonPropertyName("is_renting")]
    public int IsRenting { get; set; }

    [JsonPropertyName("is_returning")]
    public int IsReturning { get; set; }

    [JsonPropertyName("last_reported")]
    public long LastReported { get; set; }
}
public class StationStatusResponse
{
    [JsonPropertyName("data")]
    public StationStatusData? Data { get; set; }
}

public class StationStatusData
{
    [JsonPropertyName("stations")]
    public List<StationStatus>? Stations { get; set; }
}