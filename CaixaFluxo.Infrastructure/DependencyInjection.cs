using Microsoft.Extensions.DependencyInjection;
using CaixaFluxo.Application.Interfaces;
using CaixaFluxo.Infrastructure.Data;
using CaixaFluxo.Infrastructure.Repositories;

namespace CaixaFluxo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureStructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryDbContext>();
        services.AddScoped<ILancamentoRepository, LancamentoRepository>();

        return services;
    }
}
