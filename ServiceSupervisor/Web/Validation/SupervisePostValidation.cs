
using FluentValidation;

using XAS.Rest.Server.Extensions;
using ServiceSupervisorCommon.DataStructures;

namespace ServiceSupervisor.Web.Validation {

    /// <summary>
    /// Validation class for SupervisePost.
    /// </summary>
    /// 
    public class SupervisePostValidation: AbstractValidator<SupervisePost> {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public SupervisePostValidation() {

            RuleFor(p => p.Verb)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(p => p.Domain)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(p => p.Username)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(p => p.Password)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(p => p.AutoStart)
                .NotEmpty()
                .IsBoolean();

            RuleFor(p => p.ExitRetries)
                .NotEmpty()
                .IsInteger();

            RuleFor(p => p.AutoRestart)
                .NotEmpty()
                .IsBoolean();

            RuleFor(p => p.Environment)
                .NotEmpty()
                .MaximumLength(1024);

            RuleFor(p => p.WorkingDirectory)
                .NotEmpty()
                .MaximumLength(256);

        }

    }

}
