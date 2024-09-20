using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository(CityInfoContext context) : ICityInfoRepository
    {
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await context.Cities.OrderBy(x => x.Name).ToListAsync();
        }

        /// <summary>
        /// Retrieves a city with the given id
        /// </summary>
        /// <param name="cityId">id of the city</param>
        /// <param name="includePointOfInterest">determines if point of interest is returned with city</param>
        /// <returns>the city with supplied id</returns>
        public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest)
        {
            return includePointOfInterest is true ?

                await context.Cities.Include(x => x.PointsOfInterest)
                .FirstOrDefaultAsync(x => x.Id == cityId)
            :
                await context.Cities
                .FirstOrDefaultAsync(x => x.Id == cityId);
        }

        /// <summary>
        /// Retrieves the point of interest with the supplied id in the city with supplied cityid
        /// </summary>
        /// <param name="cityId">id of city</param>
        /// <param name="pointOfInterestId">id of point of interest</param>
        /// <returns>a point of interest</returns>
        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await context.PointOfInterests
                .FirstOrDefaultAsync(x => x.Id == pointOfInterestId && x.CityId == cityId);                
        }


        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await context.PointOfInterests
                .Where(x => x.CityId == cityId).ToListAsync();
        }
    }
}
