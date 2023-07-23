using GamerVII.MinecraftLauncher.Models.User;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GamerVII.MinecraftLauncher.Services.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost")
        };
    }

    public async Task<IUser> OnLogin(string login, string password)
    {
        IUser user = new User {
        
            Login = login,
            Password = password
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress!.AbsoluteUri}/auth.php");

        var content = new MultipartFormDataContent();
        content.Add(new StringContent(user.Login), "login");
        content.Add(new StringContent(user.Password), "password");

        request.Content = content;
        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        user.IsLogin = result == "success";

        return user;


    }
}
