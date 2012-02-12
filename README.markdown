Dwolla .NET/C# Library
======================
----------------------

Project Description
-------------------
A .NET implementation for the Dwolla API.

License
-------
LGPLv2.1 (GNU Lesser General Public License 2.1)


Usage
-----
### Off-Site Gateway API ###
#### Server-to-Server Checkout Request ####
```csharp
var api = new DwollaServerCheckoutApi( appKey:"...", appSecret: "..." );

//Create a checkout request
var checkoutRequest = new DwollaCheckoutRequest{
                             OrderId = "MyOrderTest",
                             Callback = new Uri("http://www.example.com/order-callback")
                             PurchaseOrder = new DwollaPurchaseOrder
                                                 {
                                                   DestinationId = "812-111-1111",
                                                   OrderItems = { 
                                                      new DwollaOrderItem
                                                      {
                                                         Description = "Expensive Candy Bar",
                                                         Name = "Candy Bar",
                                                         Price = 25.00m,
                                                         Quantity = 1,
                                                      }
                                                   },     
                                                 },
                     };

//Send the request to Dwolla.
var checkoutResponse = api.SendCheckoutRequest( checkoutRequest );

if( checkoutResponse.Result == DwollaCheckoutRequestResult.Failure )
{
    //Handle Error
}
else if( checkoutResponse.Result == DwollaCheckoutRequestResult.Success)
{
    var redirectUrl = api.GetCheckoutRedirectUrl( checkoutResponse );
    //Send HTTP Redirect to browser so the 
    //customer can finish the checkout process
}
```

#### Handling Callbacks ####
After the customer has completed the checkout process Dwolla will POST a JSON callback object to your Callback URL with the results of the payment. Here's how to handle callback communications from Dwolla.

```csharp
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
```

Reference
---------
* [Dwolla Off-Site Gateway API Documentation](https://www.dwolla.com/developers/offsitegateway)


Created by [Brian Chavez](http://bchavez.bitarmory.com).

---

*Note: This application/third-party library is not directly supported by Dwolla Corp.  Dwolla Corp. makes no claims about this application/third-party library.  This application/third-party library is not endorsed or certified by Dwolla Corp.*
