using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class LandmarkProfile:Profile
    {
        public LandmarkProfile()
        {
            CreateMap<Entities.Landmark, Models.LandmarkDto>().ReverseMap();

            CreateMap<Entities.Landmark, Models.LandmarkForCreationDto>().ReverseMap();

            CreateMap<Entities.Landmark, Models.LandmarkForUpdateDto>().ReverseMap();
        }
    }
}
