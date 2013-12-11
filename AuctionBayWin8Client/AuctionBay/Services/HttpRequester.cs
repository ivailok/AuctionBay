using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AuctionBay.Services
{
    public class HttpRequester
    {
        public static async Task<HttpResponseMessage> Post<T>(string url, T bodyContent, IDictionary<string, string> headers = null)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = new HttpMethod("POST");
            request.RequestUri = new Uri(url);

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            request.Content = new StringContent(await JsonConvert.SerializeObjectAsync(bodyContent));
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }

        public static async Task<HttpResponseMessage> Put<T>(string url, T bodyContent, IDictionary<string, string> headers = null)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = new HttpMethod("PUT");
            request.RequestUri = new Uri(url);

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            if (bodyContent != null)
            {
                request.Content = new StringContent(await JsonConvert.SerializeObjectAsync(bodyContent));
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }

        public static async Task<HttpResponseMessage> Get(string url, IDictionary<string, string> headers = null)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = new HttpMethod("GET");
            request.RequestUri = new Uri(url);

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
    }
}
