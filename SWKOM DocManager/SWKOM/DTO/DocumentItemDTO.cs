using System.ComponentModel.DataAnnotations;

namespace SWKOM.DTO;

/// <summary>
/// Structure with necessary elements for a DocumentItemDTO element (id, title, author, uploaded file, DocumentContentDto element, DocumentMetadataDto element, ocr text)
/// </summary>
public class DocumentItemDTO
{
    /// <summary>
    /// ID of DocumentItem element
    /// </summary>
    public int? Id { get; set; }
    /// <summary>
    /// Entered title by user
    /// </summary>
    public string Title { get; set; } = String.Empty;
    /// <summary>
    /// Entered author by user
    /// </summary>
    public string Author { get; set; } = String.Empty;
    /// <summary>
    /// Uploaded document
    /// </summary>
    public IFormFile UploadedFile { get; set; }
    /// <summary>
    /// Document Content element
    /// </summary>
    public DocumentContentDTO? DocumentContentDto { get; set; }
    /// <summary>
    /// Document Metadata element
    /// </summary>
    public DocumentMetadataDTO? DocumentMetadataDto { get; set; }
    /// <summary>
    /// Generated OCR Text
    /// </summary>
    public string? OcrText { get; set; }

    //public string? FileName { get; set; }
}
