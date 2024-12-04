using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDAL.Entities;

namespace DocumentDAL.Repositories
{
    public interface IDocumentContentRepository
    {
        Task<IEnumerable<DocumentContent>> GetAllContentAsync();
        Task<DocumentContent> GetContentByIdAsync(int id);
        Task<DocumentContent> AddContentAsync(DocumentContent item);
        Task UpdateContentAsync(DocumentContent item);
        Task DeleteContentAsync(int id);
    }
}
