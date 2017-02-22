using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace Dwolla.Checkout.Serialization
{
    public class JsonNetSerializer : ISerializer, IDeserializer
    {
        /// <summary>
        /// Default serializer
        /// </summary>
        public JsonNetSerializer()
        {
            ContentType = "application/json";
        }

        /// <summary>
        /// Serialize the object as JSON
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON as String</returns>
        public string Serialize( object obj )
        {
            return JsonConvert.SerializeObject( obj, this.RequestJsonSettings );
        }
        

        public JsonSerializerSettings RequestJsonSettings { get; set; } = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

        public JsonSerializerSettings ResponseJsonSettings { get; set; } = new JsonSerializerSettings();

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string RootElement { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Content type for serialized content
        /// </summary>
        public string ContentType { get; set; }

        public T Deserialize<T>(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content, this.ResponseJsonSettings);
        }
    }
}