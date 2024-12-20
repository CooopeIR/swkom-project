using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace SWKOM.Services;

/// <summary>
/// Service which handles File Upload, Download and Deletion from MinIO Microservice
/// </summary>
public class FileService : IFileService
{
    private readonly IMinioClient _minioClient;
    private const string BucketName = "uploads";

    /// <summary>
    /// Constructor, which instantiates a new MinIO Client for file operations
    /// </summary>
    public FileService()
    {
        _minioClient = new MinioClient()
            .WithEndpoint("minio", 9000)
            .WithCredentials("minioadmin", "minioadmin")
            .WithSSL(false)
            .Build();
    }

    /// <summary>
    /// Uploads a file to MinIO Microservice
    /// </summary>
    /// <param name="file"></param>
    /// <exception cref="Exception"></exception>
    public async Task UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File is empty or missing");

        await EnsureBucketExists();

        var fileName = Path.GetFileName(file.FileName);
        await using var fileStream = file.OpenReadStream();

        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(file.Length));
    }

    /// <summary>
    /// Downloads a file from MinIO Microservice asynchrono
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<FileStreamResult> DownloadFileAsync(string fileName)
    {
        var memoryStream = new MemoryStream();

        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(BucketName)
            .WithObject(fileName)
            .WithCallbackStream(async stream =>
            {
                await stream.CopyToAsync(memoryStream);
            }));

        memoryStream.Position = 0;

        // Get the correct content type based on the file extension
        var contentType = GetContentType(fileName);

        return new FileStreamResult(memoryStream, contentType)
        {
            FileDownloadName = fileName
        };
    }

    private string GetContentType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
    
    /// <summary>
    /// Deletes a file from MinIO Microservice
    /// </summary>
    /// <param name="fileName"></param>
    /// <exception cref="Exception"></exception>
    public async void DeleteFileAsync(string fileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new Exception("File name cannot be empty.");
            }

            // Check if file exists before attempting deletion
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(BucketName)
                .WithObject(fileName);

            try
            {
                await _minioClient.StatObjectAsync(statObjectArgs);
            }
            catch (ObjectNotFoundException)
            {
                throw new Exception($"File '{fileName}' not found.");
            }

            // Proceed with deletion
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(BucketName)
                .WithObject(fileName));
        }
        catch (MinioException ex)
        {
            // Log the specific MinIO exception
            throw new Exception($"MinIO error while deleting file: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Log the general exception
            throw new Exception($"Unexpected error while deleting file: {ex.Message}");
        }
    }

    private async Task EnsureBucketExists()
    {
        bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(BucketName));
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(BucketName));
        }
    }
}