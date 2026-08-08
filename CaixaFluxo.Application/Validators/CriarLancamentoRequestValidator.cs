using FluentValidation;
using CaixaFluxo.Application.DTOs;

namespace CaixaFluxo.Application.Validators;

public class CriarLancamentoRequestValidator : AbstractValidator<CriarLancamentoRequest>
{
    public CriarLancamentoRequestValidator()
    {
        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor do lançamento deve ser maior que zero.");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("O tipo de lançamento não pode estar vazio.")
            .Must(tipo => tipo.Trim().ToUpper() == "CREDITO" || tipo.Trim().ToUpper() == "DEBITO")
            .WithMessage("Tipo de lançamento inválido. Use 'Credito' ou 'Debito'.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
    }
}
