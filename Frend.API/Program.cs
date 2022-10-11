using Frend.API.Helpers;
using Frend.API.Services;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region "Configure Services"
// Add services to the container.

// Add the memory cache & response compression services.
builder.Services.AddMemoryCache();
builder.Services.AddResponseCompression();

builder.Services.AddScoped<IFrendAPIService, FrendAPIService>();
builder.Services.AddScoped<HttpClientHelper>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Frend.API v1.0",
        Description = ".NET 6.0 Web Api",
    });
});
#endregion

#region "Configure Application (HTTP Request pipeline)"
var app = builder.Build();

// Configure the HTTP request pipeline.

// Configure Cors (And Middleware)
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod());
app.UseCorsMiddleware();

app.UseResponseCompression();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.Run();
#endregion
