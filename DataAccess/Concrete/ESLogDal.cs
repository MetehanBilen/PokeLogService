using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.Nodes;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete;

public class ESLogDal
{
    private const string IndexName = "logs_001";
    private readonly ElasticsearchClient elasticsearchClient;

    public ESLogDal(ElasticsearchClient elasticsearchClient)
    {
        this.elasticsearchClient = elasticsearchClient;
    }

    public async Task<LogModel> SaveAsync(LogModel logModel)
    {
        var response = await elasticsearchClient.IndexAsync(logModel,x => x.Index(IndexName));
        if (!response.IsValidResponse) return null;

        return logModel;
    }

    public async Task<ImmutableList<LogModel>> GetAllAsync()
    {
        var searchResponse = await elasticsearchClient.SearchAsync<LogModel>(s => s.Index(IndexName).Query(q => q.MatchAll( m => { })));
        
        foreach (var hit in searchResponse.Hits) hit.Source.Id = hit.Id; //id source taşıma


        return searchResponse.Documents.ToImmutableList();
    }

}

