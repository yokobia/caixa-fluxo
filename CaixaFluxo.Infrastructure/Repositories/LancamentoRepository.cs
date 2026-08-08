using CaixaFluxo.Application.DTOs;
using CaixaFluxo.Application.Interfaces;
using CaixaFluxo.Infrastructure.Data;

namespace CaixaFluxo.Infrastructure.Repositories;

public class LancamentoRepository : ILancamentoRepository
{
    private readonly InMemoryDbContext _context;

    public LancamentoRepository(InMemoryDbContext context)
    {
        _context = context;
    }

    public Task SalvarAsync(decimal valor, string tipo, string descricao, CancellationToken cancellationToken)
    {
        // Cria o registro simulando as colunas geradas pelo banco de dados
        var novoLancamento = new LancamentoResponse(
            Id: Guid.NewGuid(),
            Valor: valor,
            Tipo: tipo,
            Descricao: descricao,
            DataCriacao: DateTime.UtcNow
        );

        // Adiciona de forma thread-safe na memória
        _context.Lancamentos.Add(novoLancamento);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<LancamentoResponse>> ObterLancamentosPorDataAsync(DateTime data, CancellationToken cancellationToken)
    {
        // Filtra os lançamentos comparando apenas o componente de Data (Ignore o horário)
        var dataAlvo = data.Date;
        
        var resultados = _context.Lancamentos
            .Where(l => l.DataCriacao.ToLocalTime().Date == dataAlvo)
            .ToList();

        return Task.FromResult<IEnumerable<LancamentoResponse>>(resultados);
    }
}
