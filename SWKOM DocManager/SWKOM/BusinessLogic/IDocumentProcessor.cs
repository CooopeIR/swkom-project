using SWKOM.DTO;

namespace SWKOM.BusinessLogic;

/// <summary>
/// Interface IDocumentProcessor for DocumentProcessor functions; functions to process uploaded document
/// </summary>
public interface IDocumentProcessor
{
    /// <summary>
    /// Interface: Process document function to get information (content, file name, content type, filesize uploaded date) out of uploaded document into single variables
    /// </summary>
    /// <param name="documentItemDTO"></param>
    /// <returns>Updated DocumentItemDTO element with information in single variables</returns>
    public Task<DocumentItemDTO> ProcessDocument(DocumentItemDTO documentItemDTO);
}