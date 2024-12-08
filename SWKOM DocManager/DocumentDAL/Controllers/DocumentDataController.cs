using DocumentDAL.Entities;
using DocumentDAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Swashbuckle.AspNetCore.Annotations;

// Class to take care of underlying documentData (MetaData and Contents)

namespace DocumentDAL.Controllers
{
    /// <summary>
    /// DAL: Class with GET all, GET by ID, POST, PUT, DELETE methods for document metadata elements for the database
    /// </summary>
    /// <param name="repository"></param>
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentDataController(IDocumentDataRepository repository) : ControllerBase
    {
        /// <summary>
        /// DAL: Get all document Metadatas from the database
        /// </summary>
        /// <returns>List of all document Metadatas from Database</returns>
        [SwaggerOperation(Summary = "DAL: Get all Document Metadatas from the database")]
        [HttpGet(Name = "GetAllMetadata")]
        public async Task<IEnumerable<DocumentMetadata>> GetAllAsync()
        {
            return await repository.GetAllMetaAsync();
        }

        /// <summary>
        /// DAL: Get a specific document Metadata from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Document Metadata from Database with specified id</returns>
        [SwaggerOperation(Summary = "DAL: Get a specific document Metadata from the database with the ID of the document")]
        [HttpGet("{id}", Name = "GetMetadataById"),]
        public async Task<DocumentMetadata> GetAsyncById(int id)
        {
            return await repository.GetMetaByIdAsync(id);
        }

        /// <summary>
        /// DAL: Post a document Metadata to save in the database
        /// </summary>
        /// <param name="item"></param>
        /// <returns>CreatedAtAction status code with assigned database id or error status code</returns>
        [SwaggerOperation(Summary = "DAL: Post a document Metadata to save in the database")]
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

        /// <summary>
        /// DAL: Update a specific document Metadata in the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns>Success Status code with no content or error status code</returns>
        [SwaggerOperation(Summary = "DAL: Update a specific document Metadata in the database with the ID of the document")]
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

        /// <summary>
        /// DAL: Delete a specific document Metadata from the database with the ID of the document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status code with no content or error status code</returns>
        [SwaggerOperation(Summary = "DAL: Delete a specific document Metadata from the database with the ID of the document")]
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
