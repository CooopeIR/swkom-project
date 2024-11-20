using System.ComponentModel.DataAnnotations;

namespace SWKOM.DTO;

public class DocumentItemDTO
{
    public string Title { get; set; } = String.Empty;

    public string Author { get; set; } = String.Empty;

    public IFormFile UploadedFile { get; set; }

    public int? Id { get; set; }

    public DocumentContentDTO? DocumentContentDto { get; set; }

    public DocumentMetadataDTO? DocumentMetadataDto { get; set; }

    //public string? FileName { get; set; }

    public string? OcrText { get; set; }
}
