using DocumentDAL.Entities;
using DocumentDAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Swashbuckle.AspNetCore.Annotations;

// Class to take care of underlying documentContent

namespace DocumentDAL.Controllers
{
    /// <summary>
    /// DAL: Class with GET all, GET by ID, POST, PUT, DELETE methods for document content elements for the database
    /// </summary>
    /// <param name="repository"></param>
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentContentController(IDocumentContentRepository repository) : ControllerBase
    {
        /// <summary>
        /// DAL: Get all document contents from the database
        /// </summary>
        /// <returns>List of all document contents from Database</returns>
        [SwaggerOperation(Summary = "DAL: Get all document contents from the database")]
        [HttpGet(Name = "GetDocumentContents")]
        public async Task<IEnumerable<DocumentContent>> GetAllAsync()
        {
            return await repository.GetAllContentAsync();
        }

        /// <summary>
        /// DAL: Get a specific document content from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Document content from Database with specified id</returns>
        [SwaggerOperation(Summary = "DAL: Get a specific document contents from the database with the ID of the document")]
        [HttpGet("{id}", Name = "GetDocumentContentsById"),]
        public async Task<DocumentContent> GetAsyncById(int id)
        {
            return await repository.GetContentByIdAsync(id);
        }

        /// <summary>
        /// DAL: Post a document content to save in the database with title, author and uploaded file
        /// </summary>
        /// <param name="item"></param>
        /// <returns>CreatedAtAction status code with assigned database id or error status code</returns>
        [SwaggerOperation(Summary = "DAL: Post a document content to save in the database with title, author and uploaded file")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(DocumentContent item)
        {
            if (string.IsNullOrWhiteSpace(item.ContentType) || string.IsNullOrWhiteSpace(item.FileName))
            {
                Console.WriteLine("DocumentDAL Bad Request");
                return BadRequest(new { message = "Document ContentType / Filename cannot be empty :/" });
            }
            await repository.AddContentAsync(item);
            return CreatedAtAction(nameof(GetAsyncById), new { id = item.Id }, new { id = item.Id });
        }

        /// <summary>
        /// DAL: Update a specific document content in the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns>Success Status code with no content or error status code</returns>
        [SwaggerOperation(Summary = "DAL: Update a specific document content in the database with the ID of the document")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] DocumentContent item)
        {
            Console.WriteLine($"PutAsync empfangene ID lautet: {id}");

            if (item == null)
            {
                Console.WriteLine("Payload is null");
                return BadRequest(new { message = "Invalid payload" });
            }

            var existingItem = await repository.GetContentByIdAsync(id);
            if (existingItem == null)
            {
                Console.WriteLine("Item to update not found");
                return NotFound();
            }

            // Map updated fields
            existingItem.ContentType = item.ContentType ?? existingItem.ContentType;
            existingItem.FileName = item.FileName ?? existingItem.FileName;
            existingItem.Content = item.Content ?? existingItem.Content;
            await repository.UpdateContentAsync(existingItem);
            Console.WriteLine($"Successfully updated contents of document with ID {id}");
            return NoContent();
        }

        /// <summary>
        /// DAL: Delete a specific document from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status code with no content or error status code</returns>
        [SwaggerOperation(Summary = "DAL: Delete a specific document from the database with the ID of the document")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var item = await repository.GetContentByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await repository.DeleteContentAsync(id);
            return NoContent();
        }
    }
}
