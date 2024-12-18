namespace SWKOM.DTO
{
    /// <summary>
    /// string SearchTerm and bool IncludeOcr for searching
    /// </summary>
    public class SearchRequest
    {
        /// <summary>
        /// string SearchTerm with getter and setter
        /// </summary>
        public string SearchTerm { get; set; }
        /// <summary>
        /// bool IncludeOcr with getter and setter
        /// </summary>
        public bool IncludeOcr { get; set; }
    }
}
