using DocumentDAL.Data;
using DocumentDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentDAL.Repositories;

/// <summary>
/// DocumentContentRepository methods for DocumentContent elements on database (get all, get by ID, add new element, update element with ID, delete element with ID)
/// </summary>
/// <param name="context"></param>
public class DocumentContentRepository(DocumentContext context) : IDocumentContentRepository
{
    /// <summary>
    /// Database call to get all DocumentContent elements from database
    /// </summary>
    /// <returns>List of DocumentContents</returns>
    public async Task<IEnumerable<DocumentContent>> GetAllContentAsync()
    {
        return await context.DocumentContents.ToListAsync();
    }

    /// <summary>
    /// Database call to get DocumentContent with specific ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>DocumentContent of specific ID</returns>
    /// <exception cref="Exception">Document Contents for Document with ID not found</exception>
    public async Task<DocumentContent> GetContentByIdAsync(int id)
    {
        return await context.DocumentContents
                   .FirstOrDefaultAsync(di => di.DocumentId == id)
               ?? throw new Exception($"Document Contents for Document with ID {id} not found");
    }

    /// <summary>
    /// Database call to save a new DocumentContent element in database
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Added item</returns>
    public async Task<DocumentContent> AddContentAsync(DocumentContent item)
    {
        await context.AddAsync(item);
        await context.SaveChangesAsync();
        return item;
    }

    /// <summary>
    /// Database call to update DocumentContent in database with given DocumentContent item
    /// </summary>
    /// <param name="item">Type: DocumentContent</param>
    public async Task UpdateContentAsync(DocumentContent item)
    {
        context.DocumentContents!.Update(item);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Database call to delete specific DocumentContent element from database with its id
    /// </summary>
    /// <param name="id"></param>
    public async Task DeleteContentAsync(int id)
    {
        var item = await context.DocumentContents.FindAsync(id);
        if (item != null)
        {
            context.DocumentContents.Remove(item);
            await context.SaveChangesAsync();
        }
    }
}