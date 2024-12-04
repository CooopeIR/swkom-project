using DocumentDAL.Data;
using DocumentDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentDAL.Repositories
{
    public class DocumentItemRepository(DocumentContext context) : IDocumentItemRepository
    {

        public async Task<IEnumerable<DocumentItem>> GetAllAsync()
        {
            return await context.DocumentItems.ToListAsync();
        }

        public async Task<DocumentItem> GetByIdAsync(int id, bool includeAll)
        {
            switch (includeAll)
            {
                case true:
                    return await context.DocumentItems!
                               .Include(di => di.DocumentContent)
                               .Include(di => di.DocumentMetadata)
                               .FirstOrDefaultAsync(di => di.Id == id)
                           ?? throw new Exception($"DocumentItem with ID {id} not found");
                default:
                    return await context.DocumentItems
                               .FirstOrDefaultAsync(di => di.Id == id)
                           ?? throw new Exception($"DocumentItem with ID {id} not found");
            }
        }

        public async Task<DocumentItem> AddAsync(DocumentItem item)
        {
            await context.AddAsync(item);
            await context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateAsync(DocumentItem item)
        {
            context.DocumentItems!.Update(item);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await context.DocumentItems.FindAsync(id);
            if (item != null)
            {
                context.DocumentItems.Remove(item);
                await context.SaveChangesAsync();
            }
        }
    }
}
