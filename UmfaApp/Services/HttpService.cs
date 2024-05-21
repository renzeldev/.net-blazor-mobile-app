using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Web;

namespace UmfaApp.Services
{
    public abstract class HttpService
    {
        private readonly HttpClient _httpClient;
        
        protected HttpService(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(360)
            };
        }

        protected async Task<string> GetAsync(string endpoint, object? queryParams = null)
        {
            string queryString = queryParams != null ? ToQueryString(queryParams) : string.Empty;

            HttpResponseMessage response = await _httpClient.GetAsync(endpoint + queryString);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        protected async Task<string> PostAsync(string endpoint, object data, string mediaType = "application/json")
        {
            string jsonData = JsonSerializer.Serialize(data);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, mediaType);

            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        protected async Task<HttpResponseMessage> PostWithResponseAsync(string endpoint, object data, string mediaType = "application/json")
        {
            string jsonData = JsonSerializer.Serialize(data);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, mediaType);

            return await _httpClient.PostAsync(endpoint, content);
        }

        protected async Task<string> PutAsync(string endpoint, object data, string mediaType = "application/json")
        {
            string jsonData = JsonSerializer.Serialize(data);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, mediaType);

            HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        protected async Task<string> DeleteAsync(string endpoint, object data = null, string mediaType = "application/json")
        {
            if (data != null)
            {
                string jsonData = JsonSerializer.Serialize(data);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, mediaType);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
                {
                    Content = content
                };
                HttpResponseMessage response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        protected string ToQueryString(object queryParams)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            var properties = queryParams.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(queryParams, null);
                if (value != null)
                {
                    queryString[property.Name] = value.ToString();
                }
            }

            return "?" + queryString.ToString();
        }
    }
}
