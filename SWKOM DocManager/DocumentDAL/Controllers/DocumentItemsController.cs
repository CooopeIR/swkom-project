using DocumentDAL.Entities;
using DocumentDAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DocumentDAL.Controllers
{
    /// <summary>
    /// DAL: Class with GET all, GET by ID, POST, PUT, DELETE methods for document item elements for the database
    /// </summary>
    /// <param name="repository"></param>
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController(IDocumentItemRepository repository) : ControllerBase
    {
        /// <summary>
        /// DAL: Get all document items from the database
        /// </summary>
        /// <returns>List of all document items from database</returns>
        [SwaggerOperation(Summary = "DAL: Get all document items from the database")]
        [HttpGet(Name = "GetAllDocumentsFromDB")]
        public async Task<IEnumerable<DocumentItem>> GetAllAsync()
        {
            return await repository.GetAllAsync();
        }

        /// <summary>
        /// DAL: Get a specific document item from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Document item from Database with specified id</returns>
        [SwaggerOperation(Summary = "DAL: Get a specific document item from the database with the ID of the document")]
        [HttpGet("{id}", Name = "GetDocumentById"),]
        public async Task<DocumentItem> GetAsyncById(int id)
        {
            return await repository.GetByIdAsync(id);
        }

        /// <summary>
        /// DAL: Post a document item to save in the database
        /// </summary>
        /// <param name="item"></param>
        /// <returns>CreatedAtAction status code with assigned database id or error status code</returns>
        [SwaggerOperation(Summary = "DAL: Post a document item to save in the database")]
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

        /// <summary>
        /// DAL: Update a specific document item in the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns>Success Status code with no content or error status code</returns>
        [SwaggerOperation(Summary = "DAL: Update a specific document item in the database with the ID of the document")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] DocumentItem item)
        {

            Console.WriteLine($"PutAsync empfangene ID lautet: {id}");

            if (item == null)
            {
                Console.WriteLine("Payload is null");
                return BadRequest(new { message = "Invalid payload" });
            }

            var existingItem = await repository.GetByIdAsync(id);
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

        /// <summary>
        /// DAL: Delete a specific document item from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status code with no content or error status code</returns>
        [SwaggerOperation(Summary = "DAL: Delete a specific document item from the database with the ID of the document")]
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

        /// <summary>
        /// DAL: Delete a specific document item from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status code with no content or error status code</returns>
        [SwaggerOperation(Summary =
            "DAL: Delete a specific document item from the database with the ID of the document")]
        [HttpGet("view/{id}")]
        public async Task<IActionResult> ViewDocumentAsync(int id)
        {
            var item = await repository.GetFullDocumentAsync(id);
            return Ok(item);
        }
    }
}
