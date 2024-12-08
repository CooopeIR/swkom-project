using DocumentDAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentDAL.Repositories
{
    /// <summary>
    /// Interface IDocumentItemRepository for DocumentItemRepository; 
    /// DocumentItemRepository methods for DocumentItem elements on database (get all, get by ID, add new element, update element with ID, delete element with ID)
    /// </summary>
    public interface IDocumentItemRepository
    {
        /// <summary>
        /// Interface: Database call to get all DocumentItem elements from database
        /// </summary>
        /// <returns>List of DocumentItem</returns>
        Task<IEnumerable<DocumentItem>> GetAllAsync();

        /// <summary>
        /// Interface: Database call to get DocumentItem with specific ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeAll">type: bool</param>
        /// <returns>if includeAll = true: additionally include DocumentContent and DocumentMetadata</returns>
        /// <exception cref="Exception">DocumentItem with ID not found</exception>
        Task<DocumentItem> GetByIdAsync(int id, bool includeAll);

        /// <summary>
        /// Interface: Database call to save a new DocumentItem element in database
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Added item</returns>
        Task<DocumentItem> AddAsync(DocumentItem item);

        /// <summary>
        /// Interface: Database call to update DocumentItem in database with given DocumentItem item
        /// </summary>
        /// <param name="item">Type: DocumentItem</param>
        Task UpdateAsync(DocumentItem item);

        /// <summary>
        /// Interface: Database call to delete specific DocumentItem element from database with its id 
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(int id);
    }
}
