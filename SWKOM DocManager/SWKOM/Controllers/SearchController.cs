using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SWKOM.DTO;
using SWKOM.Services;

namespace SWKOM.Controllers
{
    /// <summary>
    /// Controller which handles search requests (fuzzy search, querystring search => allow for 2 errors or exact match)
    /// Optional: Search also in OCR Text of indexed documents
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly IMapper _mapper;
        private readonly ISearchService _searchService;

        /// <summary>
        /// Search Controller constructor, which assigns logger, mapper and searchService from Dependency Injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="searchService"></param>
        public SearchController(
            ILogger<SearchController> logger,
            IMapper mapper,
            ISearchService searchService)
        {
            _logger = logger;
            _mapper = mapper;
            _searchService = searchService;
        }

        /// <summary>
        /// Wildcard-Search (QueryString)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ActionResult: 200 OK or 404 not found or 500 failed to search documents</returns>
        [SwaggerOperation(Summary = "Search in OCR-Text of uploaded files with given search term with query string")]
        [HttpPost("querystring")]
        public async Task<IActionResult> SearchByQueryString([FromBody] SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return BadRequest(new { message = "Search term cannot be empty" });
            }

            var response = await _searchService.QueryStringSearchAsync(request);

            if (!response.IsValidResponse)
            {
                return StatusCode(500, new { message = "Failed to search documents", details = response.DebugInformation });
            }

            if (!response.Documents.Any())
            {
                return NotFound(new { message = "No documents found matching the search term." });
            }

            var searchResults = _mapper.Map<List<DocumentItemDTO>>(response.Documents);
            return Ok(searchResults);
        }

        /// <summary>
        /// Fuzzy-Search with Match(Normalisation)
        /// </summary>
        /// <param name="request">SearchRequest request</param>
        /// <returns>ActionResult: 200 OK or 404 not found or 500 failed to search documents</returns>
        [SwaggerOperation(Summary = "Search in OCR-Text of uploaded files with given search term with Fuzzy")]
        [HttpPost("fuzzy")]
        public async Task<IActionResult> SearchByFuzzy([FromBody] SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return BadRequest(new { message = "Search term cannot be empty" });
            }

            var response = await _searchService.FuzzySearchAsync(request);

            if (!response.IsValidResponse)
            {
                return StatusCode(500, new { message = "Failed to search documents", details = response.DebugInformation });
            }

            if (!response.Documents.Any())
            {
                return NotFound(new { message = "No documents found matching the search term." });
            }

            var searchResults = _mapper.Map<List<DocumentItemDTO>>(response.Documents);
            return Ok(searchResults);
        }
    }
}
