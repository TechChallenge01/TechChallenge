using Application.Auth.DTOs.Requests;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace API.test
{
    public class IntegrationTestBase
    {
        protected string? _token;

        public async Task<AuthenticationHeaderValue> AuthenticateAsync()
        {
            var app = new ApiWebApplicationFactory();
            HttpClient _client = app.CreateClient();
            var loginRequest = new LoginRequestDTO
            {
                Email = "Admin@email.com",
                Senha = "12345678"
            };

            var result = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var dataElement))
            {
                if (dataElement.TryGetProperty("token", out var tokenElement))
                {
                    _token = tokenElement.GetString();
                }
            }

            return new AuthenticationHeaderValue("Bearer", _token);
        }
    }
}
