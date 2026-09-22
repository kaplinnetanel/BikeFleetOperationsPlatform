using BikeFleet.ProcessingService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeFleet.ProcessingService.Data;

public class DBcontext : DbContext
{
    public DBcontext(DbContextOptions<DBcontext> options) : base(options)
    {

    }

    public DbSet<StationInformation> StationInformation { get; set; } = null!;
    public DbSet<VehicleType> VehicleTypes { get; set; } = null!;
}


