namespace DocumentDAL.Entities;

/// <summary>
/// Structure with necessary elements for a DocumentMetadata element (id, upload date, filesize, document id)
/// </summary>
public class DocumentMetadata
{
    /// <summary>
    /// ID of DocumentMetadata element
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Upload Date of document
    /// </summary>
    public DateTime UploadDate { get; set; }

    /// <summary>
    /// Filesize of uploaded document
    /// </summary>
    public int? FileSize { get; set; }

    /// <summary>
    /// ID of Document
    /// </summary>
    public int DocumentId { get; set; }
    //public DocumentItem DocumentItem { get; set; }
}