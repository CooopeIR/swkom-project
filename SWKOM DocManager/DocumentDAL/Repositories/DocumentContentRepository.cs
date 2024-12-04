using DocumentDAL.Data;
using DocumentDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentDAL.Repositories;

public class DocumentContentRepository(DocumentContext context) : IDocumentContentRepository
{
    public async Task<IEnumerable<DocumentContent>> GetAllContentAsync()
    {
        return await context.DocumentContents.ToListAsync();
    }

    public async Task<DocumentContent> GetContentByIdAsync(int id)
    {
        return await context.DocumentContents
                   .FirstOrDefaultAsync(di => di.DocumentId == id)
               ?? throw new Exception($"Document Contents for Document with ID {id} not found");
    }

    public async Task<DocumentContent> AddContentAsync(DocumentContent item)
    {
        await context.AddAsync(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateContentAsync(DocumentContent item)
    {
        context.DocumentContents!.Update(item);
        await context.SaveChangesAsync();
    }

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