using DocumentDAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentDAL.Repositories
{
    /// <summary>
    /// Interface IDocumentContentRepository for DocumentContentRepository; 
    /// DocumentContentRepository methods for DocumentContent elements on database (get all, get by ID, add new element, update element with ID, delete element with ID)
    /// </summary>
    public interface IDocumentContentRepository
    {
        /// <summary>
        /// Interface: Database call to get all DocumentContent elements from database
        /// </summary>
        /// <returns>List of DocumentContents</returns>
        Task<IEnumerable<DocumentContent>> GetAllContentAsync();

        /// <summary>
        /// Interface: Database call to get DocumentContent with specific ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DocumentContent of specific ID</returns>
        /// <exception cref="Exception">Document Contents for Document with ID not found</exception>
        Task<DocumentContent> GetContentByIdAsync(int id);

        /// <summary>
        /// Interface: Database call to save a new DocumentContent element in database
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Added item</returns>
        Task<DocumentContent> AddContentAsync(DocumentContent item);

        /// <summary>
        /// Interface: Database call to update DocumentContent in database with given DocumentContent item
        /// </summary>
        /// <param name="item">Type: DocumentContent</param>
        Task UpdateContentAsync(DocumentContent item);

        /// <summary>
        /// Interface: Database call to delete specific DocumentContent element from database with its id
        /// </summary>
        /// <param name="id"></param>
        Task DeleteContentAsync(int id);
    }
}
