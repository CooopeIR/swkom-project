using Elastic.Clients.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var elasticUri = builder.Configuration.GetConnectionString("ElasticSearch") ?? "http://localhost:9200";
builder.Services.AddSingleton(new ElasticsearchClient(new Uri(elasticUri)));

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger";
});


app.Urls.Add("http://*:8080");

app.UseAuthorization();

app.MapControllers();

app.Run();