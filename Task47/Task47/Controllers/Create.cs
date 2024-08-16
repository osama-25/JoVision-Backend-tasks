using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Task47.Models;
using System.IO;

namespace Task47.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Create : ControllerBase
    {
        private readonly string _tempUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "BackendImages");

        public Create()
        {
            if (!Directory.Exists(_tempUploadPath))
            {
                Directory.CreateDirectory(_tempUploadPath);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ImageUpload file)
        {
            if (file.Owner == null) file.Owner = "anonymous";

            if (file.Image == null || file.Image.Length == 0)
                return BadRequest("No file selected");

            var extension = Path.GetExtension(file.Image.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || extension != ".jpg")
                return BadRequest("Invalid file type");

            var filePath = Path.Combine(_tempUploadPath, file.Image.FileName);

            if (System.IO.File.Exists(filePath))
                return BadRequest("File already exists");

            try
            {
                var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "imagefiles.json");
                var reader = new StreamReader(jsonFilePath);
                var jsonString = reader.ReadToEnd();
                List<ImageFile> images = new List<ImageFile>();
                if (!string.IsNullOrEmpty(jsonString))
                {
                    var imageFiles = JsonSerializer.Deserialize<List<ImageFile>>(jsonString);
                    if (imageFiles != null)
                    {
                        images = imageFiles;
                        foreach (var imageFile in imageFiles)
                        {
                            if (file.Owner == imageFile.Owner)
                            {
                                return BadRequest("File already exists");
                            }
                        }
                    }
                }
                reader.Close();

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.Image.CopyToAsync(stream);
                }

                ImageFile imagefile = new ImageFile
                {
                    Owner = file.Owner,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    FileName = file.Image.FileName
                };
                images.Add(imagefile);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(images, options);
                System.IO.File.WriteAllText(jsonFilePath, json);

                return Ok(new { message = "File uploaded and metadata saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
