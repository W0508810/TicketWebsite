using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http; // Add this for IFormFile
using System.IO; // Add this for Path

namespace TicketScam.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return string.Empty;

            // Create unique file name
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "shows");

            // Ensure directory exists
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return fileName;
        }

        public void DeleteImage(string? imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName))
                return;

            var filePath = Path.Combine(_environment.WebRootPath, "images", "shows", imageFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}