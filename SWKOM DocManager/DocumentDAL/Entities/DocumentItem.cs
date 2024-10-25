namespace DocumentDAL.Entities
{
    public class DocumentItem
    {
        public DateOnly? date { get; set; }

        public string title { get; set; } = "";

        public string? author { get; set; }

        public string? contentpath { get; set; }

        public string? fileName { get; set; }

        public int id { get; set; }
    }
}
