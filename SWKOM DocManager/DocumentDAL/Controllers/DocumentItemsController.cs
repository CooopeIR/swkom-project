using DocumentDAL.Entities;
using DocumentDAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DocumentDAL.Controllers
{
    [ApiController]
    //[Route("api/documents")]
    [Route("")]
    public class DocumentController(IDocumentItemRepository repository) : ControllerBase
    {
        /*[HttpGet(Name = "GetAllDocuments")]
        public ActionResult<IEnumerable<DocumentItem>> Get()
        {

            var documents = Enumerable.Range(1, 5).Select(index => new DocumentItem
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Title = "Title 1",
                Author = "Me",
                Content = "Line 1.",
                Id = Guid.NewGuid()
            })
            .ToArray();

            return Ok(documents);
        }*/
        [HttpGet(Name = "GetAllDocuments")]
        public async Task<IEnumerable<DocumentItem>> GetAsync()
        {
            return await repository.GetAllAsync();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(DocumentItem item)
        {
            if (string.IsNullOrWhiteSpace(item.title))
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

            existingItem.title = item.title;
            existingItem.author = item.author;
            existingItem.contentpath = item.contentpath;
            existingItem.date = item.date;
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
