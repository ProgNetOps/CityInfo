using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models;

public class LandmarkForUpdateDto
{
    [Required(ErrorMessage = "Provide a name value")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }
}
