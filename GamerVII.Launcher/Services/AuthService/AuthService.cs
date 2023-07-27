using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.LocalStorage;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Splat;

namespace GamerVII.Launcher.Services.AuthService;


public class AuthService : IAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;

    public AuthService(ILocalStorageService localStorage = null)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost")
        };

        _localStorage = localStorage ?? Locator.Current.GetService<ILocalStorageService>()!;
    }

    public async Task<IUser> OnLogin(string login, string password)
    {
        IUser user = new User
        {
            Login = login,
            Password = password
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress!.AbsoluteUri}/auth.php");

        var content = new MultipartFormDataContent();
        content.Add(new StringContent(user.Login), "login");
        content.Add(new StringContent(user.Password), "password");

        request.Content = content;
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var result = await response.Content.ReadAsStringAsync();
            user.IsLogin = true;
            user.AccessToken = result;
            string refreshToken = response.Headers.FirstOrDefault(c => c.Key == "Refresh-Token").Value.FirstOrDefault() ?? string.Empty;

            await _localStorage.SetAsync("RefreshToken", refreshToken);
            await _localStorage.SetAsync("User", user);
        }

        return user;

    }

    public async Task OnLogout()
    {
        await _localStorage.SetAsync("RefreshToken", string.Empty);
        await _localStorage.SetAsync("User", string.Empty);
    }

    public async Task<IUser?> GetAuthorizedUser()
    {
        var user = await _localStorage.GetAsync<User>("User") ?? new User{ Login = string.Empty, Password = string.Empty};

        try
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken securityToken = tokenHandler.ReadToken(user.AccessToken);

            if (securityToken is JwtSecurityToken jwtSecurityToken)
            {
                var expirationDateString = jwtSecurityToken?.Claims?.FirstOrDefault(c => c.Type == "Expiration")?.Value.ToString();

                if (!string.IsNullOrEmpty(expirationDateString) && DateTime.TryParse(expirationDateString, out DateTime expirationDate))
                {
                    if (expirationDate < DateTime.Now)
                    {
                        user.IsLogin = false;
                    }
                }

            }
        }
        catch (Exception)
        {
            user.IsLogin = false;
        }

        return user;
    }

    public string Encode(string input, byte[] key)
    {
        HMACSHA256 myhmacsha = new HMACSHA256(key);
        byte[] byteArray = Encoding.UTF8.GetBytes(input);
        MemoryStream stream = new MemoryStream(byteArray);
        byte[] hashValue = myhmacsha.ComputeHash(stream);
        return Base64UrlEncoder.Encode(hashValue);
    }
}
