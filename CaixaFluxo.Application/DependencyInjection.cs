using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using CaixaFluxo.Application.Interfaces;
using CaixaFluxo.Application.Services;

namespace CaixaFluxo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationStructure(this IServiceCollection services)
    {
        services.AddScoped<IRegistrarLancamentoService, RegistrarLancamentoService>();
        services.AddScoped<IObterConsolidadoDiarioService, ObterConsolidadoDiarioService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
