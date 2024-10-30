using DocumentDAL.Data;

namespace DocumentDAL.Entities
{
    public class DocumentItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = String.Empty;

        public string Author { get; set; } = String.Empty;

        public DocumentContent? DocumentContent { get; set; }
        public DocumentMetadata? DocumentMetadata { get; set; }
    }
}
