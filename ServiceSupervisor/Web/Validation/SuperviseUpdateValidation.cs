using FluentValidation;

using XAS.Rest.Server.Extensions;
using ServiceSupervisorCommon.DataStructures;

namespace ServiceSupervisor.Web.Validation {

    /// <summary>
    /// Validation class for SuperviseUpdate.
    /// </summary>
    /// 
    public class SuperviseUpdateValidation: AbstractValidator<SuperviseUpdate> {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public SuperviseUpdateValidation() {

            RuleFor(p => p.Verb)
                .MaximumLength(32);

            RuleFor(p => p.Name)
                .MaximumLength(32);

            RuleFor(p => p.Domain)
                .MaximumLength(32);

            RuleFor(p => p.Username)
                .MaximumLength(32);

            RuleFor(p => p.Password)
                .MaximumLength(32);

            RuleFor(p => p.ExitRetries)
                .IsInteger();

            RuleFor(p => p.AutoRestart)
                .IsBoolean();

            RuleFor(p => p.Environment)
                .MaximumLength(1024);

            RuleFor(p => p.WorkingDirectory)
                .MaximumLength(256);

        }

    }

}