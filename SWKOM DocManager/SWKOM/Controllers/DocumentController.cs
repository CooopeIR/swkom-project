using Microsoft.AspNetCore.Mvc;
using SWKOM.Models;
using System.Text.Json;
using AutoMapper;

namespace SWKOM.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public DocumentController(ILogger<DocumentController> logger, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost(Name = "PostDocument")]
        public ActionResult<DocumentInformation> Post()
        {
            // Dokument empfangen
            var documentInformation = new DocumentInformation
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Title = "Title 1",
                Author = "Me",
                Content = "Line 1."
            };

            // Dokument speichern / verarbeiten
            //documentInformation.Id = Guid.NewGuid();

            // 201 Created Response mit Location Header (wo File liegt)
            return CreatedAtAction(nameof(GetDocumentById), new { id = documentInformation.Id },
                JsonSerializer.Serialize(documentInformation));
            //return JsonSerializer.Serialize(documentInformation);
        }
        

        [HttpGet(Name = "GetAllDocuments")]
        public async Task<ActionResult> Get()
        {
            var client = _httpClientFactory.CreateClient("DocumentDAL");
            var response = await client.GetAsync("/api/document");

            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadFromJsonAsync<IEnumerable<DocumentInformation>>();
                var documents = _mapper.Map<IEnumerable<DocumentInformation>>(items);
                return Ok(documents);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error retrieving documents.");
            }
        }

        [HttpGet("{id}", Name = "GetDocumentById")]
        public ActionResult<DocumentInformation> GetDocumentById(int id)
        {

            if (id == 0)
            {
                return NotFound();
            }

            return NotFound();

        }

        [HttpPut("{id}", Name = "UpdateDocumentById")]
        public ActionResult Put(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }


            return NotFound();
        }

        [HttpDelete("{id}", Name = "DeleteDocumentById")]
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
