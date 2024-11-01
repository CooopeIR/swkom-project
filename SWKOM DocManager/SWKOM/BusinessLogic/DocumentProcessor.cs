using System.Runtime.InteropServices.JavaScript;
using DocumentDAL.Entities;
using SWKOM.DTO;

namespace SWKOM.BusinessLogic;

public class DocumentProcessor : IDocumentProcessor
{
    public DocumentProcessor()
    {
        
    }
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