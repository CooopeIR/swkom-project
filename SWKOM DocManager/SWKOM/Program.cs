using FluentValidation;
using FluentValidation.AspNetCore;
using SWKOM.Mappings;
using SWKOM.Validators;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<DocumentItemDtoValidator>(); // register validators

// Add services to the container.
builder.Services.AddControllers();

//AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

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
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();


builder.Services.AddHttpClient("DocumentDAL", client =>
{
    client.BaseAddress = new Uri("http://host.docker.internal:8082");
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
