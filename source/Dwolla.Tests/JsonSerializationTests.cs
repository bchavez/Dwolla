using System;
using System.IO;
using Dwolla.OffSiteGateway;
using FluentAssertions;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Dwolla.Tests
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
            o.Status.Should().Be( DwollaCallbackStatus.Completed );
            o.Signature.Should().Be( "7f42ba58ff0d20486fdc2634745e8e7c92cb6321" );
        }

        [Test]
        public void can_deseralize_successful_checkout_response()
        {
            var json = @"
{
    'Result': 'Success',
    'CheckoutId': 'C3D4DC4F-5074-44CA-8639-B679D0A70803'
}";
            var o = JsonConvert.DeserializeObject<DwollaCheckoutResponse>(json);

            o.Result.Should().Be( DwollaCheckoutResponseResult.Success );
            o.CheckoutId.Should().Be( "C3D4DC4F-5074-44CA-8639-B679D0A70803" );
            o.Message.Should().BeNull();
        }
        
        [Test]
        public void can_deseralize_failed_checkout_response()
        {
            var json = @"
{
    'Result': 'Failure',
    'Message': 'Invalid total.'
}";
            var o = JsonConvert.DeserializeObject<DwollaCheckoutResponse>(json);

            o.Result.Should().Be(DwollaCheckoutResponseResult.Failure);
            o.CheckoutId.Should().BeNull();
            o.Message.Should().Be( "Invalid total." );
        }

        [Test]
        public void can_seralize_valid_checkout_request()
        {
            var expected = @"{
  ""Key"": null,
  ""Secret"": null,
  ""Callback"": ""http://www.example.com/order-callback"",
  ""Redirect"": null,
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
    ""Notes"": null
  }
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