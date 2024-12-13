using Elastic.Clients.Elasticsearch;
using ElasticSearch;
using RabbitMQ.Client;

namespace ElasticSearch
{
    class Program()
    {
        static void Main(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load from appsettings.json
                .AddEnvironmentVariables()                                            // Load from environment variables
                .Build();

            // Retrieve the ElasticSearch connection string
            var elasticUri = configuration.GetConnectionString("ElasticSearch") ?? "http://localhost:9200";

            // Set up the service collection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConnectionFactory>(_ =>
                    new ConnectionFactory
                    {
                        HostName = "rabbitmq",
                        UserName = "user",
                        Password = "password"
                    })
                .AddSingleton(new ElasticsearchClient(new Uri(elasticUri)))            // Register ElasticsearchClient
                .AddSingleton<IndexingWorker>()                                        // Register IndexingWorker
                .BuildServiceProvider();

            // Resolve IndexingWorker from the DI container
            var worker = serviceProvider.GetService<IndexingWorker>();
            if (worker == null)
            {
                Console.WriteLine("Failed to resolve IndexingWorker.");
                return;
            }

            worker.Initialize();
            worker.Start();

            Console.WriteLine("Indexing Worker is running. Press Ctrl+C to exit.");
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}

