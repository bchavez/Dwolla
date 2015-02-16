using System;
using System.Linq;
using Dwolla.Checkout.Validators;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Dwolla.Checkout.Tests.ValidationTests
{
    [TestFixture]
    public class DwollaCheckoutRequestValidatorTests
    {
        private DwollaCheckoutRequestValidator validator;

        [SetUp]
        public void BeforeEachTest()
        {
            validator = new DwollaCheckoutRequestValidator();
        }

        [Test]
        public void valid_key()
        {
            validator.ShouldNotHaveValidationErrorFor( cr => cr.ClientId, "abcdefg" );

            validator.ShouldHaveValidationErrorFor( cr => cr.ClientId, null as string );
            validator.ShouldHaveValidationErrorFor( cr => cr.ClientId, string.Empty );
        }
        [Test]
        public void valid_secret()
        {
            validator.ShouldNotHaveValidationErrorFor( cr => cr.ClientSecret, "abcdefg" );

            validator.ShouldHaveValidationErrorFor( cr => cr.ClientSecret, null as string );
            validator.ShouldHaveValidationErrorFor( cr => cr.ClientSecret, string.Empty );
        }


        [Test]
        public void callback_url_validation()
        {
            validator.ShouldNotHaveValidationErrorFor( cr => cr.Callback, new Uri( "http://www.example.com/callback" ) );

            validator.ShouldHaveValidationErrorFor( cr => cr.Callback, new Uri( "/callback", UriKind.Relative ) );
        }

        [Test]
        public void redirect_url_validation()
        {
            validator.ShouldNotHaveValidationErrorFor( cr => cr.Redirect, new Uri( "http://www.example.com/redirect" ) );

            validator.ShouldHaveValidationErrorFor( cr => cr.Redirect, new Uri( "/redirect", UriKind.Relative ) );
        }

        [Test]
        public void orderid_validation()
        {
            validator.ShouldNotHaveValidationErrorFor( cr => cr.OrderId, "A1B2C3" );
            validator.ShouldNotHaveValidationErrorFor( cr => cr.OrderId, null as string );

            validator.ShouldHaveValidationErrorFor( cr => cr.ClientSecret, "" );
        }

        [Test]
        public void allow_guest_checkout_validation()
        {
            var cr = new DwollaCheckoutRequest()
                {
                    AllowGuestCheckout = true,
                    AllowFundingSources = false
                };

            validator.ShouldHaveValidationErrorFor( x => x.AllowFundingSources, cr );

            cr.AllowGuestCheckout = true;
            cr.AllowFundingSources = true;
            validator.ShouldNotHaveValidationErrorFor( x => x.AllowFundingSources, cr );

            cr.AllowGuestCheckout = false;
            cr.AllowFundingSources = false;
            validator.ShouldNotHaveValidationErrorFor( x => x.AllowFundingSources, cr );
        }

        [Test]
        public void setting_additional_funding_sources_requires_allow_funding_sources_to_be_true()
        {
            var cr = new DwollaCheckoutRequest()
                {
                    AllowFundingSources = false,
                    AdditionalFundingSources = FundingSource.Banks | FundingSource.Credit
                };
            validator.ShouldHaveValidationErrorFor(x => x.AllowFundingSources, cr);
            cr.AllowFundingSources = true;
            validator.ShouldNotHaveValidationErrorFor(x => x.AllowFundingSources, cr);
        }

        [Test]
        public void purchase_order_validation()
        {
            validator.ShouldHaveValidationErrorFor( cr => cr.PurchaseOrder, null as DwollaPurchaseOrder );
        }

        [Test]
        public void should_have_child_validator_on_purcahse_order_property()
        {
            validator.ShouldHaveChildValidator( cr => cr.PurchaseOrder, typeof(DwollaPurchaseOrderValidator) );
        }


        [Test]
        public void invalid_order_item_should_cause_checkout_request_to_fail_validation()
        {
            var badRequest = new DwollaCheckoutRequest
                                 {
                                     ClientId = "abcdefg",
                                     ClientSecret = "abcdefg",
                                     OrderId = "MyOrderId1",
                                     PurchaseOrder = new DwollaPurchaseOrder
                                                         {
                                                             OrderItems =
                                                                 {
                                                                     new DwollaOrderItem("Candy Bar",-25.00m, 0)
                                                                         {
                                                                             Description = "Expensive Candy Bar",
                                                                         }
                                                                 },
                                                             DestinationId = "",
                                                         },
                                 };

            var results = new AttributedValidatorFactory().GetValidator<DwollaCheckoutRequest>()
                .Validate( badRequest );

            results.IsValid.Should().BeFalse();

            //check customized error message descriptions too.
            results.Errors.Select( x =>
                                       {
                                           Console.WriteLine( (object)x );
                                           return x.ErrorMessage;
                                       }
                ).SequenceEqual( new[]
                                     {
                                         "'PurchaseOrder.DestinationId' should not be empty.",
                                         "The 'OrderItem.Price' for 'Candy Bar' must be greater than or equal to $0.00.",
                                         "The 'OrderItem.Quantity' for 'Candy Bar' must be greater than or equal to 1.",
                                         "'PurchaseOrder.Total' must be greater than or equal to '1.00'."
                                     } )
                .Should().BeTrue();
        }
    }
}