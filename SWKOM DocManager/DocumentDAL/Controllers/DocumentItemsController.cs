using DocumentDAL.Entities;
using DocumentDAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Swashbuckle.AspNetCore.Annotations;

namespace DocumentDAL.Controllers
{
    [ApiController]
    //[Route("api/document")]
    [Route("api/[controller]")]
    public class DocumentController(IDocumentItemRepository repository) : ControllerBase
    {
        [SwaggerOperation(Summary = "DAL: Get all documents from the database")]
        [HttpGet(Name = "GetAllDocumentsFromDB")]
        public async Task<IEnumerable<DocumentItem>> GetAsync()
        {
            return await repository.GetAllAsync();
        }

        [SwaggerOperation(Summary = "DAL: Get a specific document from the database with the ID of the document")]
        [HttpGet("{id}")]
        public async Task<DocumentItem> GetAsync(int id)
        {
            return await repository.GetByIdAsync(id);
        }

        [SwaggerOperation(Summary = "DAL: Post a document to save in the database with title, author and updladed file")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(DocumentItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.Author))
            {
                return BadRequest(new { message = "Document Information cannot be empty :/" });
            }
            await repository.AddAsync(item);
            return Ok();
        }

        [SwaggerOperation(Summary = "DAL: Update a specific document in the database with the ID of the document (not finished)")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, DocumentItem item)
        {
            var existingItem = await repository.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Title = item.Title;
            existingItem.Author = item.Author;
            //existingItem.contentpath = item.contentpath;
            //existingItem.Date = item.Date;
            await repository.UpdateAsync(existingItem);
            return NoContent();
        }

        [SwaggerOperation(Summary = "DAL: Delete a specific document from the database with the ID of the document")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
