﻿using CityInfo.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities;

public class City
{
    //public City(string name)
    //{
    //    Name = name;
    //}

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }


    [MaxLength(200)]
    public string? Description { get; set; }

    //Initialize collections to prevent null reference issues
    public ICollection<PointOfInterest> PointsOfInterest { get; set; } = [];

    public override string ToString() => Name;
}
