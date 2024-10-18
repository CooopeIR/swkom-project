using DocumentDAL.Entities;
using DocumentDAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace DocumentDAL.Controllers
{
    [ApiController]
    //[Route("api/documents")]
    [Route("api/[controller]")]
    public class DocumentController(IDocumentItemRepository repository) : ControllerBase
    {
        [HttpGet(Name = "GetAllDocumentsFromDB")]
        public async Task<IEnumerable<DocumentItem>> GetAsync()
        {
            return await repository.GetAllAsync();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(DocumentItem item)
        {
            if (string.IsNullOrWhiteSpace(item.title) || string.IsNullOrWhiteSpace(item.author))
            {
                return BadRequest(new { message = "Document Information cannot be empty :/" });
            }
            await repository.AddAsync(item);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, DocumentItem item)
        {
            var existingItem = await repository.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.title = item.title;
            existingItem.author = item.author;
            existingItem.contentpath = item.contentpath;
            existingItem.date = item.date;
            await repository.UpdateAsync(existingItem);
            return NoContent();
        }

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
