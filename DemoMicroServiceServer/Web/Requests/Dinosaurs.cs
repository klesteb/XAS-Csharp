
using FluentValidation;

using XAS.Rest.Server.Extensions;
using DemoModelCommon.DataStructures;

namespace DemoMicroServiceServer.Web.Requests {

    public class DinosaurPostValidation: AbstractValidator<DinosaurPost> {

        public DinosaurPostValidation() {

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(p => p.Status)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(p => p.Height)
                .NotEmpty()
                .IsInteger();

        }

    }

    public class DinosaurUpdateValidation: AbstractValidator<DinosaurUpdate> {

        public DinosaurUpdateValidation() {

            RuleFor(p => p.Name)
                .MaximumLength(32);

            RuleFor(p => p.Status)
                .MaximumLength(32);

            RuleFor(p => p.Height)
                .IsInteger();
        }

    }

}
