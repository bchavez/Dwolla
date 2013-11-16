using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dwolla.Checkout;
using FluentAssertions;
using NUnit.Framework;

namespace Dwolla.Tests
{
    [TestFixture]
    public class DwollaSignatureTests
    {
        public class DocumentationExample
        {
            public static string HmacSingatureExample( string consumerKey, string consumerSecret, long timestamp, string orderId )
            {
                string textToEncode = consumerKey + "&" + timestamp + "&" + orderId;
                byte[] hmacKey = Encoding.ASCII.GetBytes( consumerSecret );
                HMACSHA1 myhmacsha1 = new HMACSHA1( hmacKey );
                byte[] byteArray = Encoding.ASCII.GetBytes( textToEncode );
                MemoryStream stream = new MemoryStream( byteArray );

                string signature = myhmacsha1.ComputeHash( stream ).Aggregate( "", ( s, e ) => s + String.Format( "{0:x2}", e ), s => s );
                return signature;
            }
        }

        [Test]
        public void example_signature_should_match_api_signature()
        {
            var timestamp = DateTime.UtcNow;
            var hmacExample = DocumentationExample.HmacSingatureExample( "test", "test", DwollaSignatureUtil.UnixEpochTime( timestamp ), "1" );
            var apiExample = DwollaSignatureUtil.GenerateSignature( "test", "test", "1", timestamp );

            hmacExample.Should().Be( apiExample );
        }

        [Test]
        public void can_verify_callback_signature()
        {
            //appSecret = "test"
            var c = new DwollaCallback
                        {
                            Amount = 3.25m,
                            CheckoutId = "C3D4DC4F-5074-44CA-8639-B679D0A70803",
                            Signature = "7f42ba58ff0d20486fdc2634745e8e7c92cb6321"
                        };

            DwollaSignatureUtil.VerifyCallbackSignature( "test", c.Signature, c.CheckoutId, c.Amount )
                .Should().BeTrue();
        }
    }

}
