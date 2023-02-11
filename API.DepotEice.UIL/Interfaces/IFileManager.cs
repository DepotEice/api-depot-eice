using API.DepotEice.UIL.Models;

namespace API.DepotEice.UIL.Interfaces
{
    /// <summary>
    /// The FileManager interface
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Upload a file to an AWS S3 bucket
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="key">The file's key in S3</param>
        /// <returns><c>true</c> If the upload is successful. <c>false</c> Otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        Task<bool> UploadObjectAsync(IFormFile file, string key);

        /// <summary>
        /// Delete an object based on its key from AWS S3.
        /// </summary>
        /// <param name="key">The object key in AWS. Mostly the file name but can be anything else</param>
        /// <returns><c>true</c> If the operation went successfully. <c>false</c> Otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        Task<bool> DeleteObjectAsync(string key);

        /// <summary>
        /// Get an object based on its key from AWS S3
        /// </summary>
        /// <param name="key">The object's key</param>
        /// <returns>
        /// A <see cref="FileModel"/> containing the stream, content type and the key (The filename is optional for 
        /// the moment)
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        Task<FileModel?> GetObjectAsync(string key);
    }
}
