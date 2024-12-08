using DocumentDAL.Entities;
using SWKOM.DTO;
using System.Runtime.InteropServices.JavaScript;

namespace SWKOM.BusinessLogic;

/// <summary>
/// DocumentProcessor class with functions to process uploaded document
/// </summary>
public class DocumentProcessor : IDocumentProcessor
{
    /// <summary>
    /// Constructor for DocumentProcessor class
    /// </summary>
    public DocumentProcessor()
    {

    }

    /// <summary>
    /// Process document function to get information (content, file name, content type, filesize uploaded date) out of uploaded document into single variables
    /// </summary>
    /// <param name="documentItemDTO"></param>
    /// <returns>Updated DocumentItemDTO element with information in single variables</returns>
    public async Task<DocumentItemDTO> ProcessDocument(DocumentItemDTO documentItemDTO)
    {

        Console.WriteLine("Processing Document");

        using var memoryStream = new MemoryStream();
        await documentItemDTO.UploadedFile.CopyToAsync(memoryStream);
        var contentBytes = memoryStream.ToArray();

        DocumentContentDTO documentContentDTO = new()
        {
            Content = contentBytes,
            FileName = documentItemDTO.UploadedFile.FileName,
            ContentType = documentItemDTO.UploadedFile.ContentType
        };

        Console.WriteLine(documentContentDTO.FileName);

        DocumentMetadataDTO documentMetaData = new()
        {
            FileSize = (int)documentItemDTO.UploadedFile.Length,
            UploadDate = DateTime.Today.Date
        };

        Console.WriteLine(documentMetaData.FileSize);

        documentItemDTO.DocumentContentDto = documentContentDTO;
        documentItemDTO.DocumentMetadataDto = documentMetaData;
        return documentItemDTO;
    }
}