using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDAL.Entities;

namespace DocumentDAL.Repositories
{
    public interface IDocumentItemRepository
    {
        Task<IEnumerable<DocumentItem>> GetAllAsync();
        Task<DocumentItem> GetByIdAsync(int id, bool includeAll);
        Task<DocumentItem> AddAsync(DocumentItem item);
        Task UpdateAsync(DocumentItem item);
        Task DeleteAsync(int id);
    }
}
