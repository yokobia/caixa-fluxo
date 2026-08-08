using Microsoft.AspNetCore.Mvc;
using CaixaFluxo.Application.DTOs;
using CaixaFluxo.Application.Interfaces;

namespace CaixaFluxo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LancamentosController : ControllerBase
{
    private readonly IRegistrarLancamentoService _registrarService;
    private readonly IObterConsolidadoDiarioService _consolidadoService;

    // Injeção de dependência dos Casos de Uso através do construtor
    public LancamentosController(
        IRegistrarLancamentoService registrarService,
        IObterConsolidadoDiarioService consolidadoService)
    {
        _registrarService = registrarService;
        _consolidadoService = consolidadoService;
    }

    /// <summary>
    /// Registra um lançamento de crédito ou débito.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarLancamentoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _registrarService.ExecutarAsync(request, cancellationToken);
            return StatusCode(201, new { Mensagem = "Lançamento registrado com sucesso!" });
        }
        catch (ArgumentException ex)
        {
            // Captura erros de validação de negócio (Ex: tipo inválido ou valor negativo)
            return BadRequest(new { Erro = ex.Message });
        }
        catch (Exception)
        {
            // Garante isolamento de falha interna
            return StatusCode(500, new { Erro = "Ocorreu um erro interno ao processar o lançamento." });
        }
    }

    /// <summary>
    /// Obtém o consolidado diário de uma data específica.
    /// </summary>
    /// <param name="data">Opcional. Se não informada, assume o dia atual.</param>
    [HttpGet("consolidado")]
    public async Task<IActionResult> ObterConsolidado([FromQuery] DateTime? data, CancellationToken cancellationToken)
    {
        try
        {
            var dataAlvo = data ?? DateTime.Today;
            var consolidado = await _consolidadoService.ExecutarAsync(dataAlvo, cancellationToken);
            return Ok(consolidado);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Erro = "Ocorreu um erro interno ao gerar o relatório consolidado." });
        }
    }
}
