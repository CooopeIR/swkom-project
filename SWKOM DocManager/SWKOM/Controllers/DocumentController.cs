using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SWKOM.Models;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SWKOM.Controllers
{
    [ApiController]
    [Route("documents")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            _logger = logger;
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
            documentInformation.Id = Guid.NewGuid();

            // 201 Created Response mit Location Header (wo File liegt)
            return CreatedAtAction(nameof(GetDocumentById), new { id = documentInformation.Id },
                JsonSerializer.Serialize(documentInformation));

            //return JsonSerializer.Serialize(documentInformation);
        }

        [HttpGet(Name = "GetAllDocuments")]
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

        [HttpGet("{id}", Name = "GetDocumentById")]
        public ActionResult<DocumentInformation> GetDocumentById(int id)
        {

            if (id == 0)
            {
                return NotFound();
            }

            // You'd fetch the document by its ID here, using your service layer.
            var document = new DocumentInformation
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Title = "Title 1",
                Author = "Me",
                Content = "Line 1.",
                Id = Guid.NewGuid()
            };

            return Ok(document); // Return the document
        }

        //[HttpGet("{id}", Name = "GetDocumentById")]
        //public string GetDocumentById(int id)
        //{
        //    var documentInformation = new DocumentInformation
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now),
        //        Title = "With Id",
        //        Author = "Not Me",
        //        Content = "Line 1. with Id",
        //        Id = id
        //    };

        //    return JsonSerializer.Serialize(documentInformation);
        //}


        [HttpPut("{id}", Name = "UpdateDocumentById")]
        public ActionResult Put(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var documentInformation = new DocumentInformation
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Title = "Title 1",
                Author = "Me",
                Content = "Line 1.",
                Id = Guid.NewGuid()
            };

            documentInformation.Title = "Ich wurde aktualisiert o_0";

            return Ok(documentInformation); 

            //return JsonSerializer.Serialize(documentInformation);
        }



        [HttpDelete("{id}", Name = "DeleteDocumentById")]
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var documentInformation = new DocumentInformation
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Title = "With Id",
                Author = "Not Me",
                Content = "Line 1. with Id",
                Id = Guid.NewGuid()
            };

            // Dann löschen

            return Ok();

            //return JsonSerializer.Serialize(documentInformation);
        }
    }
}
