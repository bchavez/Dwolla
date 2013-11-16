using System;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Dwolla.Tests
{
    [TestFixture]
    [Explicit]
    public class UsageExamples
    {
        [Test]
        public void example_checkout_request()
        {
            var api = new DwollaServerCheckoutApi( appKey:"...", appSecret: "..." );

            //Create a checkout request
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
            
            //Optional: Validate your checkout request before
            //          sending the request to Dwolla.
            var preflightCheck = api.ValidatorFactory.GetValidator<DwollaCheckoutRequest>()
                .Validate( checkoutRequest );
            if( !preflightCheck.IsValid )
            {
                //Check preflightCheck.Errors for a list of validation errors.
            }
            
            //Send the request to Dwolla.
            var checkoutResponse = api.SendCheckoutRequest( checkoutRequest );

            if( checkoutResponse.Result == DwollaCheckoutResponseResult.Failure )
            {
                //Handle Error
            }
            else if( checkoutResponse.Result == DwollaCheckoutResponseResult.Success)
            {
                var redirectUrl = api.GetCheckoutRedirectUrl( checkoutResponse );
                //Send HTTP Redirect to browser so the 
                //customer can finish the checkout process
            }
        }

        [Test]
        public void example_callback()
        {
            //After the customer has completed the checkout process
            //Dwolla will POST a JSON callback object to your Callback URL
            //with the results of the payment.
            var jsonCallback =
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
            //Parse the JSON into an object
            var receivedCallback = JsonConvert.DeserializeObject<DwollaCallback>( jsonCallback );
            
            var api = new DwollaServerCheckoutApi( appKey: "...", appSecret: "..." );

            //Verify the DwollaCallback.Singature
            //to ensure this is a valid HTTP POST from Dwolla.
            if( api.VerifyCallbackAuthenticity(receivedCallback) )
            {
                //Update the payment status in your database.

                if( receivedCallback.Status == DwollaCallbackStatus.Completed )
                {
                    //Payment was successful.
                }
                if( receivedCallback.Status == DwollaCallbackStatus.Failed)
                {
                    //Payment was not successful.
                }
            }
            else
            {
                //Log -- Possible URL tampering or trying to spoof their payment.
            }

        }

    }
}