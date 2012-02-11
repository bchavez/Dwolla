using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace Dwolla.Tests
{
    [TestFixture]
    public class DwollaTests
    {
        [TestFixtureSetUp]
        public void BeforeRunningTestSession()
        {

        }

        [TestFixtureTearDown]
        public void AfterRunningTestSession()
        {

        }


        [SetUp]
        public void BeforeEachTest()
        {

        }

        [TearDown]
        public void AfterEachTest()
        {

        }


        [Test]
        public void example_signature_should_match_api_signature()
        {
            var timestamp = DateTime.UtcNow;
            var hmacExample = DwollaExamples.HmacSingatureExample( "test", "test", DwollaSignatureUtil.UnixEpochTime( timestamp ), "1" );
            var apiExample = DwollaSignatureUtil.GenerateSignature( "test", "test", "1", timestamp );

            hmacExample.ShouldEqual( apiExample );
        }
    }

    public class DwollaExamples
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
}
