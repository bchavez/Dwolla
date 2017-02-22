using System;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Dwolla.Checkout.Tests
{
    [TestFixture]
    public class JsonSerializationTests
    {
        [Test]
        public void can_deseralize_checkout_callback()
        {
            var json =
                @"
{
    'Amount': 3.25,
    'OrderId': 'A1B2C3',
    'TestMode': true,
    'TransactionId': 1,
    'CheckoutId': 'C3D4DC4F-5074-44CA-8639-B679D0A70803',
    'Status': 'Completed',
    'Signature': '7f42ba58ff0d20486fdc2634745e8e7c92cb6321'
}";
            var o = JsonConvert.DeserializeObject<DwollaCallback>( json );

            o.Amount.Should().Be( 3.25m );
            o.OrderId.Should().Be( "A1B2C3" );
            o.TestMode.Should().BeTrue();
            o.TransactionId.Should().Be( 1 );
            o.CheckoutId.Should().Be( "C3D4DC4F-5074-44CA-8639-B679D0A70803" );
            o.Status.Should().Be( DwollaStatus.Completed );
            o.Signature.Should().Be( "7f42ba58ff0d20486fdc2634745e8e7c92cb6321" );
        }

        [Test]
        public void can_deseralize_successful_checkout_response()
        {
            var json = @"{""Success"":true,""Message"":""Success"",""Response"":{""CheckoutId"":""43957894-d8fa-41dc-a75d-c21c4ce9dcc9""},""_links"":null}";
            var o = JsonConvert.DeserializeObject<DwollaCheckoutResponse>(json);

            o.Success.Should().BeTrue();
            o.CheckoutId.Should().Be("43957894-d8fa-41dc-a75d-c21c4ce9dcc9");
            o.Message.Should().Be("Success");
        }
        
        [Test]
        public void can_deseralize_failed_checkout_response()
        {
            var json = @"{""Success"":false,""Message"":""Application or purchase order does not have a payment redirect URL associated with it. Please submit an application details change request or provide a redirect URL."",""Response"":null,""_links"":null}";
            var o = JsonConvert.DeserializeObject<DwollaCheckoutResponse>(json);

            o.Success.Should().BeFalse();
            o.CheckoutId.Should().BeNull();
            o.Message.Should().Be("Application or purchase order does not have a payment redirect URL associated with it. Please submit an application details change request or provide a redirect URL.");
        }

        [Test]
        public void can_seralize_valid_checkout_request()
        {
            var expected = @"{
  ""client_id"": null,
  ""client_secret"": null,
  ""AssumeCosts"": false,
  ""CheckoutWithApi"": false,
  ""Callback"": ""http://www.example.com/order-callback"",
  ""Redirect"": null,
  ""AllowGuestCheckout"": false,
  ""AllowFundingSources"": false,
  ""OrderId"": ""MyOrderTest"",
  ""Test"": false,
  ""PurchaseOrder"": {
    ""CustomerInfo"": null,
    ""DestinationId"": ""812-111-1111"",
    ""Discount"": 0.0,
    ""Shipping"": 0.0,
    ""Tax"": 0.0,
    ""Total"": 25.00,
    ""FacilitatorAmount"": null,
    ""OrderItems"": [
      {
        ""Description"": ""Expensive Candy Bar"",
        ""Name"": ""Candy Bar"",
        ""Price"": 25.00,
        ""Quantity"": 1
      }
    ],
    ""Notes"": null,
    ""Metadata"": {}
  },
  ""ProfileId"": null,
  ""AdditionalFundingSources"": null
}";

            var checkoutRequest = new DwollaCheckoutRequest
                                      {
                                          OrderId = "MyOrderTest",
                                          PurchaseOrder = new DwollaPurchaseOrder
                                                              {
                                                                  OrderItems =
                                                                      {
                                                                          new DwollaOrderItem
                                                                              ( name: "Candy Bar",
                                                                                price: 25.00m,
                                                                                quantity: 1 )
                                                                              {
                                                                                  Description = "Expensive Candy Bar",
                                                                              }
                                                                      },
                                                                  DestinationId = "812-111-1111",
                                                              },

                                          Callback = new Uri( "http://www.example.com/order-callback" )

                                      };

            var json = JsonConvert.SerializeObject( checkoutRequest, Formatting.Indented );
            Console.WriteLine( json );

            //Help with debugging serialization issues
            //File.WriteAllText( "_output.txt", json );
            //File.WriteAllText( "_expected.txt", expected );

            json.Should().Be( expected );
        }
    }
}