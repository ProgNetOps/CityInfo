using CityInfo.API.Models;

namespace CityInfo.API;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }

    //This static property returns an instance of this class [ singleton pattern: ensures we keep working on the same store as long as we do not restart the web server; we get the same effect if a singleton instance of this class is registered in services collection and injected into any controller. ]

    //public static CitiesDataStore current { get; } = new CitiesDataStore();

    public CitiesDataStore()
    {
        Cities = new List<CityDto>
        {
            new CityDto
            {
                Id = 1,
                Name = "Lagos",
                Description = "The most industrialized city in the country",
                PointsOfInterest = [
                    new PointOfInterestDto{
                        Id = 1,
                        Name= "Oshodi park",
                        Description = "Government's effort at modernizing transportation"
                    }, 
                    new PointOfInterestDto{

                        Id = 2,
                        Name= "Dangote Refinery",
                        Description = "A privately-owned functional refinery where heavily-funded government-owned refineries are kaput"
                    }, 
                    new PointOfInterestDto{

                        Id = 3,
                        Name= "National Art's Theatre",
                        Description = "A relic of African's cultural heritage"
                    },
                ]

            },
            new CityDto
            {
                Id = 2,
                Name = "Adamawa",
                Description = "Has a large landmass",
                PointsOfInterest = [
                    new PointOfInterestDto{
                        Id = 1,
                        Name= "Oshodi park",
                        Description = "Government's effort at modernizing transportation"
                    },
                    new PointOfInterestDto{

                        Id = 2,
                        Name= "Dangote Refinery",
                        Description = "A privately-owned functional refinery where heavily-funded government-owned refineries are kaput"
                    },
                    new PointOfInterestDto{

                        Id = 3,
                        Name= "National Art's Theatre",
                        Description = "A relic of African's cultural heritage"
                    },
                ]


            },
            new CityDto
            {
                Id = 3,
                Name = "Rivers",
                Description = "Lots of creeks",
                PointsOfInterest = [
                    new PointOfInterestDto{
                        Id = 1,
                        Name= "Light Rail",
                        Description = "Government's effort at revolutionizing transportation"
                    },
                    new PointOfInterestDto{

                        Id = 2,
                        Name= "Dangote Refinery",
                        Description = "A privately-owned functional refinery where heavily-funded government-owned refineries are kaput"
                    },
                    new PointOfInterestDto{

                        Id = 3,
                        Name= "National Art's Theatre",
                        Description = "A relic of African's cultural heritage"
                    },
                ]


            },
            new CityDto
            {
                Id = 4,
                Name = "Imo",
                Description = "Rochas was a governor",
                PointsOfInterest = [
                    new PointOfInterestDto{
                        Id = 1,
                        Name= "Zuma'a Statue",
                        Description = "The status of the jailed ex president of South Africa"
                    },
                    new PointOfInterestDto{

                        Id = 2,
                        Name= "Dangote Refinery",
                        Description = "A privately-owned functional refinery where heavily-funded government-owned refineries are kaput"
                    },
                    new PointOfInterestDto{

                        Id = 3,
                        Name= "National Art's Theatre",
                        Description = "A relic of African's cultural heritage"
                    },
                ]


            },
        };
    }
}
