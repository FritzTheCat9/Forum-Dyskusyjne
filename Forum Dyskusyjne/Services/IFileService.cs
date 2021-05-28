using Microsoft.AspNetCore.Http;

namespace Forum_Dyskusyjne.Services
{
    public interface IFileService
    {
        string UploadAvatar(IFormFile file);
        bool IsImageValid(IFormFile image);
    }
}