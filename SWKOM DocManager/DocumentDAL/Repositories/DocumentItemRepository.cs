using DocumentDAL.Data;
using DocumentDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentDAL.Repositories
{
    /// <summary>
    /// DocumentItemRepository methods for DocumentItem elements on database (get all, get by ID, add new element, update element with ID, delete element with ID)
    /// </summary>
    /// <param name="context"></param>
    public class DocumentItemRepository(DocumentContext context) : IDocumentItemRepository
    {
        /// <summary>
        /// Database call to get all DocumentItem elements from database
        /// </summary>
        /// <returns>List of DocumentItem</returns>
        public async Task<IEnumerable<DocumentItem>> GetAllAsync()
        {
            return await context.DocumentItems.ToListAsync();
        }

        /// <summary>
        /// Database call to get DocumentItem with specific ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeAll">type: bool</param>
        /// <returns>if includeAll = true: additionally include DocumentContent and DocumentMetadata</returns>
        /// <exception cref="Exception">DocumentItem with ID not found</exception>
        public async Task<DocumentItem> GetByIdAsync(int id)
        {
            return await context.DocumentItems
                       .FirstOrDefaultAsync(di => di.Id == id)
                   ?? throw new Exception($"DocumentItem with ID {id} not found");
        }

        /// <summary>
        /// Database call to save a new DocumentItem element in database
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Added item</returns>
        public async Task<DocumentItem> AddAsync(DocumentItem item)
        {
            await context.AddAsync(item);
            await context.SaveChangesAsync();
            return item;
        }

        /// <summary>
        /// Database call to update DocumentItem in database with given DocumentItem item
        /// </summary>
        /// <param name="item">Type: DocumentItem</param>
        public async Task UpdateAsync(DocumentItem item)
        {
            context.DocumentItems!.Update(item);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Database call to delete specific DocumentItem element from database with its id
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteAsync(int id)
        {
            var item = await context.DocumentItems.FindAsync(id);
            if (item != null)
            {
                context.DocumentItems.Remove(item);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Fetches a Document including all of its subtypes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception">DocumentItem with ID {id} not found</exception>
        public async Task<DocumentItem> GetFullDocumentAsync(int id)
        {
            var documentItem = await context.DocumentItems
                .Include(d => d.DocumentContent)    // Include DocumentContent
                .Include(d => d.DocumentMetadata)   // Include DocumentMetadata
                .Where(d => d.Id == id)     // Filter by Id
                .FirstOrDefaultAsync(); // Fetch the first match (or null if not found)

            if (documentItem == null)
                throw new Exception($"DocumentItem with ID {id} not found");

            return documentItem;
        }
    }
}
