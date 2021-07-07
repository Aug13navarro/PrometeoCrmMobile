using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Services.Exceptions;
using Newtonsoft.Json;

namespace Core.Services.Utils
{
    public static class HttpClientExtensionMethods
    {
        public static async Task<T> SendAsyncAs<T>(this HttpClient client, HttpRequestMessage request)
        {
            using (HttpResponseMessage response = await client.SendAsync(request))
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            {
                if (response.IsSuccessStatusCode)
                {
                    return DeserializeJsonFromStream<T>(stream);
                }

                string content = await StreamToStringAsync(stream);
                throw new ServiceException(content);
            }
        }

        public static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
            {
                return default(T);
            }

            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                var result = serializer.Deserialize<T>(jsonTextReader);
                return result;
            }
        }

        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
            {
                using (var streamReader = new StreamReader(stream))
                {
                    content = await streamReader.ReadToEndAsync();
                }
            }

            return content;
        }
    }
}
