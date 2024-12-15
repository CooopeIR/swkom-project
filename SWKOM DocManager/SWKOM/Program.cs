using Elastic.Clients.Elasticsearch;
using FluentValidation;
using FluentValidation.AspNetCore;
using RabbitMQ.Client;
using SWKOM.BusinessLogic;
using SWKOM.Mappings;
using SWKOM.Services;
using SWKOM.Validators;


var builder = WebApplication.CreateBuilder(args);

var elasticUri = builder.Configuration.GetConnectionString("ElasticSearch") ?? "http://host.docker.internal:9200";
builder.Services.AddSingleton(new ElasticsearchClient(new Uri(elasticUri)));

builder.Services.AddScoped<IDocumentProcessor, DocumentProcessor>();

// Add services to the container.
builder.Services.AddControllers();

// Register RabbitMQ connection factory
builder.Services.AddSingleton<IConnectionFactory>(_ =>
    new ConnectionFactory
    {
        HostName = "rabbitmq",
        UserName = "user",
        Password = "password"
    }
);

builder.Services.AddSingleton<IMessageQueueService, MessageQueueService>();
builder.Services.AddHostedService<RabbitMqListenerService>();


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
            policy.WithOrigins("http://host.docker.internal") // Remove trailing slash
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
    client.BaseAddress = new Uri("http://host.docker.internal:8082");
    //client.BaseAddress = new Uri("http://localhost:8082");

    //client.DefaultRequestHeaders.Accept.Clear();
    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowWebUI");

//app.UseHttpsRedirection();

app.Urls.Add("http://*:8081"); // Stelle sicher, dass die App nur HTTP verwendet
app.UseAuthorization();


app.MapControllers();

app.Run();
