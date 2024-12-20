using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Mvc;
using SWKOM.DTO;
using SearchRequest = SWKOM.DTO.SearchRequest;
namespace SWKOM.Services;

/// <summary>
/// Service to Search for documents in ElasticSearch Microservice
/// </summary>
public class SearchService : ISearchService
{
    private readonly ElasticsearchClient _searchClient;

    /// <summary>
    /// Constructor for SearchService assigning searchClient from Dependency Injection
    /// </summary>
    /// <param name="searchClient"></param>
    public SearchService(ElasticsearchClient searchClient)
    {
        _searchClient = searchClient;
    }

    /// <summary>
    /// Fuzzy Searching through all documents indexed in ElasticSearch
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<SearchResponse<Document>> FuzzySearchAsync(SearchRequest request)
    {
        await EnsureDocumentIndex();

        // Access the properties from the request object
        var searchTerm = request.SearchTerm;
        var includeOcr = request.IncludeOcr;

        var fields = new Field[]
        {
            "title^3"!,
            "author^2"!
        }.Concat(includeOcr ? ["ocrText"!] : Array.Empty<Field>()).ToArray();

        var response = await _searchClient.SearchAsync<Document>(search => search
            .Index("documents")
            .Query(q => q
                .MultiMatch(mm => mm
                    .Query(searchTerm)
                    .Fields(fields)
                    .Fuzziness(new Fuzziness(2))
                    .Type(TextQueryType.BestFields)
                )
            )
            .Size(10)
        );

        return response;
    }

    /// <summary>
    /// Exact Match Searching through all documents indexed in ElasticSearch
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<SearchResponse<Document>> QueryStringSearchAsync(SearchRequest request)
    {
        await EnsureDocumentIndex();

        // Access the properties from the request object
        var searchTerm = request.SearchTerm;
        var includeOcr = request.IncludeOcr;

        var fields = new Field[]
        {
            "title"!,
            "author"!
        }.Concat(includeOcr ? ["ocrText"!] : Array.Empty<Field>()).ToArray();

        var response = await _searchClient.SearchAsync<Document>(s => s
            .Index("documents")
            .Query(q => q
                .QueryString(qs => qs
                    .Query($"*{searchTerm}*")
                    .Fields(fields)
                )
            )
        );

        return response;
    }
    
    private async Task EnsureDocumentIndex()
    {
        var indexExistsResponse = await _searchClient.Indices.ExistsAsync("documents");

        if (!indexExistsResponse.Exists)
        {
            var createIndexResponse = await _searchClient.Indices.CreateAsync<Document>("documents", c => c
                .Mappings(m => m
                    .Properties(p => p
                        .LongNumber(t => t.Id)
                        .Text(t => t.Title)
                        .Text(t => t.Author)
                        .Text(t => t.OcrText ?? string.Empty)
                    )
                )
            );

            if (!createIndexResponse.IsValidResponse)
            {
                // Handle error in index creation
                Console.WriteLine("Index creation failed: " + createIndexResponse.DebugInformation);
            }
        }
    }
}