using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDAL.Entities;

namespace DocumentDAL.Repositories
{
    public interface IDocumentDataRepository
    {
        Task<IEnumerable<DocumentMetadata>> GetAllMetaAsync();
        Task<DocumentMetadata> GetMetaByIdAsync(int id);
        Task<DocumentMetadata> AddMetaAsync(DocumentMetadata item);
        Task UpdateMetaAsync(DocumentMetadata item);
        Task DeleteMetaAsync(int id);
    }
}
