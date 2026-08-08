using CaixaFluxo.Application;
using CaixaFluxo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Inversão de dependência
builder.Services.AddApplicationStructure();
builder.Services.AddInfrastructureStructure();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

// Mapeia os controllers da API
app.MapControllers();

// Endpoint de Health Check estruturado
app.MapGet("/health", () => Results.Ok(new { 
    Status = "Healthy", 
    Runtime = ".NET 8",
    Timestamp = DateTime.UtcNow 
}));

app.Run();