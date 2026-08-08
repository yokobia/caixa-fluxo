using Moq;
using Xunit;
using FluentValidation;
using CaixaFluxo.Application.DTOs;
using CaixaFluxo.Application.Interfaces;
using CaixaFluxo.Application.Services;
using CaixaFluxo.Application.Validators;

namespace CaixaFluxo.Tests;

public class RegistrarLancamentoServiceTests
{
    private readonly IValidator<CriarLancamentoRequest> _validator = new CriarLancamentoRequestValidator();

    [Fact]
    public async Task ExecutarAsync_QuandoDadosForemValidos_DeveSalvarComSucesso()
    {
        // ARRANGE
        var request = new CriarLancamentoRequest(500.00m, "Credito ", "Entrada válida"); // Testando com espaço no final
        var repositoryMock = new Mock<ILancamentoRepository>();
        var Service = new RegistrarLancamentoService(repositoryMock.Object, _validator);

        // ACT
        var resultado = await Service.ExecutarAsync(request, CancellationToken.None);

        // ASSERT
        Assert.True(resultado);
        // Verifica se salvou formatando o tipo para caixa alta ("CREDITO") conforme a regra de negócio do Service
        repositoryMock.Verify(repo => repo.SalvarAsync(500.00m, "CREDITO", "Entrada válida", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoValorForMenorOuIgualAZero_DeveLancarArgumentException()
    {
        // ARRANGE
        var request = new CriarLancamentoRequest(0.00m, "Credito", "Valor inválido");
        var repositoryMock = new Mock<ILancamentoRepository>();
        var Service = new RegistrarLancamentoService(repositoryMock.Object, _validator);

        // ACT & ASSERT
        var excecao = await Assert.ThrowsAsync<ArgumentException>(() => 
            Service.ExecutarAsync(request, CancellationToken.None)
        );

        Assert.Equal("O valor do lançamento deve ser maior que zero.", excecao.Message);
        repositoryMock.Verify(repo => repo.SalvarAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoTipoForInvalido_DeveLancarArgumentException()
    {
        // ARRANGE
        var request = new CriarLancamentoRequest(100.00m, "INVALIDO", "Tipo errado");
        var repositoryMock = new Mock<ILancamentoRepository>();
        var Service = new RegistrarLancamentoService(repositoryMock.Object, _validator);

        // ACT & ASSERT
        var excecao = await Assert.ThrowsAsync<ArgumentException>(() => 
            Service.ExecutarAsync(request, CancellationToken.None)
        );

        Assert.Equal("Tipo de lançamento inválido. Use 'Credito' ou 'Debito'.", excecao.Message);
        repositoryMock.Verify(repo => repo.SalvarAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
