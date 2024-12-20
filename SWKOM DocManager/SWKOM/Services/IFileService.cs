using Microsoft.AspNetCore.Mvc;

namespace SWKOM.Services
{
    /// <summary>
    /// Interface for FileService, making sure Upload, Download and Delete Methods are provided
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Upload a IFormFile which was submitted from a Form on the Web to MinIO Microservice
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task UploadFile(IFormFile file);

        /// <summary>
        /// Download a File from MinIO Microservice asynchronously
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Task<FileStreamResult> DownloadFileAsync(string fileName);

        /// <summary>
        /// Delete a File from MinIO Microservice asynchronously
        /// </summary>
        /// <param name="fileName"></param>
        public Task DeleteFileAsync(string fileName);
    }
}
