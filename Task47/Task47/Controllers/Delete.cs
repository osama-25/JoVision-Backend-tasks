using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Task47.Models;
using System.IO;
using System.Net;

namespace Task47.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class Delete : ControllerBase
    {
        private readonly string _tempUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "BackendImages");
        [HttpGet]
        public IActionResult Get([FromQuery] string Filename = "", string Owner = "")
        {
            if (Filename == "" || Owner == "") return BadRequest("No Owner/Filename entered");
            try
            {
                ImageFile imagemetadata = null;
                List<ImageFile> images = new List<ImageFile>();
                
                var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "imagefiles.json");
                var reader = new StreamReader(jsonFilePath);
                var jsonString = reader.ReadToEnd();
                
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
                reader.Close();

                var filePath = Path.Combine(_tempUploadPath, Filename + ".jpg");
                if (System.IO.File.Exists(filePath) && imagemetadata != null)
                {
                    System.IO.File.Delete(filePath);
                    images.Remove(imagemetadata);
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(images, options);
                    System.IO.File.WriteAllText(jsonFilePath, json);
                    return Ok(new { message = "File deleted successfully." });
                }
                else if(imagemetadata == null && System.IO.File.Exists(filePath)) 
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
