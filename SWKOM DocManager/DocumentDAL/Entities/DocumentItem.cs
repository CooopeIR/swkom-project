using DocumentDAL.Data;

namespace DocumentDAL.Entities
{
    /// <summary>
    /// Structure with necessary elements for a DocumentItem element (id, title, author, DocumentContent element, DocumentMetadata element, ocr text)
    /// </summary>
    public class DocumentItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = String.Empty;

        public string Author { get; set; } = String.Empty;
        public DocumentContent? DocumentContent { get; set; }
        public DocumentMetadata? DocumentMetadata { get; set; }
        public string? OcrText { get; set; }
    }
}
