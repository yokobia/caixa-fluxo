using CaixaFluxo.Application;
using CaixaFluxo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("LiberarAngularLocal", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Permite estritamente o seu Angular local
              .AllowAnyMethod()                     // Permite POST, GET, OPTIONS, etc.
              .AllowAnyHeader();                    // Permite cabeçalhos HTTP como Content-Type
    });
});

// Inversão de dependência
builder.Services.AddApplicationStructure();
builder.Services.AddInfrastructureStructure();

builder.Services.AddControllers();

var app = builder.Build();

// Ativa o middleware do CORS
app.UseCors("LiberarAngularLocal");

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