using Microsoft.AspNetCore.Http; 

namespace TicketScam.Services
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile imageFile);
        void DeleteImage(string? imageFileName);
    }
}