using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts;

public class CityInfoContext(DbContextOptions<CityInfoContext> options) :DbContext(options)
{
    
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<PointOfInterest> PointOfInterests => Set<PointOfInterest>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region List of cities
        List<City> cities = [
           new City{
            Name = "Lagos",
            Id = 1,
            Description = "Center of Excellence"
           },

            new City{
            Name="Ogun",
            Id = 2,
            Description = "Gateway State"

           },

            new City{
            Name="Cross River",
            Id = 3,
            Description = "The People's Paradise"
           }
         ];
        #endregion

        #region Points of Interest
        List<PointOfInterest> pointsOfInterest = [
            new PointOfInterest("Marina Blue Line Railway"){
                Id = 1,
                CityId = 1,
                Description="An functional electric-powered rail line in a country with insufficient electricity"
            },
            new PointOfInterest("National Arts Theatre"){

                Id = 2,
                CityId = 1,
                Description="A national monument: relic of the 1977 Festival of Arts and Culture hosted in Nigeria"
            },
            new PointOfInterest("Olumo Rock"){
                Id = 3,
                CityId = 2,
                Description="A historical place of refuge in times of war"
            },
            new PointOfInterest("Tinapa Resort"){

                Id = 4,
                CityId = 3,
                Description="A multi million dollar white elephant project"
            }
        ];
        #endregion

        #region Data-Seeding
        modelBuilder.Entity<City>().HasData(cities);
        modelBuilder.Entity<PointOfInterest>().HasData(pointsOfInterest);
        #endregion


        base.OnModelCreating(modelBuilder); 
    }


    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlite("");
    //    base.OnConfiguring(optionsBuilder);
    //}
}
