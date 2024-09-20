using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")] //can also be written as [Route("api/[controller]")] since cities is the prefix to our controller name
public class CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper) :ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities()
    {
        var cityEntities = await cityInfoRepository.GetCitiesAsync();

        return Ok(mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntities));
    }


    /// <summary>
    /// Gets a city and optionally includes points of interest
    /// </summary>
    /// <param name="id">id of the city</param>
    /// <param name="includePointsOfInterest">boolean that decides if point of interest is included</param>
    /// <returns>city with or without point of interest</returns>
    [HttpGet("{id}")]
    //The return type can't be Task<ActionResult<CityDto>>
    public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
    {
        var city = await cityInfoRepository.GetCityAsync(id,includePointsOfInterest);

        if (city is null)
        {
            return NotFound();
        }
        else if (includePointsOfInterest is true)
        {
            return Ok(mapper.Map<CityDto>(city));
        }
        else
        {
            return Ok(mapper.Map<CityWithoutPointOfInterestDto>(city));
        }
    }



}
