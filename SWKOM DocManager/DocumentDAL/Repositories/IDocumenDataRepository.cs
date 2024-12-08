using DocumentDAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentDAL.Repositories
{
    /// <summary>
    /// Interface IDocumentDataRepository for DocumentDataRepository; 
    /// DocumentDataRepository methods for DocumentMetadata elements on database (get all, get by ID, add new element, update element with ID, delete element with ID)
    /// </summary>
    public interface IDocumentDataRepository
    {
        /// <summary>
        /// Interface: Database call to get all DocumentMetadata elements from database
        /// </summary>
        /// <returns>List of DocumentMetadata</returns>
        Task<IEnumerable<DocumentMetadata>> GetAllMetaAsync();

        /// <summary>
        /// Interface: Database call to get DocumentMetadata with specific ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DocumentMetadata of specific ID</returns>
        /// <exception cref="Exception">Document Metadata for Document with ID not found</exception>
        Task<DocumentMetadata> GetMetaByIdAsync(int id);

        /// <summary>
        /// Interface: Database call to save a new DocumentMetadata element in database
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Added item</returns>
        Task<DocumentMetadata> AddMetaAsync(DocumentMetadata item);

        /// <summary>
        /// Interface: Database call to update DocumentMetadata in database with given DocumentMetadata item
        /// </summary>
        /// <param name="item">Type: DocumentContent</param>
        Task UpdateMetaAsync(DocumentMetadata item);

        /// <summary>
        /// Interface: Database call to delete specific DocumentMetadata element from database with its id
        /// </summary>
        /// <param name="id"></param>
        Task DeleteMetaAsync(int id);
    }
}
