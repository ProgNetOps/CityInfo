using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsOfInterest")]//cityId in any action method will be inferred and there is no need to re-specify it in any other template
[ApiController]
public class PointsOfInterestController(ILogger<PointsOfInterestController> logger, ICityInfoRepository cityInfoRepository, CitiesDataStore citiesDataStore) : ControllerBase
{

    /// <summary>
    /// Gets all the points of interest for a city
    /// </summary>
    /// <param name="cityId"></param>
    /// <returns></returns>
    [HttpGet]//CityId is already stated on the route attribute and needs not be repeated here
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        try
        {
            //throw new Exception("i am an exception to the rule");
            var city = citiesDataStore.Cities.FirstOrDefault(q => q.Id == cityId);

            if (city is null)
            {
                logger.LogInformation($"City with id {cityId} was not found when accessing points of interest");
                return NotFound();
            }
            else
            {
                return Ok(city.PointsOfInterest);
            }
        }
        catch (Exception ex)
        {
            logger.LogCritical($"Exceptions while getting points of interest for city with id {cityId}.",ex);

            return StatusCode(500,"An error occured while handling your request");
        }


    }


    /// <summary>
    /// Gets the point of interest with the supplied id for the id of the city
    /// </summary>
    /// <param name="cityId">Id of the city</param>
    /// <param name="pointOfInterestId">Id of the point of interest to retrieve from the city with supplied cityId</param>
    /// <returns></returns>
    [HttpGet("{pointOfInterestId}",Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
    {

        var city = citiesDataStore.Cities.FirstOrDefault(q => q.Id == cityId);

        if (city is null)
        {
            return NotFound();
        }
        else
        {
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(q => q.Id == pointOfInterestId);

            return pointOfInterest is null ? NotFound() : Ok(pointOfInterest);
        }
    }


    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
    {
        var city = citiesDataStore.Cities.FirstOrDefault(q => q.Id == cityId);

        if (city is null)
        {
            return NotFound();
        }

        //DEMO PURPOSES - To be improved later
        //get the max id in the data store then increment it by 1
        var maxPointOfInterestId = citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);


        var finalPointOfInterest = new PointOfInterestDto
        {
            Id = maxPointOfInterestId + 1,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };

        city.PointsOfInterest.Add(finalPointOfInterest);

        //We state the name of the route and use an anonymous object to
        //pass the required parameters for the route, then we
        //pass in the newly-created resource which will be in the response body

        return CreatedAtRoute(nameof(GetPointOfInterest), new
        {
            cityId = cityId,
            pointOfInterestId = finalPointOfInterest.Id
        },
        finalPointOfInterest);


    }


    /// <summary>
    /// This method does a full update of the point of interest in a city
    /// </summary>
    /// <param name="cityId">Id of the city</param>
    /// <param name="pointOfInterestId">Id of the point of interest to be updated</param>
    /// <param name="pointOfInterest">The model containing the updated values </param>
    /// <returns></returns>
    [HttpPut("{pointOfInterestId}")]
    public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
    {
        //find city
        var city = citiesDataStore.Cities.FirstOrDefault(q => q.Id == cityId);

        //find pointOfInterest to update
        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(m => m.Id == pointOfInterestId);

        if (city is null || pointOfInterestFromStore is null)
        {
            return NotFound();
        }
        else
        {
            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

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
    public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var city = citiesDataStore.Cities.FirstOrDefault(q => q.Id == cityId);

        //find pointOfInterest to update
        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(m => m.Id == pointOfInterestId);

        if (city is null || pointOfInterestFromStore is null)
        {
            return NotFound();
        }
        else
        {
            //pointOfInterestToPatch is not the model but it may contain invalid values, so, we have to validate its values
            var pointOfInterestToPatch = new PointOfInterestForUpdateDto
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            //Apply the patch document to the passed model and check for errors by passing the modelstate too
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if(ModelState.IsValid is false || TryValidateModel(pointOfInterestToPatch) is false)
            {
                return BadRequest(ModelState);
            }
            else
            {
                pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
                pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
            }

            return NoContent();
        }
    }


    /// <summary>
    /// Deletes point of interest from a city
    /// </summary>
    /// <param name="cityId">Id of city</param>
    /// <param name="pointOfInterestId">Id of point of interest to delete</param>
    /// <returns></returns>
    [HttpDelete("{pointOfInterestId}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
        var pointOfInterestFromStore = city?.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);

        if (city is null || pointOfInterestFromStore is null)
        {
            return NotFound();
        }
        else
        {
            city.PointsOfInterest.Remove(pointOfInterestFromStore);
            return NoContent();
        }
    }


}
