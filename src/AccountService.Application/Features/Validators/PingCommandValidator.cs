using AccountService.Application.Features.Commands.AccountService;
using FluentValidation;

namespace AccountService.Application.Features.Validators;

public class PingCommandValidator : AbstractValidator<PingCommand>
{
    public PingCommandValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message cannot be empty.")
            .MaximumLength(500).WithMessage("Message cannot exceed 500 characters.")
            .MinimumLength(1).WithMessage("Message must be at least 1 character long.");
    }
}
