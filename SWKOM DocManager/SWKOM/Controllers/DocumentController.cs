using AutoMapper;
using DocumentDAL.Entities;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SWKOM.DTO;
using SWKOM.Services;
using SWKOM.Validators;
using Document = SWKOM.DTO.Document;


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
        private readonly IFileService _fileService;
        private readonly ElasticsearchClient _searchClient;


        /// <summary>
        /// Constructor for DocumentController class, assigning local variables
        /// </summary>
        /// <param name="logger">ILogger(DocumentController) logger</param>
        /// <param name="mapper">IMapper mapper</param>
        /// <param name="httpClientFactory">IHttpClientFactory httpClientFactory</param>
        /// <param name="documentProcessor">IDocumentProcessor documentProcessor</param>
        /// <param name="messageQueueService">IMessageQueueService messageQueueService</param>
        /// <param name="fileService"></param>
        public DocumentController(ILogger<DocumentController> logger, IMapper mapper,
            IHttpClientFactory httpClientFactory, IDocumentProcessor documentProcessor,
            IMessageQueueService messageQueueService, IFileService fileService, ElasticsearchClient searchClient)
        {
            _logger = logger;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _documentProcessor = documentProcessor;
            _messageQueueService = messageQueueService;
            _fileService = fileService;
            _searchClient = searchClient;
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
                _logger.LogError("No File uploaded");
                return BadRequest(ModelState);
            }
            if (!documentDTO.UploadedFile.FileName.EndsWith(".pdf"))
            {
                ModelState.AddModelError("DocumentFile", "Nur PDF-Dateien sind erlaubt.");
                _logger.LogWarning("Only PDF-files are allowed, but file uploaded which is no PDF.");
                return BadRequest(ModelState);
            }

            await _fileService.UploadFile(documentDTO.UploadedFile);

            // Empfangenes DTO verarbeiten in Subparts für Speichern in Postgres
            documentDTO = await _documentProcessor.ProcessDocument(documentDTO);


            // Validierung mit FluentValidation
            var validator = new DocumentItemDtoValidator();
            var validationResult = validator.Validate(documentDTO); // Validiere das DTO

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation: DocumentItemDTO is invalid");
                return BadRequest(validationResult.Errors);
            }

            // Das verarbeitete DTO zu DAL-Item mappen
            var item = _mapper.Map<DocumentItem>(documentDTO);


            // Speichere das Item im DAL
            var client = _httpClientFactory.CreateClient("DocumentDAL");
            var saveFileResponse = await client.PostAsJsonAsync("/api/document", item);
            if (!saveFileResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Error with saving document file, Error Code: {StatusCode}", (int)saveFileResponse.StatusCode);
                return StatusCode((int)saveFileResponse.StatusCode, $"Fehler beim Speichern des Dateinamens für Dokument {documentDTO.Id}");
            }

            // Das mittels CreatedAtAction returnierte DocumentItem (inklusive ID von PostgresDB) auslesen
            var createdDocument = await saveFileResponse.Content.ReadFromJsonAsync<DocumentItem>();
            if (createdDocument == null || createdDocument.Id == 0)
            {
                _logger.LogError("Error in reading saved data");
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
                await _messageQueueService.SendToFileQueue($"{documentDtoWithId.Id}|{filePath}");
                Console.WriteLine($@"File Path {filePath} an RabbitMQ Queue gesendet.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error with sending message to RabbitMQ: {message}", ex.Message);
                return StatusCode(500, $"Fehler beim Senden der Nachricht an RabbitMQ: {ex.Message}");
            }

            return Ok(new { message = $"Dateiname {documentDTO.UploadedFile.FileName} für Dokument {documentDTO.Id} erfolgreich gespeichert." });
        }

        /// <summary>
        /// Get all documents from the database
        /// </summary>
        /// <returns>Success status code with list of DocumentItems or error status code</returns>
        [SwaggerOperation(Summary = "Get all documents from the database")]
        [HttpGet(Name = "GetDocuments")]
        public async Task<IActionResult> Get()
        {
            var client = _httpClientFactory.CreateClient("DocumentDAL");

            var response = await client.GetAsync("/api/document");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error in retrieving documents: Code {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, "Error retrieving documents.");
            }

            var items = await response.Content.ReadFromJsonAsync<IEnumerable<DocumentItem>>();
            var sortedItems = items.OrderBy(item => item.Id);
            var documents = _mapper.Map<IEnumerable<DocumentItemDTO>>(sortedItems);

            return Ok(documents);
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

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving Document item from DAL: {StatusCode}", (int)response.StatusCode);
                return StatusCode((int)response.StatusCode, "Error retrieving Document item from DAL");
            }

            var item = await response.Content.ReadFromJsonAsync<DocumentItem>();
            var dtoItem = _mapper.Map<DocumentItemDTO>(item);

            if (item == null)
            {
                _logger.LogWarning("Document with id {id} not found", id);
                return NotFound();
            }

            return Ok(dtoItem);
        }

        /// <summary>
        /// Get a specific document from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success status code with DocumentItem with specified id or error status code</returns>
        [SwaggerOperation(Summary = "Get a specific document from the database with the ID of the document")]
        [HttpGet("view/{id}")]
        public async Task<IActionResult> GetFullDocumentById(int id)
        {
            var client = _httpClientFactory.CreateClient("DocumentDAL");
            var response = await client.GetAsync($"/api/document/view/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving Document item from DAL: Code {StatusCode}", (int)response.StatusCode);
                return StatusCode((int)response.StatusCode, "Error retrieving Document item from DAL");
            }

            var item = await response.Content.ReadFromJsonAsync<DocumentItem>();
            var dtoItem = _mapper.Map<DocumentItemDTO>(item);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(dtoItem);
        }

        /// <summary>
        /// Get a specific document from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success status code for deletion of DocumentItem with specified id or error status code</returns>
        [SwaggerOperation(Summary = "Get a file from MinIO to display")]
        [HttpGet("file/{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            // Fetch document metadata from the DAL
            var client = _httpClientFactory.CreateClient("DocumentDAL");
            var response = await client.GetAsync($"/api/documentcontent/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving Document item from DAL: Code {Code}", (int)response.StatusCode);
                return StatusCode((int)response.StatusCode, "Error fetching Document item in DAL");
            }

            var item = await response.Content.ReadFromJsonAsync<DocumentContent>();
            if (item == null)
                return NotFound($"DocumentContents for id {id} not found");

            if (string.IsNullOrEmpty(item.FileName))
            {
                _logger.LogWarning("FileName is missing");
                return NotFound("FileName is missing.");
            }

            // Fetch the file from MinIO
            var fileResult = await _fileService.DownloadFileAsync(item.FileName);

            // Return the file stream as a response
            return File(fileResult.FileStream, fileResult.ContentType, fileResult.FileDownloadName);
        }

        /// <summary>
        /// Delete a specific document from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success status code for deletion of DocumentItem with specified id or error status code</returns>
        [SwaggerOperation(Summary = "Delete a specific document from the database with the ID of the document")]
        [HttpDelete("{id}", Name = "DeleteDocumentById")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Fetch document metadata from the DAL
                var client = _httpClientFactory.CreateClient("DocumentDAL");
                var contentResponse = await client.GetAsync($"/api/documentcontent/{id}");

                if (!contentResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Error retrieving Document item from DAL: Code {Code}", (int)contentResponse.StatusCode);
                    return StatusCode((int)contentResponse.StatusCode, "Error fetching Document item in DAL");
                }

                var item = await contentResponse.Content.ReadFromJsonAsync<DocumentContent>();
                if (item == null)
                    return NotFound($"DocumentContents for id {id} not found");

                if (string.IsNullOrEmpty(item.FileName))
                {
                    _logger.LogWarning("FileName is missing");
                    return NotFound("FileName is missing.");
                }

                // Delete the file from MinIO
                await _fileService.DeleteFileAsync(item.FileName);

                // Perform the delete operation on Elasticsearch
                var elasticResponse = await _searchClient.DeleteAsync<Document>(id, d => d.Index("documents"));

                if (elasticResponse.IsValidResponse)
                {
                    Console.WriteLine($"Document with ID {id} successfully deleted.");
                }
                else
                {
                    Console.WriteLine($"Failed to delete document with ID {id}: {elasticResponse.DebugInformation}");
                }

                // Delete the document item from the DAL
                var response = await client.DeleteAsync($"/api/document/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error deleting Document item in DAL: Code {StatusCode}", (int)response.StatusCode);
                    return StatusCode((int)response.StatusCode, "Error deleting Document item in DAL");
                }

                return Ok(nameof(Delete));
            }
            catch (HttpRequestException httpEx)
            {
                // Handle errors related to HTTP requests
                Console.WriteLine($"HTTP error: {httpEx.Message}");
                _logger.LogError("An error occurred while processing the request");
                return StatusCode(500, "An error occurred while processing the request.");
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.WriteLine($"Unexpected error: {ex.Message}");
                _logger.LogError("An unexpected error occurred");
                return StatusCode(500, "An unexpected error occurred.");
            }

        }
    }
}
