using System;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;

namespace Dwolla.Checkout.Tests
{
    [TestFixture]
    public class DwollaServerCheckoutTests
    {
        [Test]
        public void server_checkout_api_can_verify_callback()
        {
            //appSecret = "test"
            var c = new DwollaCallback
                        {
                            Amount = 3.25m,
                            CheckoutId = "C3D4DC4F-5074-44CA-8639-B679D0A70803",
                            Signature = "7f42ba58ff0d20486fdc2634745e8e7c92cb6321"
                        };

            var api = new DwollaServerCheckoutApi( "test", "test" );

            api.VerifyAuthenticity( c )
               .Should().BeTrue();
        }

        [Test]
        public void server_checkout_api_can_get_redirect_url_on_successful_response()
        {
            var r = new DwollaCheckoutResponse
                        {
                            Result = DwollaCheckoutResponseResult.Success,
                            CheckoutId = "C3D4DC4F-5074-44CA-8639-B679D0A70803",
                        };

            var api = new DwollaServerCheckoutApi( "test", "test" );

            var redirectUrl = api.GetCheckoutRedirectUrl( r );

            redirectUrl.Should().Be( "https://www.dwolla.com/payment/checkout/C3D4DC4F-5074-44CA-8639-B679D0A70803" );
        }

        [Test]
        public void getting_redirect_url_from_failed_checkout_response_throws_exception()
        {
            var r = new DwollaCheckoutResponse
                        {
                            Result = DwollaCheckoutResponseResult.Failure,
                            Message = "invalid total."
                        };

            var api = new DwollaServerCheckoutApi( "test", "test" );

            new Action( () => api.GetCheckoutRedirectUrl( r ) )
                .ShouldThrow<ValidationException>();
        }
    }
}