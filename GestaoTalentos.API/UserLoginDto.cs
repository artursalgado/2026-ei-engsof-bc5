using System;
namespace GestaoTalentos.Shared.DTOs;

public class UserLoginDto
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class UserRegisterDto
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResponseDto
{
    public string Token { get; set; } = "";
}