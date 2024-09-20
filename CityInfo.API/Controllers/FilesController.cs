using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController(FileExtensionContentTypeProvider _fileExtensionContentTypeProvider) : ControllerBase
    {

        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            var pathToFile = "Technical Assessment.pdf";

            if (System.IO.File.Exists(pathToFile) is false){
                return NotFound();
            }

            //Try to get the type of the file
            if(_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out string? contentType) is false)
            {
                //If content type cannot be got successfully, set contentType
                //"application/octet-stream" is the default media type for arbitrary binary data that there are no specific information about
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(pathToFile);
            return File(bytes, contentType, Path.GetFileName(pathToFile));
        }

        
    }
}
