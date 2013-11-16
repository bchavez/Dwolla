using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dwolla.Gateway
{
    public static class DwollaSignatureUtil
    {
        public static string GenerateSignature( string appKey, string appSecret, string orderId, DateTime utcTimestamp )
        {
            var timestamp = UnixEpochTime( utcTimestamp );

            var signatureData = String.Format( "{0}&{1}&{2}", appKey, timestamp, orderId );

            return GetHMACSHA1InHex( appKey, signatureData );
        }

        public static bool VerifyCallbackSignature( string appSecret, string receivedCallbackSignature, string receivedCheckoutId, decimal receivedAmount )
        {
            var signatureData = String.Format( "{0}&{1}", receivedCheckoutId, receivedAmount );

            return receivedCallbackSignature == GetHMACSHA1InHex( appSecret, signatureData );
        }

        internal static string GetHMACSHA1InHex(string key, string data )
        {
            var hmacKey = Encoding.ASCII.GetBytes( key );

            var signatureStream = new MemoryStream( Encoding.ASCII.GetBytes( data ) );

            var hex = new HMACSHA1( hmacKey ).ComputeHash( signatureStream )
                .Aggregate( new StringBuilder(), ( sb, b ) => sb.AppendFormat( "{0:x2}", b ), sb => sb.ToString() );

            return hex;
        }

        public static long UnixEpochTime( DateTime utcTime )
        {
            var timeDifference = utcTime - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            return Convert.ToInt64( timeDifference.TotalSeconds );
        }
    }
}