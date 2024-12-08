namespace DocumentDAL.Entities;

/// <summary>
/// Structure with necessary elements for a DocumentContent element (id, filename, content type, content, document id)
/// </summary>
public class DocumentContent
{
    /// <summary>
    /// ID of DocumentContent element
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// File Name of Document
    /// </summary>
    public string? FileName { get; set; }
    /// <summary>
    /// Content Type of Document
    /// </summary>
    public string? ContentType { get; set; }
    /// <summary>
    /// Content of Document
    /// </summary>
    public byte[]? Content { get; set; }
    /// <summary>
    /// ID of Document
    /// </summary>
    public int DocumentId { get; set; }
}