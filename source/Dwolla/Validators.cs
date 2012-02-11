using System;
using System.Linq;
using FluentValidation;

namespace Dwolla
{
    public class DwollaCheckoutRequestValidator : AbstractValidator<DwollaCheckoutRequest>
    {
        public DwollaCheckoutRequestValidator()
        {
            RuleFor( cr => cr.Key ).NotEmpty();

            RuleFor( cr => cr.Secret ).NotEmpty();

            RuleFor( cr => cr.Callback )
                .Must( uri => uri.IsAbsoluteUri )
                .Unless( cr => cr.Callback == null )
                .WithMessage( "The callback URL must be absolute" );

            RuleFor( cr => cr.Redirect )
                .Must( uri => uri.IsAbsoluteUri )
                .Unless( cr => cr.Redirect == null )
                .WithMessage( "The redirect URL must be absolute" );

            RuleFor( cr => cr.OrderId ).Length( 1, 100 )
                .Unless( cr => cr.OrderId == null );

            RuleFor( cr => cr.PurchaseOrder ).NotNull();
        }
    }

    public class DwollaPurchaseOrderValidator : AbstractValidator<DwollaPurchaseOrder>
    {
        public DwollaPurchaseOrderValidator()
        {
            RuleFor( po => po.DestinationId ).NotEmpty();

            RuleFor( po => po.Discount ).LessThanOrEqualTo( 0.00m );

            RuleFor( po => po.Shipping ).GreaterThanOrEqualTo( 0.00m );

            RuleFor( po => po.Tax ).GreaterThanOrEqualTo( 0.00m );

            RuleFor( po => po.OrderItems ).NotNull()
                .Must( x => x.Count >= 1 )
                .SetCollectionValidator( new DwollaOrderItemValidator() );

            RuleFor( po => po.Total ).GreaterThanOrEqualTo( 1.00m );

            RuleFor( po => po.FacilitatorAmount ).GreaterThanOrEqualTo( 0 )
                .Must( HasValidFacilitatorAmount )
                .When( po => po.FacilitatorAmount != null )
                .WithMessage( "Facilitator fee cannot exceed 25% of the total and must be greater than or equal to zero or null." );
        }
        protected virtual bool HasValidFacilitatorAmount(DwollaPurchaseOrder po, decimal? facilitatorAmount )
        {
            const decimal FacilitatorMaxPrecent = 0.25m;

            if( facilitatorAmount == null ) return true;

            if( facilitatorAmount < 0 ) return false;

            var maximumFacilitatorFee = po.Total * FacilitatorMaxPrecent;
            if( facilitatorAmount >= maximumFacilitatorFee ) return false;

            return true;
        }
    }
    
    public class DwollaOrderItemValidator : AbstractValidator<DwollaOrderItem>
    {
        public DwollaOrderItemValidator()
        {
            RuleFor( item => item.Description ).Length(1, 200)
                .When( item => item.Description != null );

            RuleFor( item => item.Name ).Length( 1, 100 );

            RuleFor( item => item.Price ).GreaterThanOrEqualTo( 0.00m );

            RuleFor( item => item.Quantity ).GreaterThanOrEqualTo( 1 );
        }
    }

    public class DwollaCallbackValidator : AbstractValidator<DwollaCallback>
    {
        public DwollaCallbackValidator(string appSecret)
        {
            RuleFor( c => c.Signature ).Must( HasValidSignature );

        }

        protected virtual bool HasValidSignature( DwollaCallback c, string signature )
        {
            var amount = c.Amount;
            return false;
        }
    }
}