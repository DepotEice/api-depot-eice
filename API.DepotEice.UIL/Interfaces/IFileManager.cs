using API.DepotEice.UIL.Models;

namespace API.DepotEice.UIL.Interfaces
{
    public interface IFileManager
    {
        Task<bool> UploadFileAsync(IFormFile file, string key);
        Task<FileModel?> GetFileAsync(string key);
    }
}
