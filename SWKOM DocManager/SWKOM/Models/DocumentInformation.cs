namespace SWKOM.Models
{
    public class DocumentInformation
    {
        public DateOnly Date { get; set; }

        public string Title { get; set; } = "";

        public string? Author { get; set; }

        public string? Content { get; set; }
        public Guid? Id { get; set; }
    }
}
