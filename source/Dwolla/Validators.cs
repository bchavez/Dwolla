using System;
using System.Linq;
using FluentValidation;

namespace Dwolla
{
    public class DwollaCheckoutRequestValidator : AbstractValidator<DwollaCheckoutRequest>
    {
        public DwollaCheckoutRequestValidator()
        {
            RuleFor( cr => cr.Key ).NotEmpty()
                .WithName( "CheckoutRequest.Key" );

            RuleFor( cr => cr.Secret ).NotEmpty()
                .WithName("CheckoutRequest.Secret");

            RuleFor( cr => cr.Callback )
                .Must( uri => uri.IsAbsoluteUri )
                .Unless( cr => cr.Callback == null )
                .WithMessage( "The CheckoutRequest.Callback URL must be absolute and a valid http/https URL" );

            RuleFor( cr => cr.Redirect )
                .Must( uri => uri.IsAbsoluteUri )
                .Unless( cr => cr.Redirect == null )
                .WithMessage( "The CheckoutRequest.Redirect URL must be absolute and a valid http/https URL." );

            RuleFor( cr => cr.OrderId ).Length( 1, 100 )
                .Unless( cr => cr.OrderId == null );

            RuleFor( cr => cr.PurchaseOrder ).NotNull()
                .SetValidator( new DwollaPurchaseOrderValidator() );
        }
    }

    public class DwollaPurchaseOrderValidator : AbstractValidator<DwollaPurchaseOrder>
    {
        public DwollaPurchaseOrderValidator()
        {
            RuleFor( po => po.DestinationId ).NotEmpty()
                .WithName("PurchaseOrder.DestinationId");

            RuleFor( po => po.Discount ).LessThanOrEqualTo( 0.00m )
                .WithName("PurchaseOrder.Discount");

            RuleFor( po => po.Shipping ).GreaterThanOrEqualTo( 0.00m )
                .WithName( "PurchaseOrder.Shipping" );

            RuleFor( po => po.Tax ).GreaterThanOrEqualTo( 0.00m )
                .WithName("PurchaseOrder.Tax");

            RuleFor( po => po.OrderItems ).NotNull()
                .Must( x => x.Count >= 1 )
                .SetCollectionValidator( new DwollaOrderItemValidator() )
                .WithName( "PurchaseOrder.OrderItems" );

            RuleFor( po => po.Total ).GreaterThanOrEqualTo( 1.00m )
                .WithName("PurchaseOrder.Total");

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

            RuleFor( item => item.Name ).Length( 1, 100 )
                .WithName("OrderItem.Name");

            RuleFor( item => item.Price ).GreaterThanOrEqualTo( 0.00m )
                .WithMessage( "The 'OrderItem.{PropertyName}' for '{0}' must be greater than or equal to ${ComparisonValue}.", item => item.Name );

            RuleFor( item => item.Quantity ).GreaterThanOrEqualTo( 1 )
                .WithMessage( "The 'OrderItem.{PropertyName}' for '{0}' must be greater than or equal to {ComparisonValue}.", item => item.Name );
                
        }
    }

    public class DwollaCheckoutResponseValidator : AbstractValidator<DwollaCheckoutResponse>
    {
        public DwollaCheckoutResponseValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor( cr => cr.Result )
                .Must( r => r == DwollaCheckoutRequestResult.Success )
                .WithMessage( "The checkout request failed. Message from Dwolla's Servers: '{0}'", cr => cr.Message );

            RuleFor( r => r.CheckoutId ).NotEmpty()
                .WithName( "CheckoutResponse.CheckoutId" );
        }
    }
}