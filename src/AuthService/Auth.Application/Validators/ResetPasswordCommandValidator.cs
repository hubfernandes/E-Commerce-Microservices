using Auth.Application.Commands;
using Auth.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
namespace Auth.Application.Validators
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        private readonly UserManager<AppUser> _userManager;

        public ResetPasswordCommandValidator(UserManager<AppUser> userManager)
        {
            _userManager = userManager;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Reset token is required.")
                .MustAsync(IsValidToken!).WithMessage("Invalid or expired token.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
        }

        private async Task<bool> IsValidToken(ResetPasswordCommand command, string token, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(command.Email!);
            if (user == null)
                return false;

            return await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", token);
        }
    }
}
