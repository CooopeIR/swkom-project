namespace SWKOM.DTO;

public class DocumentContentDTO
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte[] Content { get; set; }
    //public DocumentItemDTO DocumentItem { get; set; }
}