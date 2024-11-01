namespace SWKOM.DTO;

public class DocumentMetadataDTO
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public DateTime UploadDate { get; set; }
    public int FileSize { get; set; }

    //public DocumentItemDTO DocumentItemDTO { get; set; }
}
