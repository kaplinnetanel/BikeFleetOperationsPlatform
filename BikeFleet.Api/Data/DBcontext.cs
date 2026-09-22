using Microsoft.EntityFrameworkCore;

namespace BikeFleet.Api.Data;

public class DBcontext :DbContext
{
    public DBcontext(DbContextOptions<DBcontext> options)
        : base(options)
    {
    }

    public DbSet<StationInformation> StationInformation { get; set; } = null!;

    public DbSet<VehicleType> VehicleTypes { get; set; } = null!;
}
