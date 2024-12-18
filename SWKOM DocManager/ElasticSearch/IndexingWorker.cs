using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using ElasticSearch.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace ElasticSearch;

/// <summary>
/// Class with IndexingWorker and associated functions
/// </summary>
public class IndexingWorker : IIndexingWorker
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ElasticsearchClient _elasticClient;
    private IConnection _connection;
    private IModel _channel;

    /// <summary>
    /// Constructor for IndexingWorker, get elasticClient and connectionFactory
    /// </summary>
    /// <param name="connectionFactory"></param>
    /// <param name="elasticClient"></param>
    public IndexingWorker(IConnectionFactory connectionFactory, ElasticsearchClient elasticClient)
    {
        _connectionFactory = connectionFactory;
        _elasticClient = elasticClient;
    }

    /// <summary>
    /// Tries to connect to RabbitMQ
    /// </summary>
    /// <exception cref="Exception">Konnte keine Verbindung zu RabbitMQ herstellen, alle Versuche fehlgeschlagen</exception>
    public void ConnectToRabbitMQ()
    {

        int retries = 15;
        while (retries > 0)
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                //Queue to read files for OCR work
                _channel.QueueDeclare(queue: "indexing_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                Console.WriteLine("Erfolgreich mit RabbitMQ verbunden und Index Queue erstellt.");

                break; // Wenn die Verbindung klappt, verlässt es die Schleife
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Verbinden mit RabbitMQ: {ex.Message}. Versuche es in 5 Sekunden erneut...");
                Thread.Sleep(5000);
                retries--;
            }
        }

        if (_connection == null || !_connection.IsOpen)
        {
            throw new Exception("Konnte keine Verbindung zu RabbitMQ herstellen, alle Versuche fehlgeschlagen.");
        }
    }

    /// <summary>
    /// Start ConnectToRabbitMQ function to start connection
    /// </summary>
    public void Initialize()
    {
        ConnectToRabbitMQ();
    }

    /// <summary>
    /// Get Channel from private _channel variable; To access the channel in tests
    /// </summary>
    /// <returns>IModel _channel</returns>
    public IModel GetChannel()
    {
        return _channel;
    }

    /// <summary>
    /// Tries to index the given document
    /// </summary>
    /// <param name="item">get Document item</param>
    public async Task IndexDocument(Document item)
    {
        Console.WriteLine($"Indexing Document {item.Id}");

        //await _elasticClient.IndexAsync(item, i => i.Index("documents"));

        //Console.WriteLine($"Indices: {_elasticClient.Indices}");

        var response = await _elasticClient.IndexAsync(item, i => i.Index("documents"));

        Console.WriteLine($"Result: {response.Result}, Index: {response.Index}, CallDwetailsd: {response.ApiCallDetails}, Success: {response.IsSuccess()}");


        if (response.IsValidResponse)
        {
            Console.WriteLine($"Index document with ID {response.Id} succeeded.");
        }
    }

    /// <summary>
    /// Wrapper for IndexDocument with additional needed preprocessing and exception catching
    /// </summary>
    public void Start()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            await Task.Run(async () =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var documentItem = JsonSerializer.Deserialize<Document>(message);
                    if (documentItem == null)
                        throw new Exception("Deserialization returned null. Message might be malformed.");

                    Console.WriteLine(
                        $"  [Indexing Worker]  Deserialized Document with Id {documentItem.Id} and Title {documentItem.Title}");

                    await IndexDocument(documentItem);

                    // Acknowledge the message on successful processing
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"Deserialization error: {jsonEx.Message}");
                    Console.WriteLine($"Raw Message: {Encoding.UTF8.GetString(ea.Body.ToArray())}");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false,
                        requeue: true); // Requeue on failure
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during indexing: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false,
                        requeue: true); // Requeue on failure
                }
            });
        };

        _channel.BasicConsume(queue: "indexing_queue", autoAck: false, consumer: consumer);
    }

    /// <summary>
    /// Dispose function to close channel and connection
    /// </summary>
    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}