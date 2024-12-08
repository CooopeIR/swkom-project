using DocumentDAL.Data;

namespace DocumentDAL.Entities
{
    /// <summary>
    /// Structure with necessary elements for a DocumentItem element (id, title, author, DocumentContent element, DocumentMetadata element, ocr text)
    /// </summary>
    public class DocumentItem
    {
        /// <summary>
        /// ID of DocumentItem element
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Entered title by user
        /// </summary>
        public string Title { get; set; } = String.Empty;
        /// <summary>
        /// Entered author by user
        /// </summary>
        public string Author { get; set; } = String.Empty;
        /// <summary>
        /// Document Content element
        /// </summary>
        public DocumentContent? DocumentContent { get; set; }
        /// <summary>
        /// Document Metadata element
        /// </summary>
        public DocumentMetadata? DocumentMetadata { get; set; }
        /// <summary>
        /// Generated OCR Text
        /// </summary>
        public string? OcrText { get; set; }
    }
}
