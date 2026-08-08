using System.Collections.Concurrent;
using CaixaFluxo.Application.DTOs;

namespace CaixaFluxo.Infrastructure.Data;

public class InMemoryDbContext
{
    // ConcurrentBag é otimizado para operações concorrentes de alta performance
    public ConcurrentBag<LancamentoResponse> Lancamentos { get; } = new();
}
