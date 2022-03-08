using WebApi.Startups.Application;
using WebApi.Startups.Infrastructure.Logging;
using WebApi.Startups.Infrastructure.Persistence;
using WebApi.Startups.Presentation;
using WebApi.Startups.Presentation.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Log
builder.Host.AddMySerilogLogging();

// Add services
builder.Services.AddMyApplicationDependencies();
builder.Services.AddMyPersistence(builder.Environment);
builder.Services.AddMyVersioning();
builder.Services.AddMySwagger(builder.Configuration);
builder.Services.AddMyRestApi();

// Startup App
var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.UseRouting();
app.UseApiVersioning();
app.UseAuthorization();
app.UseMySwagger(builder.Configuration);
app.UseMyRequestLogging();
app.UseMyRestApi(builder.Environment);

app.Run();
