using BikeFleet.ProcessingService.Data;
using BikeFleet.ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BikeFleet.ProcessingService.Service;

public class StationInformationHandler
{
    private readonly DBcontext _dbContext;
    public StationInformationHandler(DBcontext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ProcessEventAsync(string jsonMessage)
    {
        var station = JsonSerializer.Deserialize<StationInformation>(jsonMessage);

        if (station == null)
        {
            return false;
        }
        var existingStation = await _dbContext.StationInformation
         .FindAsync(station.StationId);

        if (existingStation == null)
        {
            _dbContext.StationInformation.Add(station);
            await _dbContext.SaveChangesAsync();
        }
        return true;
    }
}