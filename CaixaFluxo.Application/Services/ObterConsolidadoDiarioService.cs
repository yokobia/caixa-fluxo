using CaixaFluxo.Application.DTOs;
using CaixaFluxo.Application.Interfaces;

namespace CaixaFluxo.Application.Services;

public class ObterConsolidadoDiarioService : IObterConsolidadoDiarioService
{
    private readonly ILancamentoRepository _repository;

    public ObterConsolidadoDiarioService(ILancamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<ConsolidaSaldoDiarioResponse> ExecutarAsync(DateTime data, CancellationToken cancellationToken)
    {
        // 1. Busca os lançamentos da data informada via repositório
        var lancamentosDoDia = await _repository.ObterLancamentosPorDataAsync(data, cancellationToken);

        // 2. Regra de Negócio: Cálculos de Consolidação
        decimal totalCreditos = 0;
        decimal totalDebitos = 0;

        foreach (var lancamento in lancamentosDoDia)
        {
            if (lancamento.Tipo == "CREDITO")
                totalCreditos += lancamento.Valor;
            else if (lancamento.Tipo == "DEBITO")
                totalDebitos += lancamento.Valor;
        }

        decimal saldoConsolidado = totalCreditos - totalDebitos;

        // 3. Retorna o DTO estruturado com o resultado consolidado
        return new ConsolidaSaldoDiarioResponse(
            Data: data.Date,
            TotalCreditos: totalCreditos,
            TotalDebitos: totalDebitos,
            SaldoConsolidado: saldoConsolidado
        );
    }
}
