using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DocumentDAL.Repositories;
using DocumentDAL.Entities;

namespace DocumentDAL.Controllers
{
    [ApiController]
    [Route("api/documents")]
    public class DocumentController(IDocumentItemRepository repository) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<DocumentItem>> GetAsync()
        {
            return await repository.GetAllAsync();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(DocumentItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
            {
                return BadRequest(new { message = "Document Title cannot be empty :/" });
            }
            await repository.AddAsync(item);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, DocumentItem item)
        {
            var existingItem = await repository.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Title = item.Title;
            existingItem.Author = item.Author;
            existingItem.Content = item.Content;
            existingItem.Date = item.Date;
            await repository.UpdateAsync(existingItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
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
