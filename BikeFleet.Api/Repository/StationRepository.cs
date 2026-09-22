using BikeFleet.Api.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Security.Cryptography.X509Certificates;

namespace BikeFleet.Api.Repository
{
    public class StationRepository
    {
        private readonly DBcontext _dBcontext;
        private readonly IDatabase _redis;
        private readonly IMongoDatabase _mongoDatabase;
        public StationRepository(DBcontext dBcontext, IDatabase redis, IMongoDatabase mongoDatabase)
        {
            _dBcontext = dBcontext;
            _redis = redis;
            _mongoDatabase = mongoDatabase;
        }
        public async Task<List<StationInformation>> GetAllStationsAsync()
        {
            return await _dBcontext.StationInformation.ToListAsync();
        }
        public async Task<StationInformation?> GetStationsByIdAsync(string id)
        {
            var result = await _dBcontext.StationInformation.FirstOrDefaultAsync(p => p.StationId == id);
            if (result == null)
            {
                return null;
            }
            return result;
        }
        public async Task<string?> GetStationStatusAsync(string id)
        {
            var key = $"station:{id}";
            var result = await _redis.StringGetAsync(key);

            if (!result.HasValue)
            {
                return null;
            }

            return result.ToString();
        }
    }
}


