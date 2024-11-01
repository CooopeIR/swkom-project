namespace DocumentDAL.Entities;

public class DocumentMetadata
{
    public int Id { get; set; }
    public DateTime UploadDate { get; set; }
    public int FileSize { get; set; }

    public int DocumentId { get; set; }
    //public DocumentItem DocumentItem { get; set; }
}