using FluentValidation;
using AppAdminEmployed.Models;

public class EmpleadoValidator : AbstractValidator<EmpleadoModel>
{
    public EmpleadoValidator()
    {
        RuleFor(x => x.IDENTIFICACION.ToString())
            .NotEmpty().WithMessage("El campo es obligatorio")
            .MinimumLength(3).WithMessage("Debe tener al menos 3 caracteres")
            .MaximumLength(12).WithMessage("No puede tener más de 12 caracteres")
            .Matches(@"^[0-9]+$").WithMessage("Solo puede contener números");

        RuleFor(x => x.PRIMER_NOMBRE)
            .NotEmpty().WithMessage("El campo es obligatorio")
            .MinimumLength(3).WithMessage("Debe tener al menos 3 caracteres")
            .MaximumLength(50).WithMessage("No puede tener más de 50 caracteres")
            .Matches(@"^[a-zA-Z ]+$").WithMessage("El nombre solo puede contener letras");

        RuleFor(x => x.SEGUNDO_NOMBRE)
            .MinimumLength(3).WithMessage("Debe tener al menos 3 caracteres")
            .MaximumLength(50).WithMessage("No puede tener más de 50 caracteres")
            .Matches(@"^[a-zA-Z ]+$").WithMessage("El nombre solo puede contener letras");

        RuleFor(x => x.PRIMER_APELLIDO)
            .NotEmpty().WithMessage("El campo es obligatorio")
            .MinimumLength(3).WithMessage("Debe tener al menos 3 caracteres")
            .MaximumLength(50).WithMessage("No puede tener más de 50 caracteres")
            .Matches(@"^[a-zA-Z ]+$").WithMessage("El nombre solo puede contener letras");
        
        RuleFor(x => x.SEGUNDO_APELLIDO)
            .NotEmpty().WithMessage("El campo es obligatorio")
            .MinimumLength(3).WithMessage("Debe tener al menos 3 caracteres")
            .MaximumLength(50).WithMessage("No puede tener más de 50 caracteres")
            .Matches(@"^[a-zA-Z ]+$").WithMessage("El nombre solo puede contener letras");
    }
}