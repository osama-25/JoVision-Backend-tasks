using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Task47.Models;

namespace Task47.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Retrieve : ControllerBase
    {
        private readonly string _tempUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "BackendImages");
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string Filename, [FromQuery] string Owner)
        {
            if (Filename == "" || Owner == "") return BadRequest("No Owner/Filename entered");
            try
            {
                ImageFile imagemetadata = null;
                List<ImageFile> images = new List<ImageFile>();

                var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "imagefiles.json");
                var reader = new StreamReader(jsonFilePath);
                var jsonString = reader.ReadToEnd();
                reader.Close();

                if (!string.IsNullOrEmpty(jsonString))
                {
                    var imageFiles = JsonSerializer.Deserialize<List<ImageFile>>(jsonString);
                    if (imageFiles != null)
                    {
                        images = imageFiles;
                        foreach (var imageFile in imageFiles)
                        {
                            if (Owner == imageFile.Owner && (Filename + ".jpg") == imageFile.FileName)
                            {
                                imagemetadata = imageFile;
                                break;
                            }
                        }
                    }
                }

                var filePath = Path.Combine(_tempUploadPath, Filename + ".jpg");
                if (imagemetadata != null)
                {
                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    return File(fileStream, "image/jpeg", Filename + ".jpg");
                }
                else if (imagemetadata == null && System.IO.File.Exists(filePath))
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
                else
                {
                    return BadRequest("No File Found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
