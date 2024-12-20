using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using SWKOM.DTO;
using SearchRequest = SWKOM.DTO.SearchRequest;

namespace SWKOM.Services
{
    /// <summary>
    /// Interface for Search Service, making sure FuzzySearch and QueryStringSearch methods are implemented
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Method to FuzzySearch in ElasticSearch indexed documents
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        Task<SearchResponse<Document>> FuzzySearchAsync(SearchRequest searchTerm);
        /// <summary>
        /// Method to Exact Match Search in ElasticSearch indexed documents
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        Task<SearchResponse<Document>> QueryStringSearchAsync(SearchRequest searchTerm);
    }
}
