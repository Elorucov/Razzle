using ELOR.Razzle.DTO.Requests;
using FluentValidation;

namespace ELOR.Razzle.Validators
{
    public sealed class SignInRequestValidator : AbstractValidator<SignInRequest>
    {
        public SignInRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public sealed class SignUpRequestValidator : AbstractValidator<SignUpRequest>
    {
        public SignUpRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(4, 32)
                .Matches("^[A-Za-z0-9_-]+$").WithMessage("may only contain letters, digits, '_' and '-'");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        }
    }
}
