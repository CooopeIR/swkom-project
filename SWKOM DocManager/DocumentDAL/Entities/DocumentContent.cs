namespace DocumentDAL.Entities;

public class DocumentContent
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte[] Content { get; set; }

    public int DocumentId { get; set; }
    public DocumentItem DocumentItem { get; set; } = null!;
}