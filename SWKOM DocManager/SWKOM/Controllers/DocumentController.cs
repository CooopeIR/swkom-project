using AutoMapper;
using DocumentDAL.Entities;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.Annotations;
using SWKOM.DTO;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using IModel = RabbitMQ.Client.IModel;

using System.Text;
using SWKOM.DTO;
using SWKOM.BusinessLogic;

namespace SWKOM.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IDocumentProcessor _documentProcessor;

        public DocumentController(ILogger<DocumentController> logger, IMapper mapper,
            IHttpClientFactory httpClientFactory, IDocumentProcessor documentProcessor)
        {
            _logger = logger;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _documentProcessor = documentProcessor;

            // Stelle die Verbindung zu RabbitMQ her
            var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "user", Password = "password" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Deklariere die Queue
            _channel.QueueDeclare(queue: "file_queue", durable: false, exclusive: false, autoDelete: false,
                arguments: null);

        }

        [SwaggerOperation(Summary = "Post a document to save in the database with title, author and updladed file")]
        [HttpPost(Name = "PostDocument")]
        public async Task<ActionResult> Create([FromForm] DocumentItemDTO documentDTO)
        {

            Console.WriteLine($"DocumentDTO: {documentDTO.Title}, {documentDTO.Author}, {documentDTO.UploadedFile.FileName} / {documentDTO.UploadedFile.ContentType},");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState in invalid");
                return BadRequest(ModelState);
            }

            documentDTO = await _documentProcessor.ProcessDocument(documentDTO);

            var client = _httpClientFactory.CreateClient("DocumentDAL");
            Console.WriteLine(documentDTO.DocumentContentDto == null ? "DocumentContentDto is null" : "DocumentContentDto is populated");
            Console.WriteLine(documentDTO.DocumentMetadataDto == null ? "DocumentMetadataDto is null" : "DocumentMetadataDto is populated");

            var item = _mapper.Map<DocumentItem>(documentDTO);

            Console.WriteLine(item.DocumentContent == null ? "item Content is null" : "Item ContentDto is populated");
            Console.WriteLine(item.DocumentMetadata == null ? "Item Metadata is null" : "Item MetadataDto is populated");


            Console.WriteLine($"Title: {item.Title}, Author: {item.Author}, " +
                              $"DocContent: (Type){item.DocumentContent.ContentType} (Content) {item.DocumentContent.Content} (FileName) {item.DocumentContent.FileName}" +
                              $"DocMetaData: (FileSize) {item.DocumentMetadata.FileSize} (Date) {item.DocumentMetadata.UploadDate}");

            var response = await client.PostAsJsonAsync("/api/document", item);

            if (response.IsSuccessStatusCode)   
            {
                return CreatedAtAction(nameof(GetDocumentById), new { id = item.Id }, documentDTO);
            }
            return StatusCode((int)response.StatusCode, "Error creating Document item in DAL");
        }

        [SwaggerOperation(Summary = "Get all documents from the database")]
        [HttpGet(Name = "GetAllDocuments")]
        public async Task<ActionResult> Get([FromQuery] string? search)
        {
            var client = _httpClientFactory.CreateClient("DocumentDAL");

            var response = await client.GetAsync("/api/document");

            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadFromJsonAsync<IEnumerable<DocumentItemDTO>>();
                var documents = _mapper.Map<IEnumerable<DocumentItemDTO>>(items);

                if (!string.IsNullOrEmpty(search))
                {
                    documents = documents.Where(d =>
                        d.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        d.Author.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                return Ok(documents);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error retrieving documents.");
            }
        }

        [SwaggerOperation(Summary = "Get a specific document from the database with the ID of the document")]
        [HttpGet("{id}", Name = "GetDocumentById")]
        public async Task<ActionResult> GetDocumentById(int id)
        {

            var client = _httpClientFactory.CreateClient("DocumentDAL");
            var response = await client.GetAsync($"/api/document/{id}");

            if (response.IsSuccessStatusCode)
            {
                var item = await response.Content.ReadFromJsonAsync<DocumentItemDTO>();
                var dtoItem = _mapper.Map<DocumentItem>(item);
                if (item != null)
                {
                    return Ok(dtoItem);
                }
                return NotFound();
            }
            return StatusCode((int)response.StatusCode, "Error retrieving Document item from DAL");
        }

        [SwaggerOperation(Summary = "Update a specific document in the database with the ID of the document (not finished)")]
        [HttpPut("{id}", Name = "UpdateDocumentById")]
        public ActionResult Put(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }


            return NotFound();
        }

        [SwaggerOperation(Summary = "Delete a specific document from the database with the ID of the document")]
        [HttpDelete("{id}", Name = "DeleteDocumentById")]
        public async Task<ActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("DocumentDAL");
            var response = await client.DeleteAsync($"/api/document/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Ok(nameof(Delete));
            }
            return StatusCode((int)response.StatusCode, "Error deleting Document item in DAL");
        }

        [SwaggerOperation(Summary = "Update a specific document from the database with the ID of the document")]
        [HttpPut("{id}/upload")]
        public async Task<IActionResult> UploadFile(int id, IFormFile? documentFile)
        {
            Console.WriteLine($"DocumentDTO:");


            if (documentFile == null || documentFile.Length == 0)
            {
                return BadRequest("Keine Datei hochgeladen.");
            }

            // Hole den Task vom DAL
            var client = _httpClientFactory.CreateClient("DocumentDAL");
            var response = await client.GetAsync($"/api/document/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound($"Error while fetching document with id {id}");
            }

            // Mappe das empfangene TodoItem auf ein TodoItemDto
            var documentItem = await response.Content.ReadFromJsonAsync<DocumentItemDTO>();
            if (documentItem == null)
            {
                return NotFound($"Document with id {id} not found.");
            }

            var documentItemDto = _mapper.Map<DocumentItem>(documentItem);

            //// Setze den Dateinamen im DTO
            //documentItemDto.fileName = documentFile.FileName;

            // Aktualisiere das Item im DAL, nutze das DTO
            var updateResponse = await client.PutAsJsonAsync($"/api/document/{id}", documentItemDto);
            if (!updateResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)updateResponse.StatusCode, $"Fehler beim Speichern des Dateinamens für Dokument {id}");
            }

            // Nachricht an RabbitMQ
            try
            {
                SendToMessageQueue(documentFile.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fehler beim Senden der Nachricht an RabbitMQ: {ex.Message}");
            }

            return Ok(new { message = $"Dateiname {documentFile.FileName} für Dokument {id} erfolgreich gespeichert." });
        }

        private void SendToMessageQueue(string fileName)
        {
            // Sende die Nachricht in den RabbitMQ channel/queue
            var body = Encoding.UTF8.GetBytes(fileName);
            _channel.BasicPublish(exchange: "", routingKey: "file_queue", basicProperties: null, body: body);
            Console.WriteLine($@"[x] Sent {fileName}");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
