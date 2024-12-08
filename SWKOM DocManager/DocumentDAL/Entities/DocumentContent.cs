namespace DocumentDAL.Entities;

/// <summary>
/// Structure with necessary elements for a DocumentContent element (id, filename, content type, content, document id)
/// </summary>
public class DocumentContent
{
    public int Id { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public byte[]? Content { get; set; }

    public int DocumentId { get; set; }
}