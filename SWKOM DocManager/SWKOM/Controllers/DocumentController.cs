using Microsoft.AspNetCore.Mvc;
using SWKOM.Models;
using System.Text.Json;
using AutoMapper;
using DocumentDAL.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        public async Task<ActionResult> Create(DocumentInformation documentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = _httpClientFactory.CreateClient("DocumentDAL");
            var item = _mapper.Map<DocumentItem>(documentDTO);
            var response = await client.PostAsJsonAsync("/api/document", item);

            if (response.IsSuccessStatusCode)
            {
                return CreatedAtAction(nameof(GetDocumentById), new { id = item.id }, documentDTO);
            }
            return StatusCode((int)response.StatusCode, "Error creating Document item in DAL");
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
        public async Task<ActionResult> GetDocumentById(int id)
        {

            var client = _httpClientFactory.CreateClient("DocumentDAL");
            var response = await client.GetAsync($"/api/document/{id}");

            if (response.IsSuccessStatusCode)
            {
                var item = await response.Content.ReadFromJsonAsync<DocumentInformation>();
                var dtoItem = _mapper.Map<DocumentItem>(item);
                if (item != null)
                {
                    return Ok(dtoItem);
                }
                return NotFound();
            }
            return StatusCode((int)response.StatusCode, "Error retrieving Document item from DAL");
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
