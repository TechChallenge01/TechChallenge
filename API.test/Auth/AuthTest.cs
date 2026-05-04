using Application.Auth.DTOs.Requests;
using Org.BouncyCastle.Security;
using System.Net;
using System.Net.Http.Json;

namespace API.test.Auth
{
    public class AuthTest
    {
        [Fact]
        public async Task Auth_Post_Login_OK()
        {
            var client = new ApiWebApplicationFactory().CreateClient();

            var user = new LoginRequestDTO
            {
                Email = "Admin@email.com",
                Senha = "12345678"
            };

            var result = await client.PostAsJsonAsync("/api/auth/login", user);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
        [Fact]
        public async Task Auth_Post_Login_Unauthorized()
        {
            var client = new ApiWebApplicationFactory().CreateClient();

            var user = new LoginRequestDTO
            {
                Email = "tester@email.com",
                Senha = "32165465414230321"
            };

            var result = await client.PostAsJsonAsync("/api/auth/login", user);

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}
