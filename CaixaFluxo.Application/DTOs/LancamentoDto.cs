namespace CaixaFluxo.Application.DTOs;

public record CriarLancamentoRequest(
    decimal Valor, 
    string Tipo, // "Credito" ou "Debito"
    string Descricao
);

public record LancamentoResponse(
    Guid Id, 
    decimal Valor, 
    string Tipo, 
    string Descricao, 
    DateTime DataCriacao
);

public record ConsolidaSaldoDiarioResponse(
    DateTime Data, 
    decimal TotalCreditos, 
    decimal TotalDebitos, 
    decimal SaldoConsolidado
);
