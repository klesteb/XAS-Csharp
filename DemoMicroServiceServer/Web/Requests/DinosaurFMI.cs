
using FluentValidation;

namespace DemoMicroServiceServer.Model.Services {

    public class DinosaurFMI {

        public string Name { get; set; }
        public string Status { get; set; }
        public int Height { get; set; }

    }

    public class DinosaurFMIValidation: AbstractValidator<DinosaurFMI> {

        public DinosaurFMIValidation() {

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(p => p.Status)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(p => p.Height)
                .NotEmpty();

        }
        
    }

 }
