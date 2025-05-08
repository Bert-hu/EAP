using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EAP.Client.Utils
{
    public class HttpClientHelper
    {
        static HttpClientHelper()
        {

        }
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<T?> HttpPostRequestAsync<T>(string url, object body, int timeoutSeconds = 5)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                var jsonBody = JsonSerializer.Serialize(body);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                using var response = await _httpClient.PostAsync(url, content, cts.Token);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync(cts.Token);
                return JsonSerializer.Deserialize<T>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            //catch (TaskCanceledException)
            //{
            //    Console.WriteLine("Request timed out.");
            //    return default(T);
            //}
            catch (Exception ex)
            {
                //Console.WriteLine($"Request failed: {ex.Message}");
                //return default(T);
                throw ex;
            }
        }

    }
}