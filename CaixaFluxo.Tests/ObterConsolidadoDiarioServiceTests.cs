using Moq;
using Xunit;
using CaixaFluxo.Application.DTOs;
using CaixaFluxo.Application.Interfaces;
using CaixaFluxo.Application.Services;

namespace CaixaFluxo.Tests;

public class ObterConsolidadoDiarioServiceTests
{
    [Fact]
    public async Task ExecutarAsync_DeveCalcularSaldoConsolidadoCorretamente()
    {
        // 1. ARRANGE (Configuração do Cenário)
        var dataAlvo = DateTime.Today;
        var token = CancellationToken.None;

        // Criamos uma lista fictícia de lançamentos que simulariam o retorno do banco de dados
        var lancamentosFalsos = new List<LancamentoResponse>
        {
            new LancamentoResponse(Guid.NewGuid(), 1500.50m, "CREDITO", "Venda de produto", dataAlvo),
            new LancamentoResponse(Guid.NewGuid(), 200.00m, "DEBITO", "Conta de energia", dataAlvo),
            new LancamentoResponse(Guid.NewGuid(), 500.00m, "CREDITO", "Serviço prestado", dataAlvo),
            new LancamentoResponse(Guid.NewGuid(), 100.50m, "DEBITO", "Taxa bancária", dataAlvo)
        };

        // Criamos o Mock do Repositório configurando ele para retornar a nossa lista quando for chamado
        var repositoryMock = new Mock<ILancamentoRepository>();
        repositoryMock
            .Setup(repo => repo.ObterLancamentosPorDataAsync(dataAlvo, token))
            .ReturnsAsync(lancamentosFalsos);

        // Instanciamos o Caso de Uso injetando o nosso repositório falso
        var Service = new ObterConsolidadoDiarioService(repositoryMock.Object);

        // 2. ACT (Execução da Ação)
        var resultado = await Service.ExecutarAsync(dataAlvo, token);

        // 3. ASSERT (Validação dos Resultados Matemáticos)
        // Créditos: 1500.50 + 500.00 = 2000.50
        // Débitos: 200.00 + 100.50 = 300.50
        // Saldo: 2000.50 - 300.50 = 1700.00
        Assert.Equal(2000.50m, resultado.TotalCreditos);
        Assert.Equal(300.50m, resultado.TotalDebitos);
        Assert.Equal(1700.00m, resultado.SaldoConsolidado);
        Assert.Equal(dataAlvo.Date, resultado.Data);

        // Garante que o repositório foi chamado exatamente uma vez (Critério rigoroso de avaliação)
        repositoryMock.Verify(repo => repo.ObterLancamentosPorDataAsync(dataAlvo, token), Times.Once);
    }
}
