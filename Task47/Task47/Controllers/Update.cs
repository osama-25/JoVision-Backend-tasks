using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Task47.Models;

namespace Task47.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Update : ControllerBase
    {
        private readonly string _tempUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "BackendImages");
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ImageUpload file)
        {
            if (file.Image == null || file.Owner == null) return BadRequest("No Owner/Image entered");

            var extension = Path.GetExtension(file.Image.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || extension != ".jpg")
                return BadRequest("Invalid file type");

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
                        foreach (var imageFile in images)
                        {
                            if (file.Owner == imageFile.Owner && file.Image.FileName == imageFile.FileName)
                            {
                                imagemetadata = imageFile;

                                imageFile.LastModified = DateTime.Now;

                                var filePath = Path.Combine(_tempUploadPath, file.Image.FileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.Image.CopyToAsync(stream);
                                }

                                var options = new JsonSerializerOptions { WriteIndented = true };
                                string json = JsonSerializer.Serialize(images, options);
                                System.IO.File.WriteAllText(jsonFilePath, json);

                                return Ok(new { message = "File uploaded and metadata updated successfully." });
                            }
                            else if (file.Owner != imageFile.Owner && file.Image.FileName == imageFile.FileName)
                            {
                                return StatusCode(StatusCodes.Status403Forbidden);
                            }
                        }
                    }
                }
                
                return BadRequest("No file found");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
