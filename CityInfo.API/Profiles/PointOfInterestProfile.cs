using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class PointOfInterestProfile:Profile
    {
        public PointOfInterestProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>().ReverseMap();

            CreateMap<Entities.PointOfInterest, Models.PointOfInterestForCreationDto>().ReverseMap();

            CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdateDto>().ReverseMap();

        }
        
    }
}
