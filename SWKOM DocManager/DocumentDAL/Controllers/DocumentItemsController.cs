using DocumentDAL.Entities;
using DocumentDAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace DocumentDAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController(IDocumentItemRepository repository) : ControllerBase
    {
        [HttpGet(Name = "GetAllDocumentsFromDB")]
        public async Task<IEnumerable<DocumentItem>> GetAsync()
        {
            return await repository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<DocumentItem> GetAsync(int id)
        {
            return await repository.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(DocumentItem item)
        {

            Console.WriteLine("PostAsync DocumentItem");
            Console.WriteLine($"DocumentItem: {item.Title}, {item.Author} --- DocumentContent: Length: {item.DocumentContent.Content.Length}, FileName: {item.DocumentContent.FileName} --- DocumentMetaData: Date. {item.DocumentMetadata.UploadDate}");
            if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.Author))
            {
                Console.WriteLine("DocumentDAL Bad Request");
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

            existingItem.Title = item.Title;
            existingItem.Author = item.Author;
            //existingItem.contentpath = item.contentpath;
            //existingItem.Date = item.Date;
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
