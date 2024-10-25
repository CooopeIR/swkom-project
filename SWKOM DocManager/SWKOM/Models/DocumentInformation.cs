namespace SWKOM.Models
{
    public class DocumentInformation
    {
        public DateOnly? Date { get; set; }

        public string Title { get; set; } = "";

        public string? Author { get; set; }

        public string? Contentpath { get; set; }

        public string? FileName { get; set; }

        public int? Id { get; set; }
    }
}
