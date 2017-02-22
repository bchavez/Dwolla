using System;
using FluentAssertions;
using FluentValidation;
using Newtonsoft.Json;
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
        public void getting_redirect_url_from_failed_checkout_response_throws_exception()
        {
            var r = GetFailedResponse();

            var api = new DwollaServerCheckoutApi( "test", "test" );

            new Action( () => api.GetCheckoutRedirectUrl( r ) )
                .ShouldThrow<ValidationException>();
        }

        private DwollaCheckoutResponse GetFailedResponse()
        {
            var json =
                @"{""Success"":false,""Message"":""Application or purchase order does not have a payment redirect URL associated with it. Please submit an application details change request or provide a redirect URL."",""Response"":null,""_links"":null}";

            return JsonConvert.DeserializeObject<DwollaCheckoutResponse>(json);
        }

        private DwollaCheckoutResponse GetSuccessResponse()
        {
            var json = @"{""Success"":true,""Message"":""Success"",""Response"":{""CheckoutId"":""43957894-d8fa-41dc-a75d-c21c4ce9dcc9""},""_links"":null}";

            return JsonConvert.DeserializeObject<DwollaCheckoutResponse>(json);
        }

        [Test]
        public void can_deseralize_a_server_response()
        {
            var x = GetSuccessResponse();
            x.Message.Should().Be("Success");
            x.Success.Should().Be(true);
            x.CheckoutId.Should().Be("43957894-d8fa-41dc-a75d-c21c4ce9dcc9");
            x.GetRedirectUrl().Should().Be("https://www.dwolla.com/payment/checkout/43957894-d8fa-41dc-a75d-c21c4ce9dcc9");

        }
    }
}