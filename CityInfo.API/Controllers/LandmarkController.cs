using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/landmarks")]
[ApiController]
public class LandmarkController(ILogger<LandmarkController> logger,
    IMailService mailService,
    ICityInfoRepository cityInfoRepository,
    IMapper mapper) : ControllerBase
{
    private readonly ILogger<LandmarkController> _logger = logger;
    private readonly IMailService _mailService = mailService;
    private readonly ICityInfoRepository _cityInfoRepository = cityInfoRepository;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LandmarkDto>>> GetLandmarks(int cityId)
    {
        if(await _cityInfoRepository.CityExistsAsync(cityId) is false)
        {
            _logger.LogInformation($"City with Id, {cityId}, not found when accessing landmarks.");
            return NotFound();
        }
     
        var landmarksForCity = await _cityInfoRepository.GetLandmarksForCityAsync(cityId);

        return Ok(_mapper.Map<IEnumerable<LandmarkDto>>(landmarksForCity));
    }


    [HttpGet("{landmarkId}", Name = "GetLandmark")]
    public async Task<ActionResult<LandmarkDto>> GetLandmark(int cityId, int landmarkId)
    {
        if (await _cityInfoRepository.CityExistsAsync(cityId) is false)
        {
            _logger.LogInformation($"City with Id, {cityId}, not found when accessing landmarks.");
            return NotFound();
        }

        var landmark = await _cityInfoRepository.GetLandmarkForCityAsync(cityId, landmarkId);

        return landmark is null ?
            NotFound(landmark)
            : Ok(_mapper.Map<LandmarkDto>(landmark));
    }


    [HttpPost]
    public async Task<ActionResult<LandmarkDto>> CreateLandmark(int cityId, LandmarkForCreationDto landmark)
    {
        //Find city
        if (await _cityInfoRepository.CityExistsAsync(cityId) is false)
        {
            return NotFound();
        }

        var latestLandmark = _mapper.Map<Entities.Landmark>(landmark);

        await _cityInfoRepository.AddLandmarkForCityAsync(cityId, latestLandmark);

        await _cityInfoRepository.SaveChangesAsync();

        var createdLandmarkToReturn = _mapper.Map<Models.LandmarkDto>(latestLandmark);

        /*The first parameter is the 'Name' value passed in the Http verb of the action method referred to*/
        return CreatedAtRoute(nameof(GetLandmark),
            new
            {
                cityId = cityId,
                landmarkId = createdLandmarkToReturn.Id
            },
            createdLandmarkToReturn);

    }


    [HttpPut("{landMarkId}")]
    public async Task<ActionResult> UpdateLandmarkDto(int cityId, int landmarkId, LandmarkForUpdateDto landmark)
    {
        //Find city
        if (await _cityInfoRepository.CityExistsAsync(cityId) is false)
        {
            return NotFound();
        }

        //Find Landmark
        var landmarkEntity = await _cityInfoRepository.GetLandmarkForCityAsync(cityId, landmarkId);

        if (landmarkEntity is null)
        {
            return NotFound();
        }

        //Automapper overrides the values in the destination with what's in the source object
        _mapper?.Map(landmark, landmarkEntity);

        //Persist changes to the database
        await _cityInfoRepository.SaveChangesAsync();

        return NoContent(); //A 204 No Content

    }


    [HttpPatch("{landmarkId}")]
    public async Task<ActionResult> PartiallyUpdateLandmark(int cityId, int landmarkId, JsonPatchDocument<LandmarkForUpdateDto> patchDocument)
    {
        //Find city
        if (await _cityInfoRepository.CityExistsAsync(cityId) is false)
        {
            return NotFound();
        }

        //Find Landmark
        var landmarkEntity = await _cityInfoRepository.GetLandmarkForCityAsync(cityId, landmarkId);

        if (landmarkEntity is null)
        {
            return NotFound();
        }

        //Map the landmark to LandmarkForUpdateDto
        var landmarkToPatch = _mapper.Map<LandmarkForUpdateDto>(landmarkEntity);

        //Apply the patch document
        patchDocument.ApplyTo(landmarkToPatch, ModelState);

        //Validate the state of the DTO
        if (ModelState.IsValid is false)
        {
            return BadRequest(ModelState);
        }

        if (TryValidateModel(landmarkToPatch) is false)
        {
            return BadRequest(ModelState);
        }

        //Map the changes back to the entity from the DTO
        _mapper?.Map(landmarkToPatch, landmarkEntity);

        //Persist changes to the database
        _cityInfoRepository?.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{landmarkId}")]
    public async Task<ActionResult> DeleteLandmark(int cityId, int landmarkId)
    {
        //Find city
        if (await _cityInfoRepository.CityExistsAsync(cityId) is false)
        {
            return NotFound();
        }

        //Find Landmark
        var landmarkEntity = await _cityInfoRepository.GetLandmarkForCityAsync(cityId, landmarkId);

        if (landmarkEntity is null)
        {
            return NotFound();
        }

        _cityInfoRepository.DeleteLandmark(landmarkEntity);

        _mailService.Send("Landmark deleted.", $"Landmark {landmarkEntity.Name} with Id {landmarkEntity.Id} was deleted.");

        return NoContent();
    }


}