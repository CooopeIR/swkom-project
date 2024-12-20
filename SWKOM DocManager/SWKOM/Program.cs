using Elastic.Clients.Elasticsearch;
using FluentValidation;
using FluentValidation.AspNetCore;
using RabbitMQ.Client;
using SWKOM.Services;
using SWKOM.Mappings;
using SWKOM.Validators;


var builder = WebApplication.CreateBuilder(args);

var elasticUri = builder.Configuration.GetConnectionString("ElasticSearch") ?? "http://host.docker.internal:9200";

// Register ElasticsearchClient
builder.Services.AddSingleton<ElasticsearchClient>(sp =>
{
    var settings = new ElasticsearchClientSettings(new Uri(elasticUri))
        .DefaultIndex("documents");
    return new ElasticsearchClient(settings);
});

// Register RabbitMQ connection factory
builder.Services.AddSingleton<IConnectionFactory>(_ =>
    new ConnectionFactory
    {
        HostName = "rabbitmq",
        UserName = "user",
        Password = "password"
    }
);

// Register Services
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IDocumentProcessor, DocumentProcessor>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddSingleton<IMessageQueueService, MessageQueueService>();
builder.Services.AddHostedService<RabbitMqListenerService>();

// Add services to the container.
builder.Services.AddControllers();

//AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Fluent Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<DocumentItemDtoValidator>(); // register validators

// CORS konfigurieren, um Anfragen von localhost:80 (WebUI) zuzulassen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebUI",
        policy =>
        {
            policy.WithOrigins("http://webui") // Remove trailing slash
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddHttpClient();


builder.Services.AddHttpClient("DocumentDAL", client =>
{
    client.BaseAddress = new Uri("http://document_dal:8082");
    //client.BaseAddress = new Uri("http://host.docker.internal:8082");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowWebUI");

app.UseDefaultFiles();
app.UseStaticFiles();

app.Urls.Add("http://*:8081"); // Stelle sicher, dass die App nur HTTP verwendet
app.UseAuthorization();


app.MapControllers();

app.Run();