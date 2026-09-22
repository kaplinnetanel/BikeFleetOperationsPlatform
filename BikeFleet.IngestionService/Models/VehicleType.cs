using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace BikeFleet.IngestionService.Models;

public class VehicleType
{
    [JsonPropertyName("vehicle_type_id")]
    public string VehicleTypeId { get; set; } = string.Empty;

    [JsonPropertyName("form_factor")]
    public string FormFactor { get; set; } = string.Empty;

    [JsonPropertyName("propulsion_type")]
    public string PropulsionType { get; set; } = string.Empty;

    [JsonPropertyName("max_range_meters")]
    public double? MaxRangeMeters { get; set; }
}

public class VehicleTypesResponse
{
    [JsonPropertyName("data")]
    public VehicleTypesData? Data { get; set; }
}

public class VehicleTypesData
{
    [JsonPropertyName("vehicle_types")]
    public List<VehicleType>? VehicleTypes { get; set; }
}