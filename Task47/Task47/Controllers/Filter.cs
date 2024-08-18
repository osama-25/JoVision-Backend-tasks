using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Task47.Models;

namespace Task47.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Filter : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromForm] DateTime? CreationDate, [FromForm] DateTime? ModificationDate, [FromForm] string? Owner, [FromForm] FilterType? filter)
        {
            if (filter == null || CreationDate == null || ModificationDate == null || Owner == null)
            {
                return BadRequest("missing values");
            }
            try
            {
                List<ImageFile> images = new List<ImageFile>();

                var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "imagefiles.json");
                var reader = new StreamReader(jsonFilePath);
                var jsonString = reader.ReadToEnd();
                reader.Close();

                if (filter == FilterType.ByModificationDate)
                {
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var imageFiles = JsonSerializer.Deserialize<List<ImageFile>>(jsonString);
                        if (imageFiles != null)
                        {
                            images = imageFiles
                                     .Where(file => file.LastModified < ModificationDate)
                                     .ToList();
                        }
                    }
                }
                else if (filter == FilterType.ByOwner)
                {
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var imageFiles = JsonSerializer.Deserialize<List<ImageFile>>(jsonString);
                        if (imageFiles != null)
                        {
                            images = imageFiles
                                     .Where(file => file.Owner == Owner)
                                     .ToList();
                        }
                    }
                }
                else if (filter == FilterType.ByCreationDateDescending)
                {
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var imageFiles = JsonSerializer.Deserialize<List<ImageFile>>(jsonString);
                        if (imageFiles != null)
                        {
                            images = imageFiles
                                     .Where(file => file.Created > CreationDate)
                                     .OrderByDescending(file => file.Created)
                                     .ToList();
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var imageFiles = JsonSerializer.Deserialize<List<ImageFile>>(jsonString);
                        if (imageFiles != null)
                        {
                            images = imageFiles
                                     .Where(file => file.Created > CreationDate)
                                     .OrderBy(file => file.Created)
                                     .ToList();
                        }
                    }
                }
                if (images != null)
                {
                    return Ok(images);
                }
                else
                {
                    return BadRequest("No data found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
