using SWKOM.DTO;

namespace SWKOM.BusinessLogic;

public interface IDocumentProcessor
{
    public Task<DocumentItemDTO> ProcessDocument(DocumentItemDTO documentItemDTO);
}