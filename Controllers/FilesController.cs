using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.IO.Compression;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        [HttpGet("download/{id}")]
        public IActionResult DownloadZip(string id)
        {
            try
            {
                // Validate the ID and fetch related files (mock example)
                var files = GetFilesById(id); // Replace with actual logic to fetch files
                if (files == null || files.Count == 0)
                {
                    return NotFound(new { error = "No files found for the given ID" });
                }

                // Create a memory stream to hold the zip file
                using var memoryStream = new MemoryStream();
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var filePath = Path.Combine("Files", file); // Adjust path as needed
                        if (System.IO.File.Exists(filePath))
                        {
                            var fileBytes = System.IO.File.ReadAllBytes(filePath);
                            var zipEntry = archive.CreateEntry(file, CompressionLevel.Fastest);
                            using var zipStream = zipEntry.Open();
                            zipStream.Write(fileBytes, 0, fileBytes.Length);
                        }
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream, "application/zip", $"{id}.zip");
            }
            catch (Exception ex)
            {
                // Log the error (optional)
                Console.WriteLine($"Error generating zip file: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // Mock function to fetch files by ID
        private List<string> GetFilesById(string id)
        {
            // Replace with actual logic to fetch files based on the ID
            if (id == "example")
            {
                return new List<string> { "file1.txt", "file2.txt" };
            }
            return null;
        }
    }
}
