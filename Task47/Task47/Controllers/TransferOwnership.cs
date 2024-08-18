using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Task47.Models;

namespace Task47.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferOwnership : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery] string? OldOwner, [FromQuery] string? NewOwner)
        {
            if (OldOwner == null || NewOwner == null)
            {
                return BadRequest("No name entered");
            }
            try
            {
                List<ImageFile> images = null;

                var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "imagefiles.json");
                var reader = new StreamReader(jsonFilePath);
                var jsonString = reader.ReadToEnd();
                reader.Close();

                if (!string.IsNullOrEmpty(jsonString))
                {
                    var imageFiles = JsonSerializer.Deserialize<List<ImageFile>>(jsonString);
                    if (imageFiles != null)
                    {
                        foreach (var imageFile in imageFiles)
                        {
                            if (OldOwner == imageFile.Owner)
                            {
                                imageFile.Owner = NewOwner;
                            }
                        }

                        var options = new JsonSerializerOptions { WriteIndented = true };
                        string json = JsonSerializer.Serialize(imageFiles, options);
                        System.IO.File.WriteAllText(jsonFilePath, json);

                        images = imageFiles
                                .Where(file => file.Owner == NewOwner)
                                .ToList();
                    }
                }
                if (images != null && images.Count != 0)
                {
                    return Ok(images);
                }
                else
                {
                    return BadRequest("No file found with old owner name");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
