using CaixaFluxo.Application.DTOs;

namespace CaixaFluxo.Application.Interfaces;

public interface ILancamentoRepository
{
    Task SalvarAsync(decimal valor, string tipo, string descricao, CancellationToken cancellationToken);
    Task<IEnumerable<LancamentoResponse>> ObterLancamentosPorDataAsync(DateTime data, CancellationToken cancellationToken);
}
