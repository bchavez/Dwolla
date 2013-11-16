using FluentValidation;

namespace Dwolla.Gateway.Validators
{
    public class DwollaOrderItemValidator : AbstractValidator<DwollaOrderItem>
    {
        public DwollaOrderItemValidator()
        {
            RuleFor( item => item.Description )
                .NotNull()
                .Length( 1, 200 );

            RuleFor( item => item.Name )
                .NotNull()
                .Length( 1, 100 )
                .WithName("OrderItem.Name");

            RuleFor( item => item.Price ).GreaterThanOrEqualTo( 0.00m )
                .WithMessage( "The 'OrderItem.{PropertyName}' for '{0}' must be greater than or equal to ${ComparisonValue}.", item => item.Name );

            RuleFor( item => item.Quantity ).GreaterThanOrEqualTo( 1 )
                .WithMessage( "The 'OrderItem.{PropertyName}' for '{0}' must be greater than or equal to {ComparisonValue}.", item => item.Name );
                
        }
    }
}