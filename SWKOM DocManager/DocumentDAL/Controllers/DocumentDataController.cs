using DocumentDAL.Entities;
using DocumentDAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Swashbuckle.AspNetCore.Annotations;

// Class to take care of underlying documentData (MetaData and Contents)

namespace DocumentDAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentDataController(IDocumentDataRepository repository) : ControllerBase
    {
        [SwaggerOperation(Summary = "DAL: Get all documents from the database")]
        [HttpGet(Name = "GetAllMetadata")]
        public async Task<IEnumerable<DocumentMetadata>> GetAllAsync()
        {
            return await repository.GetAllMetaAsync();
        }

        [SwaggerOperation(Summary = "DAL: Get a specific document from the database with the ID of the document")]
        [HttpGet("{id}", Name= "GetMetadataById"), ]
        public async Task<DocumentMetadata> GetAsyncById(int id)
        {
            return await repository.GetMetaByIdAsync(id);
        }

        [SwaggerOperation(Summary = "DAL: Post a document to save in the database with title, author and uploaded file")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(DocumentMetadata item)
        {
            
            if (item.FileSize == null || item.UploadDate == null)
            {
                Console.WriteLine("DocumentDAL Bad Request");
                return BadRequest(new { message = "Document Information cannot be empty :/" });
            }
            await repository.AddMetaAsync(item);
            return CreatedAtAction(nameof(GetAsyncById), new { id = item.Id }, new { id = item.Id });
        }

        [SwaggerOperation(Summary = "DAL: Update a specific document in the database with the ID of the document (not finished)")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] DocumentMetadata item)
        {
            Console.WriteLine($"PutAsync empfangene ID lautet: {id}");

            if (item == null)
            {
                Console.WriteLine("Payload is null");
                return BadRequest(new { message = "Invalid payload" });
            }

            var existingItem = await repository.GetMetaByIdAsync(id);
            if (existingItem == null)
            {
                Console.WriteLine("Item to update not found");
                return NotFound();
            }

            // Map updated fields
            existingItem.FileSize = item.FileSize ?? existingItem.FileSize;
            //existingItem.UploadDate = item.UploadDate ?? existingItem.UploadDate;

            await repository.UpdateMetaAsync(existingItem);
            Console.WriteLine($"Successfully updated Metadata of document with ID {id}");
            return NoContent();
        }

        [SwaggerOperation(Summary = "DAL: Delete a specific document from the database with the ID of the document")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var item = await repository.GetMetaByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await repository.DeleteMetaAsync(id);
            return NoContent();
        }
    }
}
