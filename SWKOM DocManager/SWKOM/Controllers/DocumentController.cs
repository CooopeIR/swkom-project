using AutoMapper;
using DocumentDAL.Entities;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FluentValidation;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
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
using System.Text.Json;
using System.Threading.Channels;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using IModel = RabbitMQ.Client.IModel;
using SearchRequest = SWKOM.DTO.SearchRequest;

namespace SWKOM.Controllers
{
    /// <summary>
    /// Document Controller of SWKOM (REST Service) methods (create, get, get by id, delete)
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDocumentProcessor _documentProcessor;
        private readonly IMessageQueueService _messageQueueService;
        private readonly ElasticsearchClient _searchClient;

        /// <summary>
        /// Constructor for DocumentController class, assigning local variables
        /// </summary>
        /// <param name="logger">ILogger(DocumentController) logger</param>
        /// <param name="mapper">IMapper mapper</param>
        /// <param name="httpClientFactory">IHttpClientFactory httpClientFactory</param>
        /// <param name="documentProcessor">IDocumentProcessor documentProcessor</param>
        /// <param name="messageQueueService">IMessageQueueService messageQueueService</param>
        /// <param name="searchClient">ElasticsearchClient searchClient</param>
        public DocumentController(ILogger<DocumentController> logger, IMapper mapper,
            IHttpClientFactory httpClientFactory, IDocumentProcessor documentProcessor,
            IMessageQueueService messageQueueService, ElasticsearchClient searchClient)
        {
            _logger = logger;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _documentProcessor = documentProcessor;
            _messageQueueService = messageQueueService;
            _searchClient = searchClient;
        }

        private async Task EnsureDocumentIndex()
        {
            var indexExistsResponse = await _searchClient.Indices.ExistsAsync("documents");

            if (!indexExistsResponse.Exists)
            {
                var createIndexResponse = await _searchClient.Indices.CreateAsync<Document>("documents", c => c
                    .Mappings(m => m
                        .Properties(p => p
                            .LongNumber(t => t.Id)
                            .Text(t => t.Title)
                            .Text(t => t.Author)
                            .Text(t => t.OcrText)
                        )
                    )
                );

                if (!createIndexResponse.IsValidResponse)
                {
                    // Handle error in index creation
                    Console.WriteLine("Index creation failed: " + createIndexResponse.DebugInformation);
                }
            }
        }

