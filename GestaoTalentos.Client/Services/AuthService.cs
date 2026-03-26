//using GestaoTalentos.Shared.DTOs;
//using System;
//using System.Net.Http.Json;
//using System.Threading.Tasks;

//namespace GestaoTalentos.Client.Services;

//public class AuthService(HttpClient http, LocalStorageService storage)
//{
//    private const string TokenKey = "auth_token";

//    public async Task<string?> GetTokenAsync() =>
//        await storage.GetItemAsync(TokenKey);

//    public async Task<bool> IsLoggedInAsync() =>
//        !string.IsNullOrWhiteSpace(await GetTokenAsync());

//    public async Task<string?> LoginAsync(string username, string password)
//    {
//        var response = await http.PostAsJsonAsync("/login", new UserLoginDto
//        {
//            Username = username,
//            Password = password
//        });

//        if (!response.IsSuccessStatusCode)
//            return null;

//        var data = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
//        if (string.IsNullOrWhiteSpace(data?.Token))
//            return null;

//        await storage.SetItemAsync(TokenKey, data.Token);
//        return data.Token;
//    }

//    public async Task<bool> RegisterAsync(string username, string password)
//    {
//        var response = await http.PostAsJsonAsync("/register", new UserRegisterDto
//        {
//            Username = username,
//            Password = password
//        });

//        return response.IsSuccessStatusCode;
//    }

//    public async Task LogoutAsync()
//    {
//        await storage.RemoveItemAsync(TokenKey);
//    }
//}