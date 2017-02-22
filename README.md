[![Build status](https://ci.appveyor.com/api/projects/status/ctkvbvvufolnhsjp?svg=true)](https://ci.appveyor.com/project/bchavez/dwolla)

Dwolla .NET/C# Library
======================
----------------------

Project Description
-------------------
A .NET implementation for the Dwolla Checkout (off-site gateway) API.

### License
* [MIT License](https://github.com/bchavez/Dwolla/blob/master/LICENSE)

### Requirements
* .NET 4.0

### Download & Install
via Nuget [Dwolla.Checkout](https://www.nuget.org/packages/Dwolla.Checkout/)

```

Install-Package Dwolla.Checkout

```

Building
--------
* Download the source code.
* Run `build.bat`.

Upon successful build, the results will be in the `\__package` directory.

Usage
-----
### Off-Site Gateway API
Dwolla allows two methods to initiate payment from a customer (off-site, without using OAuth).

* Initiate Payments by HTML Form Post.
* Initiate Payments by Server-to-Server Communication.

Implementers usually choose a single method to initiate payments depending on your development scenario.

Both methods provide an option to configure a **Callback URL**. The **Callback URL** is a location on your server where Dwolla will post a JSON object that describes the success or failure of a payment after the user has completed the checkout process.

----
#### Initiate Payments by HTML Form Post
When using the HTML Form method, use the `DwollaSignatureUtil.GenerateSignature()` to generate a `signature` for the form submission. Dwolla uses the form's `signature` value to verify the authenticity of the form submission.

```html
<form accept-charset="UTF-8" action="https://www.dwolla.com/payment/pay" method="post">
  <input id="key" name="key" type="hidden" value="myAppKey" />
  <input id="signature" name="signature" type="hidden" 
         value="<%= DwollaSignatureUtil.GenerateSignature("appKey", "appSecret",
                                                           orderId:"188375",
                                                           DateTime.UtcNow); %>" />
  <input id="callback" name="callback" type="hidden" 
        value="http://www.example.com/callback" />
  <input id="redirect" name="redirect" type="hidden" 
        value="http://www.example.com/redirect" />
  <input id="test" name="test" type="hidden" value="true" />
  <input id="name" name="name" type="hidden" value="Purchase" />
  <input id="description" name="description" type="hidden" value="Description" />
  <input id="destinationid" name="destinationid" type="hidden" value="812-111-1111" />
  <input id="amount" name="amount" type="hidden" value="1.00" />
  <input id="shipping" name="shipping" type="hidden" value="0.00" />
  <input id="tax" name="tax" type="hidden" value="0.00" />
  <input id="orderid" name="orderid" type="hidden" value="188375" />
  <input id="timestamp" name="timestamp" type="hidden" value="1323302400" />
	    
  <button type="submit">Submit Order</button>
</form>
```

----
#### Initiate Payments by Server-to-Server Checkout Request
The general process of communicating with Dwolla in a Server-to-Server Checkout process involves:

1. Creating a `DwollaCheckoutRequest`.
2. Sending the `DwollaCheckoutRequest` to Dwolla.
3. Parse the `DwollaCheckoutResponse` and deal with any errors.
4. Use the `DwollaCheckoutResponse` to generate a Redirect URL for the customer's browser to complete the checkout process.

```csharp
var api = new DwollaServerCheckoutApi( appKey:"...", appSecret: "..." );

//Create a checkout request
var checkoutRequest = new DwollaCheckoutRequest
              {
                OrderId = "MyOrderTest",
                Callback = new Uri("http://www.example.com/order-callback")
                PurchaseOrder = new DwollaPurchaseOrder
                           {
                            DestinationId = "812-111-1111",
                            OrderItems = { 
                               new DwollaOrderItem( name: "Candy Bar", price: 25.00m, quantity: 1 )
                               {
                                  Description = "Expensive Candy Bar",
                               }
                             },     
                           },
             };

//Send the request to Dwolla.
var checkoutResponse = api.SendCheckoutRequest( checkoutRequest );

if( checkoutResponse.Result == DwollaCheckoutResponseResult.Failure )
{
    //Handle Error
}
else if( checkoutResponse.Result == DwollaCheckoutResponseResult.Success)
{
    var redirectUrl = checkoutResponse.GetRedirectUrl();
    //Send HTTP Redirect to browser so the 
    //customer can finish the checkout process
}
```

-------
#### Handling Callbacks on Your Server
Regardless of the method chosen to initiate payments, if a **Callback URL** was specified in the request, Dwolla will "post back" a JSON object to the **Callback URL** after the user has completed the checkout. This callback usually happens ***asynchronously*** after the user's browser is ***redirected***.  It is good practice to verify the authenticity of the callback to ensure the callback hasn't been spoofed.

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
if( api.VerifyAuthenticity(receivedCallback) )
{
    //Update the payment status in your database.

    if( receivedCallback.Status == DwollaStatus.Completed )
    {
        //Payment was successful.
    }
    else if( receivedCallback.Status == DwollaStatus.Failed)
    {
        //Payment was not successful.
    }
}
else
{
    //Log -- Possible URL tampering or trying to spoof their payment.
}
```

-------
#### Handling Redirects on Your Server
Optionally, you can also handle the user's browser redirect from Dwolla too. Handling the URL parameters from the user's browser on an MVC controller is show below:

```csharp
var api = new DwollaServerCheckoutApi( appKey: "...", appSecret: "..." );

public ActionResult Handle_Dwolla_BrowserRedirect(DwollaRedirect redirect)
{
    if( api.VerifyAuthenticity( redirect ) )
    {
        if( redirect.Status == DwollaStatus.Completed )
        {
            //The User completed the transaction.
        }
        else
        {
            //The transaction failed.
        }
    }
    return new HttpStatusCodeResult( HttpStatusCode.BadRequest );
}
```

 

Reference
---------
* [Dwolla Off-Site Gateway API Documentation](http://developers.dwolla.com/dev/docs/gateway)


Created by [Brian Chavez](http://bchavez.bitarmory.com).

---

*Note: This application/third-party library is not directly supported by Dwolla Corp.  Dwolla Corp. makes no claims about this application/third-party library.  This application/third-party library is not endorsed or certified by Dwolla Corp.*
