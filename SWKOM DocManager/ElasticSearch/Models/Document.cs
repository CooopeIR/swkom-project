namespace ElasticSearch.Models
{
    /// <summary>
    /// Structure with necessary elements for a DocumentItem element (id, title, author, DocumentContent element, DocumentMetadata element, ocr text)
    /// </summary>
    public class Document
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
        /// Generated OCR Text
        /// </summary>
        public string? OcrText { get; set; }
    }
    
}