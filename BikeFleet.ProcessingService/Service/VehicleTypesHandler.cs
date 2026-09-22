using BikeFleet.ProcessingService.Data;
using BikeFleet.ProcessingService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace BikeFleet.ProcessingService.Service;
public class VehicleTypesHandler
{
    private readonly DBcontext _dbContext;

    public VehicleTypesHandler(DBcontext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> ProcessEventAsync(string jsonMessage)
    {
        var vehicleType = JsonSerializer.Deserialize<VehicleType>(jsonMessage);

        if (vehicleType == null)
        {
            return false;
        }
        var existingStation = await _dbContext.StationInformation
        .FindAsync(vehicleType.VehicleTypeId);

        if (existingStation == null)
        {
            _dbContext.VehicleTypes.Add(vehicleType);
            await _dbContext.SaveChangesAsync();
        }

        return true;
    }
  
}
