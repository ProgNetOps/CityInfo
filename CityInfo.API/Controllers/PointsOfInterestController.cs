using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsOfInterest")]//cityId in any action method will be inferred and there is no need to re-specify it in any other template
[ApiController]
public class PointsOfInterestController(ILogger<PointsOfInterestController> logger, ICityInfoRepository cityInfoRepository,IMapper mapper) : ControllerBase
{

    /// <summary>
    /// Gets all the points of interest for a city
    /// </summary>
    /// <param name="cityId"></param>
    /// <returns></returns>
    [HttpGet]//CityId is already stated on the route attribute and needs not be repeated here
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
    {
        if (await cityInfoRepository.CityExistsAsync(cityId) is false)
        {
            //returns not found if city does not exist
            logger.LogInformation($"City with Id {cityId} does not exist");
            return NotFound();
        }
        else
        {
            //returns a list of the points of interest for a city or an empty list if the city has no point of interest
            var pointsOfInterestForCity = await cityInfoRepository
                .GetPointsOfInterestForCityAsync(cityId);
            return Ok(mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
        }
    }


    /// <summary>
    /// Gets the point of interest with the supplied id for the id of the city
    /// </summary>
    /// <param name="cityId">Id of the city</param>
    /// <param name="pointOfInterestId">Id of the point of interest to retrieve from the city with supplied cityId</param>
    /// <returns></returns>
    [HttpGet("{pointOfInterestId}",Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
    {

      if (await cityInfoRepository.CityExistsAsync(cityId) is false)
        {
            return NotFound();
        }
        else
        {
            var pointOfInterest = await cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            return pointOfInterest is null ? 
                NotFound() :
                Ok(mapper.Map<PointOfInterestDto>(pointOfInterest));
        }
    }


    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
    {
        if(await cityInfoRepository.CityExistsAsync(cityId) is false)
        {
            return NotFound();
        }
        else
        {
            var finalPointOfInterest =  mapper.Map<PointOfInterest>(pointOfInterest);

            await cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);


            //We state the name of the route and use an anonymous object to
            //pass the required parameters for the route, then we
            //pass in the newly-created resource which will be in the response body

            return CreatedAtRoute(nameof(GetPointOfInterest),
            new {
                cityId = cityId,
                pointOfInterestId = createdPointOfInterestToReturn.Id
            },
            createdPointOfInterestToReturn);
        }



    }


    /// <summary>
    /// This method does a full update of the point of interest in a city
    /// </summary>
    /// <param name="cityId">Id of the city</param>
    /// <param name="pointOfInterestId">Id of the point of interest to be updated</param>
    /// <param name="pointOfInterest">The model containing the updated values </param>
    /// <returns></returns>
    [HttpPut("{pointOfInterestId}")]
    public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
    {
        if (await cityInfoRepository.CityExistsAsync(cityId) is false)
        {
            return NotFound();
        }
        else
        {
            var pointOfInterestEntity = await cityInfoRepository
                .GetPointsOfInterestForCityAsync(cityId);

            if(pointOfInterestEntity is null)
            {
                return NotFound();
            }
            else
            {
                //Automapper will overwrite the values in source object with the ones in the destination object
                mapper.Map(pointOfInterest, pointOfInterestEntity);

                await cityInfoRepository.SaveChangesAsync();
            }

            //Since we are not returning any value after the update
            return NoContent();
        }
    }


    /// <summary>
    /// This method does a partial update of the point of interest
    /// </summary>
    /// <param name="cityId">Id of the city</param>
    /// <param name="pointOfInterestId">Id of the point of interest to be updated</param>
    /// <param name="patchDocument">The list operations we want to apply to the point of interest</param>
    /// <returns></returns>
    [HttpPatch("{pointOfInterestId}")]
    public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        if (await cityInfoRepository.CityExistsAsync(cityId) is false) 
        {
            return NotFound();
        }
        else
        {
            var pointOfInterestEntity = cityInfoRepository.GetPointOfInterestForCityAsync(cityId,pointOfInterestId);
            
            if(pointOfInterestEntity is null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            //Apply the patch document to the passed model and check for errors by passing the modelstate too
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);


            //Check the dto to ensure it is still valid after the patch document is applied
            if (ModelState.IsValid is false || TryValidateModel(pointOfInterestToPatch) is false)
            {
                return BadRequest(ModelState);
            }
            else
            {
                await mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

                await cityInfoRepository.SaveChangesAsync();

                return NoContent();
            }

        }
    }


    /// <summary>
    /// Deletes point of interest from a city
    /// </summary>
    /// <param name="cityId">Id of city</param>
    /// <param name="pointOfInterestId">Id of point of interest to delete</param>
    /// <returns></returns>
    [HttpDelete("{pointOfInterestId}")]
    public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        if (await cityInfoRepository.CityExistsAsync(cityId) is false)
        {
            return NotFound();
        }
        else
        {
            var pointOfInterestEntity = await cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if(pointOfInterestEntity is null)
            {
                return NotFound();
            }
            else
            {
                cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
                return NoContent();
            }


        }
    }


}
