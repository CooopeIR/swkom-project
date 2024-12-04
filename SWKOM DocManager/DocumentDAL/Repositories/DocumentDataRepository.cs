using DocumentDAL.Data;
using DocumentDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentDAL.Repositories;

public class DocumentDataRepository(DocumentContext context) : IDocumentDataRepository
{
    public async Task<IEnumerable<DocumentMetadata>> GetAllMetaAsync()
    {
        return await context.DocumentMetadatas.ToListAsync();
    }

    public async Task<DocumentMetadata> GetMetaByIdAsync(int id)
    {
        return await context.DocumentMetadatas
                   .FirstOrDefaultAsync(di => di.DocumentId == id)
               ?? throw new Exception($"Document Metadata for Document with ID {id} not found");
    }

    public async Task<DocumentMetadata> AddMetaAsync(DocumentMetadata item)
    {
        await context.AddAsync(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateMetaAsync(DocumentMetadata item)
    {
        context.DocumentMetadatas!.Update(item);
        await context.SaveChangesAsync();
    }

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