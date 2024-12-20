namespace SWKOM.DTO;

/// <summary>
/// Structure with necessary elements for a DocumentContentDTO element (id, filename, content type, content, document id)
/// </summary>
public class DocumentContentDTO
{
    /// <summary>
    /// ID of DocumentContentDTO element
    /// </summary>
    public int? Id { get; set; }
    /// <summary>
    /// File Name of Document
    /// </summary>
    public string FileName { get; set; } = String.Empty;
    /// <summary>
    /// Content Type of Document
    /// </summary>
    public string ContentType { get; set; } = String.Empty;
    /// <summary>
    /// Content of Document
    /// </summary>
    public byte[]? Content { get; set; }
    /// <summary>
    /// ID of Document
    /// </summary>
    public int? DocumentId { get; set; }
    //public DocumentItemDTO DocumentItemDTO { get; set; }
}