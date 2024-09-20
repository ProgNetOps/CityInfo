namespace CityInfo.API.Models
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        //Initialize collections to prevent null reference issues
        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } = [];

        //computed property
        public int NumberOfPointsOfInterest => PointsOfInterest.Count;

    }
}
