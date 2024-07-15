using Elastic.Clients.Elasticsearch.IndexLifecycleManagement;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
namespace Core;

public static class ElasticseachExt
{
    public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
    {

        var userName = configuration.GetSection("Elastic")["Username"];
        var password = configuration.GetSection("Elastic")["Password"];

        var settings = new ElasticsearchClientSettings(new Uri(configuration.GetSection("Elastic")["Url"]!))
           .Authentication(new BasicAuthentication(userName!, password!));

        var client = new ElasticsearchClient(settings);

        services.AddSingleton(client);
    }
}
