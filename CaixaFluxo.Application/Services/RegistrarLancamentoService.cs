using FluentValidation;
using CaixaFluxo.Application.DTOs;
using CaixaFluxo.Application.Interfaces;

namespace CaixaFluxo.Application.Services;

public class RegistrarLancamentoService : IRegistrarLancamentoService
{
    private readonly ILancamentoRepository _repository;
    private readonly IValidator<CriarLancamentoRequest> _validator;

    public RegistrarLancamentoService(ILancamentoRepository repository, IValidator<CriarLancamentoRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<bool> ExecutarAsync(CriarLancamentoRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);
        }

        var tipoFormatado = request.Tipo.Trim().ToUpper();
        await _repository.SalvarAsync(request.Valor, tipoFormatado, request.Descricao, cancellationToken);

        return true;
    }
}
