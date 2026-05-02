// Programa principal da API GestaoTalentos
// Configura e executa uma aplicação ASP.NET Core com APIs mínimas para gestão de utilizadores e perfis
// Inclui autenticação JWT, autorização por roles e acesso a PostgreSQL

using GestaoTalentos.API;
using GestaoTalentos.Domain;
using GestaoTalentos.Infrastructure;
using  GestaoTalentos.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

//
// ======================
// CONFIGURAÇÃO SERVIÇOS
// ======================
//

// JSON enums como string
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger
builder.Services.AddEndpointsApiExplorer();

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPerfilRepository, PerfilRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPaisRepository, PaisRepository>();

//
// ======================
// JWT CONFIG
// ======================
//

var jwtKey = builder.Configuration["Jwt:Key"] ?? "SUPER_SECRET_KEY_2026";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "GestaoTalentosApi";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.Name
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

//
// Middleware
//
app.UseAuthentication();
app.UseAuthorization();

//
// ======================
// AUTH ENDPOINTS
// ======================
//

// REGISTER
app.MapPost("/register", async (UserRegisterDto request, IUserRepository repo, IRoleRepository roleRepo) =>
{
    if (await repo.GetByUsernameAsync(request.Username) != null)
        return Results.Conflict("Username já existe.");

    var role = await roleRepo.GetByNomeAsync(request.RoleId.ToString());
    if (role == null)
        return Results.BadRequest("Role inválida.");

    var user = new User
    {
        Username = request.Username,
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        RoleId = role.Id
    };

    await repo.AddAsync(user);

    return Results.Created($"/users/{user.Id}", new { user.Id, user.Username });
});

// LOGIN
app.MapPost("/login", async (UserLoginDto request, IUserRepository repo) =>
{
    var user = await repo.GetByUsernameAsync(request.Username);

    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        return Results.Unauthorized();

    var token = JwtTokenHelper.GenerateToken(user, jwtKey, jwtIssuer);

    return Results.Ok(new { token });
});

//
// ======================
// PERFIS
// ======================
//

// LISTAR
app.MapGet("/perfis", async (IPerfilRepository repo) =>
{
    var perfis = await repo.GetAllAsync();
    return Results.Ok(perfis.Select(MapPerfilToDto));
})
.RequireAuthorization();

// GET BY ID
app.MapGet("/perfis/{id}", async (int id, IPerfilRepository repo) =>
{
    var perfil = await repo.GetByIdAsync(id);
    if (perfil == null) return Results.NotFound();

    return Results.Ok(MapPerfilToDto(perfil));
})
.RequireAuthorization();

// CREATE
app.MapPost("/perfis", async (PerfilCreateDto request, ClaimsPrincipal user, IPerfilRepository repo) =>
{
    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    if (string.IsNullOrWhiteSpace(request.Content))
        return Results.BadRequest("Content é obrigatório.");

    var perfil = new Perfil
    {
        OwnerId = userId,
        Content = request.Content,
        PaisId = request.PaisId,
        IsShared = request.IsShared,
        CreatedAt = DateTime.UtcNow
    };

    await repo.AddAsync(perfil);

    return Results.Created($"/perfis/{perfil.Id}", MapPerfilToDto(perfil));
})
.RequireAuthorization();

// UPDATE
app.MapPut("/perfis/{id}", async (int id, PerfilUpdateDto request, IPerfilRepository repo) =>
{
    var perfil = await repo.GetByIdAsync(id);
    if (perfil == null) return Results.NotFound();

    perfil.Content = request.Content;
    perfil.PaisId = request.PaisId;
    perfil.IsShared = request.IsShared;

    await repo.UpdateAsync(perfil);

    return Results.NoContent();
})
.RequireAuthorization();

// DELETE
app.MapDelete("/perfis/{id}", async (int id, IPerfilRepository repo) =>
{
    var perfil = await repo.GetByIdAsync(id);
    if (perfil == null) return Results.NotFound();

    await repo.DeleteAsync(id);

    return Results.NoContent();
})
.RequireAuthorization();

//
// ======================
// MAPPER DTO
// ======================
//

static object MapPerfilToDto(Perfil p) => new
{
    p.Id,
    p.OwnerId,
    p.Content,
    p.PaisId,
    p.IsShared,
    p.CreatedAt
};

app.Run();