        /// <summary>
        /// Wildcard-Search (QueryString)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ActionResult: 200 OK or 404 not found or 500 failed to search documents</returns>
        [SwaggerOperation(Summary = "Search in OCR-Text of uploaded files with given search term with query string")]
        [HttpPost("search/querystring")]
        public async Task<IActionResult> SearchByQueryString([FromBody] SearchRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            await EnsureDocumentIndex();

            var pingResponse = await _searchClient.PingAsync();
            if (!pingResponse.IsValidResponse)
            {
                Console.WriteLine($"Ping failed: {pingResponse.DebugInformation}");
                return StatusCode(500, "Elasticsearch is unavailable");
            }

            // Access the properties from the request object
            var searchTerm = request.SearchTerm;
            var includeOcr = request.IncludeOcr;

            Console.WriteLine(_searchClient.ElasticsearchClientSettings.DefaultIndex);
            Console.WriteLine(_searchClient.ElasticsearchClientSettings.NodePool);

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "Search term cannot be empty" });
            }

            //string[] fields = includeOcr ? ["title", "author"] : ["ocrtext"];

            //var response = await _searchClient.SearchAsync<Document>(s => s
            //    .Index("documents")
            //    .Query(q => q
            //        .QueryString(qs => qs.Query($"*{searchTerm}*"))
            //    ));

            var fields = new Field[]
            {
                "title",
                "author"
            }.Concat(includeOcr ? new Field[] { "ocrText" } : Array.Empty<Field>()).ToArray();

            var response = await _searchClient.SearchAsync<Document>(s => s
                .Index("documents")
                .Query(q => q
                    .QueryString(qs => qs
                        .Query($"*{searchTerm}*")
                        .Fields(fields)
                    )
                )
            );

            // Log the response from Elasticsearch
            Console.WriteLine("Elasticsearch Response: ");
            Console.WriteLine(JsonSerializer.Serialize(response));
            Console.WriteLine(JsonSerializer.Serialize(response.Documents));

            return HandleSearchResponse(response);
        }

        /// <summary>
        /// Fuzzy-Search with Match(Normalisation)
        /// </summary>
        /// <param name="request">SearchRequest request</param>
        /// <returns>ActionResult: 200 OK or 404 not found or 500 failed to search documents</returns>
        [SwaggerOperation(Summary = "Search in OCR-Text of uploaded files with given search term with Fuzzy")]
        [HttpPost("search/fuzzy")]
        public async Task<IActionResult> SearchByFuzzy([FromBody] SearchRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            await EnsureDocumentIndex();

            // Access the properties from the request object
            var searchTerm = request.SearchTerm;
            var includeOcr = request.IncludeOcr;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "Search term cannot be empty" });
            }

            // Log the received request parameters
            Console.WriteLine("Search Term: " + searchTerm);
            Console.WriteLine("Include OCR: " + includeOcr);

            // Log Elasticsearch Client settings (general configuration details)
            Console.WriteLine("Elasticsearch Client Settings: ");
            Console.WriteLine(_searchClient.ElasticsearchClientSettings.DefaultIndex);
            Console.WriteLine(_searchClient.ElasticsearchClientSettings.NodePool);


            //string[] fields = includeOcr ? ["title", "author"] : ["ocrtext"];

            var fields = new Field[]
            {
                "title^3",
                "author^2"
            }.Concat(includeOcr ? new Field[] { "ocrText" } : Array.Empty<Field>()).ToArray();

            var response = await _searchClient.SearchAsync<Document>(search => search
                .Index("documents")
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Query(searchTerm)
                        .Fields(fields)
                        .Fuzziness(new Fuzziness(2))
                        .Type(TextQueryType.BestFields)
                    )
                )
                .Size(10)
            );

            // Log the response from Elasticsearch
            Console.WriteLine("Elasticsearch Response: ");
            Console.WriteLine(JsonSerializer.Serialize(response));
            Console.WriteLine(JsonSerializer.Serialize(response.Documents));

            Console.WriteLine(response);

            return HandleSearchResponse(response);
        }


        /// <summary>
        /// Checks if response is a valid response and has content (Documents)
        /// </summary>
        /// <param name="response"></param>
        /// <returns>ActionResult: 200 OK or 404 not found or 500 failed to search documents</returns>
        private IActionResult HandleSearchResponse(SearchResponse<Document> response)
        {
            Console.WriteLine(response.Documents);
            if (response.IsValidResponse)
            {
                // Handle errors
                var debugInfo = response.DebugInformation;
                //var error = response.ServerError.Error;
                Console.WriteLine(debugInfo);

                if (!response.Documents.Any())
                    return NotFound(new { message = "No documents found matching the search term." });

                var searchResults = _mapper.Map<List<DocumentItemDTO>>(response.Documents);

                return Ok(searchResults);
            }

            return StatusCode(500, new { message = "Failed to search documents", details = response.DebugInformation });
        }


        /// <summary>
        /// Post a document to save in the database with title, author and uploaded file (Pre-Processing with validation,save-request to DAL, document sent to RabbitMQ)
        /// </summary>
        /// <param name="documentDTO"></param>
        /// <returns>Successful status code with document name and id or error status code</returns>
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

            // Das verarbeitete DTO zu DAL-Item mappen
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

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!); // Erstelle das Verzeichnis, falls es nicht existiert
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await documentDTO.UploadedFile.CopyToAsync(stream);
            }

            // Nachricht an RabbitMQ
            try
            {
                _messageQueueService.SendToFileQueue($"{documentDtoWithId.Id}|{filePath}");
                Console.WriteLine($@"File Path {filePath} an RabbitMQ Queue gesendet.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fehler beim Senden der Nachricht an RabbitMQ: {ex.Message}");
            }

            return Ok(new { message = $"Dateiname {documentDTO.UploadedFile.FileName} für Dokument {documentDTO.Id} erfolgreich gespeichert." });
        }

        /// <summary>
        /// Get all documents from the database
        /// </summary>
        /// <param name="search"></param>
        /// <returns>Success status code with list of DocumentItems or error status code</returns>
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

        /// <summary>
        /// Get a specific document from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success status code with DocumentItem with specified id or error status code</returns>
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

        /// <summary>
        /// Delete a specific document from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success status code for deletion of DocumentItem with specified id or error status code</returns>
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
