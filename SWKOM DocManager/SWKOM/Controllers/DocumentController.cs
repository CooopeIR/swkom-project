using AutoMapper;
using DocumentDAL.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Swashbuckle.AspNetCore.Annotations;
using SWKOM.BusinessLogic;
using SWKOM.DTO;
using SWKOM.Services;
using SWKOM.Validators;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using IModel = RabbitMQ.Client.IModel;

namespace SWKOM.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDocumentProcessor _documentProcessor;
        private readonly IMessageQueueService _messageQueueService;

        public DocumentController(ILogger<DocumentController> logger, IMapper mapper,
            IHttpClientFactory httpClientFactory, IDocumentProcessor documentProcessor,
            IMessageQueueService messageQueueService)
        {
            _logger = logger;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _documentProcessor = documentProcessor;
            _messageQueueService = messageQueueService;
        }

        [SwaggerOperation(Summary = "Post a document to save in the database with title, author and uploaded file")]
        [HttpPost(Name = "PostDocument")]
        public async Task<ActionResult> Create([FromForm] DocumentItemDTO documentDTO)
        {
            Console.WriteLine($"DocumentDTO: {documentDTO.Title}, {documentDTO.Author}, {documentDTO.UploadedFile.FileName} / {documentDTO.UploadedFile.ContentType},");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState in invalid");
                return BadRequest(ModelState);
            }

            if (documentDTO.UploadedFile == null || documentDTO.UploadedFile.Length == 0)
            {
                ModelState.AddModelError("DocumentFile", "Keine Datei hochgeladen.");
                return BadRequest(ModelState);
            }
            if (!documentDTO.UploadedFile.FileName.EndsWith(".pdf"))
            {
                ModelState.AddModelError("DocumentFile", "Nur PDF-Dateien sind erlaubt.");
                return BadRequest(ModelState);
            }

            // Empfangenes DTO verarbeiten in Subparts für Speichern in Postgres
            documentDTO = await _documentProcessor.ProcessDocument(documentDTO);


            // Validierung mit FluentValidation
            var validator = new DocumentItemDtoValidator();
            var validationResult = validator.Validate(documentDTO); // Validiere das DTO

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // Das verarbeitet DTO zu DAL-Item mappen
            var item = _mapper.Map<DocumentItem>(documentDTO);


            // Speichere das Item im DAL
            var client = _httpClientFactory.CreateClient("DocumentDAL");
            var saveFileResponse = await client.PostAsJsonAsync("/api/document", item);
            if (!saveFileResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)saveFileResponse.StatusCode, $"Fehler beim Speichern des Dateinamens für Dokument {documentDTO.Id}");
            }

            // Das mittels CreatedAtAction returnierte DocumentItem (inklusive ID von PostgresDB) auslesen
            var createdDocument = await saveFileResponse.Content.ReadFromJsonAsync<DocumentItem>();
            if (createdDocument == null || createdDocument.Id == 0)
            {
                return NotFound("Fehler beim Abrufen der Dokument-ID nach dem Speichern.");
            }

            // Das DocumentItem mit Id zurück zum Document DTO mappen
            var documentDtoWithId = _mapper.Map<DocumentItemDTO>(createdDocument);

            // Datei speichern (lokal im Container) mit der Id von der Datenbank und dem Originalfile aus dem ursprünglichen documentDto
            var filePath = Path.Combine("/app/uploads", documentDTO.UploadedFile.FileName);
            Console.WriteLine(filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!); // Erstelle das Verzeichnis, falls es nicht existiert
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await documentDTO.UploadedFile.CopyToAsync(stream);
            }

            Console.WriteLine($"With Id: {documentDtoWithId.Id}");
            Console.WriteLine($"Without Id: {documentDTO.Id}");

            // Nachricht an RabbitMQ
            try
            {
                //SendToMessageQueue(documentFile.FileName);
                _messageQueueService.SendToQueue($"{documentDtoWithId.Id}|{filePath}");
                Console.WriteLine($@"File Path {filePath} an RabbitMQ Queue gesendet.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fehler beim Senden der Nachricht an RabbitMQ: {ex.Message}");
            }

            return Ok(new { message = $"Dateiname {documentDTO.UploadedFile.FileName} für Dokument {documentDTO.Id} erfolgreich gespeichert." });
        }

        [SwaggerOperation(Summary = "Get all documents from the database")]
        [HttpGet(Name = "GetDocuments")]
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
        [HttpGet("{id}")]
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

        //[SwaggerOperation(Summary = "Update a specific document in the database with the ID of the document (not finished)")]
        //[HttpPut("{id}", Name = "UpdateDocumentById")]
        //public ActionResult Put(int id)
        //{

        //    if (id == 0)
        //    {
        //        return NotFound();
        //    }


        //    return NotFound();
        //}

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

        //[SwaggerOperation(Summary = "Update a specific document from the database with the ID of the document")]
        //[HttpPut("{id}/upload")]
        //public async Task<IActionResult> UploadFile(int id, IFormFile? documentFile)
        //{
        //    Console.WriteLine($"DocumentDTO:");

        //    if (documentFile == null || documentFile.Length == 0)
        //    {
        //        //return BadRequest("Keine Datei hochgeladen.");
        //        ModelState.AddModelError("taskFile", "Keine Datei hochgeladen.");
        //        return BadRequest(ModelState);
        //    }
        //    if (!documentFile.FileName.EndsWith(".pdf"))
        //    {
        //        ModelState.AddModelError("taskFile", "Nur PDF-Dateien sind erlaubt.");
        //        return BadRequest(ModelState);
        //    }

        //    // Hole den Task vom DAL
        //    var client = _httpClientFactory.CreateClient("DocumentDAL");
        //    var response = await client.GetAsync($"/api/document/{id}");
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return NotFound($"Error while fetching document with id {id}");
        //    }

        //    // Mappe das empfangene TodoItem auf ein TodoItemDto
        //    //var documentItem = await response.Content.ReadFromJsonAsync<DocumentItem>();
        //    var documentItem = await response.Content.ReadFromJsonAsync<DocumentItem>();
        //    if (documentItem == null)
        //    {
        //        return NotFound($"Document with id {id} not found.");
        //    }

        //    //var documentItemDto = _mapper.Map<DocumentItem>(documentItem);
        //    //Console.WriteLine($@"[PUT] Gemappter OcrText: {documentItemDto.OcrText}");

        //    // Setze den Dateinamen im DTO
        //    var documentItemDto = _mapper.Map<DocumentItemDTO>(documentItem); // Mappe TodoItem auf TodoItemDto
        //    documentItemDto.FileName = documentFile.FileName;

        //    // Validierung mit FluentValidation
        //    var validator = new DocumentItemDtoValidator();
        //    var validationResult = validator.Validate(documentItemDto); // Validiere das DTO

        //    if (!validationResult.IsValid)
        //    {
        //        return BadRequest(validationResult.Errors);
        //    }

        //    // Mappe wieder zurück zu TodoItem, um es im DAL zu aktualisieren
        //    var updatedTodoItem = _mapper.Map<DocumentItem>(documentItemDto);

        //    // Aktualisiere das Item im DAL
        //    var updateResponse = await client.PutAsJsonAsync($"/api/document/{id}", updatedTodoItem);
        //    if (!updateResponse.IsSuccessStatusCode)
        //    {
        //        return StatusCode((int)updateResponse.StatusCode, $"Fehler beim Speichern des Dateinamens für Dokument {id}");
        //    }

        //    // Datei speichern (lokal im Container)
        //    var filePath = Path.Combine("/app/uploads", documentFile.FileName);
        //    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!); // Erstelle das Verzeichnis, falls es nicht existiert
        //    await using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await documentFile.CopyToAsync(stream);
        //    }

        //    // Nachricht an RabbitMQ
        //    try
        //    {
        //        //SendToMessageQueue(documentFile.FileName);
        //        _messageQueueService.SendToQueue($"{id}|{filePath}");
        //        Console.WriteLine($@"File Path {filePath} an RabbitMQ Queue gesendet.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Fehler beim Senden der Nachricht an RabbitMQ: {ex.Message}");
        //    }

        //    return Ok(new { message = $"Dateiname {documentFile.FileName} für Dokument {id} erfolgreich gespeichert." });
        //}

        /*private void SendToMessageQueue(string fileName)
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
        }*/
    }
}
