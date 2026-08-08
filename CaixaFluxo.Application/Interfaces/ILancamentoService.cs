using CaixaFluxo.Application.DTOs;

namespace CaixaFluxo.Application.Interfaces;

public interface IRegistrarLancamentoService
{
    Task<bool> ExecutarAsync(CriarLancamentoRequest request, CancellationToken cancellationToken);
}

public interface IObterConsolidadoDiarioService
{
    Task<ConsolidaSaldoDiarioResponse> ExecutarAsync(DateTime data, CancellationToken cancellationToken);
}
