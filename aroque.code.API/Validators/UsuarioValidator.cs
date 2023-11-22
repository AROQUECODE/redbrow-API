using FluentValidation;
using aroque.code.response;
namespace aroque.code.API.Validators
{
    public class UsuarioValidator : AbstractValidator<UsuarioDTO>
    {
        public UsuarioValidator()
        {
            RuleFor(x=> x.Nombre)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El campo {PropertyName} es obligatorio.")
                .Length(1,50)
                .WithMessage("{PropertyName} tiene {TotalLength} letras. Debe tener una logitud entre {MinLength} y {MaxLength} letras.");

            RuleFor(x => x.Correo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El campo {PropertyName} es obligatorio.")
                .EmailAddress()
                .WithMessage("El valor ingresado no es un correo valido");

            RuleFor(x => x.Edad)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El campo {PropertyName} es obligatorio.")
                .InclusiveBetween(18,65)
                .WithMessage("Recuerda que nuestro sistema solo admite a mayores de edad.");

            RuleFor(x => x.Clave)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El campo {PropertyName} es obligatorio.")
                .Length(1, 20)
                .WithMessage("{PropertyName} tiene {TotalLength} letras. Debe tener una logitud entre {MinLength} y {MaxLength} letras.");
        }
    }
}
