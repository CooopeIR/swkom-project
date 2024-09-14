using Microsoft.AspNetCore.Mvc;
using SWKOM.Models;

namespace SWKOM.Controllers
{
    [ApiController]
    [Route("")]
    public class GlobalController : ControllerBase
    {
        private readonly ILogger<GlobalController> _logger;

        public GlobalController(ILogger<GlobalController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetGlobalAllDocuments")]
        public ActionResult<IEnumerable<DocumentInformation>> Get()
        {

            var documents = Enumerable.Range(1, 5).Select(index => new DocumentInformation
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Title = "Title 1",
                Author = "Me",
                Content = "Line 1.",
                Id = Guid.NewGuid()
            })
            .ToArray();

            return Ok(documents);
        }
    }
}
