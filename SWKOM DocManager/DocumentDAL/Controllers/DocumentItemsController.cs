using DocumentDAL.Entities;
using DocumentDAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DocumentDAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController(IDocumentItemRepository repository) : ControllerBase
    {
        [SwaggerOperation(Summary = "DAL: Get all documents from the database")]
        [HttpGet(Name = "GetAllDocumentsFromDB")]
        public async Task<IEnumerable<DocumentItem>> GetAllAsync()
        {
            return await repository.GetAllAsync();
        }

        [SwaggerOperation(Summary = "DAL: Get a specific document from the database with the ID of the document")]
        [HttpGet("{id}", Name= "GetDocumentById"), ]
        public async Task<DocumentItem> GetAsyncById(int id)
        {
            return await repository.GetByIdAsync(id, false);
        }

        [SwaggerOperation(Summary = "DAL: Post a document to save in the database with title, author and uploaded file")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(DocumentItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.Author))
            {
                Console.WriteLine("DocumentDAL Bad Request");
                return BadRequest(new { message = "Document Information cannot be empty :/" });
            }
            await repository.AddAsync(item);
            return CreatedAtAction(nameof(GetAsyncById), new { id = item.Id }, new { id = item.Id });
        }

        [SwaggerOperation(Summary = "DAL: Update a specific document in the database with the ID of the document (not finished)")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] DocumentItem item)
        {

            Console.WriteLine($"PutAsync empfangene ID lautet: {id}");

            if (item == null)
            {
                Console.WriteLine("Payload is null");
                return BadRequest(new { message = "Invalid payload" });
            }

            var existingItem = await repository.GetByIdAsync(id, false);
            if (existingItem == null)
            {
                Console.WriteLine("Item to update not found");
                return NotFound();
            }

            // Map updated fields
            existingItem.Title = item.Title;
            existingItem.Author = item.Author;
            existingItem.OcrText = item.OcrText ?? existingItem.OcrText;
            //existingItem.DocumentContent = item.DocumentContent;
            //existingItem.DocumentMetadata = item.DocumentMetadata;
            await repository.UpdateAsync(existingItem);
            Console.WriteLine($"Successfully updated document with ID {id}");
            return NoContent();
        }

        [SwaggerOperation(Summary = "DAL: Delete a specific document from the database with the ID of the document")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var item = await repository.GetByIdAsync(id, false);
            if (item == null)
            {
                return NotFound();
            }

            await repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
