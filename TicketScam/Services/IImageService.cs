using Microsoft.AspNetCore.Http; // Add this for IFormFile

namespace TicketScam.Services
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile imageFile);
        void DeleteImage(string? imageFileName);
    }
}