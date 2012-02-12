using NBehave.Spec.NUnit;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Dwolla.Tests
{
    [TestFixture]
    public class JsonParsingTests
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

            o.Amount.ShouldEqual( 3.25m );
            o.OrderId.ShouldEqual( "A1B2C3" );
            o.TestMode.ShouldBeTrue();
            o.TransactionId.ShouldEqual( 1 );
            o.CheckoutId.ShouldEqual( "C3D4DC4F-5074-44CA-8639-B679D0A70803" );
            o.Status.ShouldEqual( DwollaCallbackStatus.Completed );
            o.Signature.ShouldEqual( "7f42ba58ff0d20486fdc2634745e8e7c92cb6321" );
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

            o.Result.ShouldEqual(DwollaCheckoutRequestResult.Success);
            o.CheckoutId.ShouldEqual( "C3D4DC4F-5074-44CA-8639-B679D0A70803" );
            o.Message.ShouldBeNull();
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

            o.Result.ShouldEqual(DwollaCheckoutRequestResult.Failure);
            o.CheckoutId.ShouldBeNull();
            o.Message.ShouldEqual( "Invalid total." );
        }
    }
}