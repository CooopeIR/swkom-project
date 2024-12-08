using DocumentDAL.Data;
using DocumentDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentDAL.Repositories;

/// <summary>
/// DocumentDataRepository methods for DocumentMetadata elements on database (get all, get by ID, add new element, update element with ID, delete element with ID)
/// </summary>
/// <param name="context"></param>
public class DocumentDataRepository(DocumentContext context) : IDocumentDataRepository
{
    /// <summary>
    /// Database call to get all DocumentMetadata elements from database
    /// </summary>
    /// <returns>List of DocumentMetadata</returns>
    public async Task<IEnumerable<DocumentMetadata>> GetAllMetaAsync()
    {
        return await context.DocumentMetadatas.ToListAsync();
    }

    /// <summary>
    /// Database call to get DocumentMetadata with specific ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>DocumentMetadata of specific ID</returns>
    /// <exception cref="Exception">Document Metadata for Document with ID not found</exception>
    public async Task<DocumentMetadata> GetMetaByIdAsync(int id)
    {
        return await context.DocumentMetadatas
                   .FirstOrDefaultAsync(di => di.DocumentId == id)
               ?? throw new Exception($"Document Metadata for Document with ID {id} not found");
    }

    /// <summary>
    /// Database call to save a new DocumentMetadata element in database
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Added item</returns>
    public async Task<DocumentMetadata> AddMetaAsync(DocumentMetadata item)
    {
        await context.AddAsync(item);
        await context.SaveChangesAsync();
        return item;
    }

    /// <summary>
    /// Database call to update DocumentMetadata in database with given DocumentMetadata item
    /// </summary>
    /// <param name="item">Type: DocumentContent</param>
    public async Task UpdateMetaAsync(DocumentMetadata item)
    {
        context.DocumentMetadatas!.Update(item);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Database call to delete specific DocumentMetadata element from database with its id
    /// </summary>
    /// <param name="id"></param>
    public async Task DeleteMetaAsync(int id)
    {
        var item = await context.DocumentMetadatas.FindAsync(id);
        if (item != null)
        {
            context.DocumentMetadatas.Remove(item);
            await context.SaveChangesAsync();
        }
    }
}