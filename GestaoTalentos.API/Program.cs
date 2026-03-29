// Programa principal da API GestaoTalentos
// Configura e executa uma aplicação ASP.NET Core com APIs mínimas para gestão de
// utilizadores, registros, clientes e apresentações.
// Inclui autenticação JWT, autorização baseada em roles e acesso à base de dados PostgreSQL.

using GestaoTalentos.API;
using GestaoTalentos.Domain;
using GestaoTalentos.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do contexto da base de dados com PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                      "Host=localhost;Port=5432;Database=gestaotalentos;Username=postgres;Password=postgres"));

// Registro de repositórios para injeção de dependência
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRecordRepository, RecordRepository>();
//----------------------
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
//builder.Services.AddScoped<IApresentacaoRepository, ApresentacaoRepository>();
//------------------------
// Configuração de autenticação JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "MudaIstoParaSegredoMuitoForte#2026";
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Configuração de políticas de autorização baseadas em roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy => policy.RequireRole(UserRole.User.ToString(), UserRole.UserManager.ToString(), UserRole.Admin.ToString()));
    options.AddPolicy("UserManagerPolicy", policy => policy.RequireRole(UserRole.UserManager.ToString(), UserRole.Admin.ToString()));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole(UserRole.Admin.ToString()));
});

// erro cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientCors", policy =>
        policy.WithOrigins("http://localhost:5025", "https://localhost:5025")
            .AllowAnyHeader()
            .AllowAnyMethod());
});


var app = builder.Build();

// Configuração de middleware para desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ClientCors");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Inicialização da base de dados e criação de utilizador admin se não existir
await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();

    var userRepository = services.GetRequiredService<IUserRepository>();
    if (!await userRepository.AnyAsync())
    {
        var admin = new User
        {
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = UserRole.Admin
        };
        await userRepository.AddAsync(admin);
    }
}

// Endpoint para registro de novo utilizador
app.MapPost("/register", async (UserRegisterDto request, IUserRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        return Results.BadRequest("Username e password são obrigatórios.");

    if (await repo.GetByUsernameAsync(request.Username) != null)
        return Results.Conflict("Username já existe.");

    var user = new User
    {
        Username = request.Username.Trim(),
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        Role = UserRole.User
    };

    await repo.AddAsync(user);
    return Results.Created($"/users/{user.Id}", new { user.Id, user.Username, user.Role });
});

// Endpoint para login e geração de token JWT
app.MapPost("/login", async (UserLoginDto request, IUserRepository repo) =>
{
    var user = await repo.GetByUsernameAsync(request.Username);
    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        return Results.Unauthorized();

    var token = JwtTokenHelper.GenerateToken(user, jwtKey, jwtIssuer);
    return Results.Ok(new { token });
});

// Endpoint para obter informações do utilizador logado
app.MapGet("/users/me", async (ClaimsPrincipal user, IUserRepository repo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    var current = await repo.GetByIdAsync(userId);
    return current is null ? Results.NotFound() : Results.Ok(new { current.Id, current.Username, current.Role });
}).RequireAuthorization();

// Endpoint para listar todos os utilizador (apenas UserManager e Admin)
app.MapGet("/users", async (IUserRepository repo) =>
    Results.Ok(await repo.GetAllAsync())).RequireAuthorization("UserManagerPolicy");

// Endpoint para alterar a role de um utilizador (apenas UserManager e Admin)
app.MapPut("/users/{id}/role", async (int id, UserRoleUpdateDto request, IUserRepository repo) =>
{
    if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        return Results.BadRequest("Role inválida (User, UserManager, Admin).");

    var user = await repo.GetByIdAsync(id);
    if (user == null)
        return Results.NotFound();

    user.Role = role;
    await repo.UpdateAsync(user);
    return Results.NoContent();
}).RequireAuthorization("UserManagerPolicy");

// Endpoint para criar um novo utilizador (apenas UserManager e Admin)
app.MapPost("/users", async (UserCreateDto request, IUserRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        return Results.BadRequest("Username e password são obrigatórios.");

    if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        return Results.BadRequest("Role inválida (User, UserManager, Admin).");

    if (await repo.GetByUsernameAsync(request.Username) != null)
        return Results.Conflict("Username já existe.");

    var user = new User
    {
        Username = request.Username.Trim(),
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        Role = role
    };

    await repo.AddAsync(user);
    return Results.Created($"/users/{user.Id}", new { user.Id, user.Username, user.Role });
}).RequireAuthorization("UserManagerPolicy");

// Endpoint para listar registros (records) visíveis para o utilizador
app.MapGet("/records", async (ClaimsPrincipal user, IRecordRepository repo, IUserRepository userRepo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role == UserRole.User)
    {
        var records = await repo.GetVisibleForUserAsync(userId);
        return Results.Ok(records.Select(r => new RecordDto(r.Id, r.OwnerId, r.Content, r.IsShared, r.CreatedAt)));
    }

    var all = await repo.GetAllAsync();
    return Results.Ok(all.Select(r => new RecordDto(r.Id, r.OwnerId, r.Content, r.IsShared, r.CreatedAt)));
}).RequireAuthorization("UserPolicy");

// Endpoint para obter um registro específico por ID
app.MapGet("/records/{id}", async (int id, ClaimsPrincipal user, IRecordRepository repo, IUserRepository userRepo) =>
{
    var rec = await repo.GetByIdAsync(id);
    if (rec == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role == UserRole.Admin || current.Role == UserRole.UserManager || rec.OwnerId == userId || rec.IsShared)
        return Results.Ok(new RecordDto(rec.Id, rec.OwnerId, rec.Content, rec.IsShared, rec.CreatedAt));

    return Results.Forbid();
}).RequireAuthorization("UserPolicy");

// Endpoint para criar um novo registro
app.MapPost("/records", async (RecordCreateDto request, ClaimsPrincipal user, IRecordRepository repo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;

    if (string.IsNullOrWhiteSpace(request.Content))
        return Results.BadRequest("Content obrigatório.");

    var record = new Record { OwnerId = userId, Content = request.Content.Trim(), IsShared = request.IsShared };
    await repo.AddAsync(record);
    return Results.Created($"/records/{record.Id}", new RecordDto(record.Id, record.OwnerId, record.Content, record.IsShared, record.CreatedAt));
}).RequireAuthorization("UserPolicy");

// Endpoint para atualizar um registro
app.MapPut("/records/{id}", async (int id, RecordUpdateDto request, ClaimsPrincipal user, IRecordRepository repo, IUserRepository userRepo) =>
{
    var rec = await repo.GetByIdAsync(id);
    if (rec == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role != UserRole.Admin && current.Role != UserRole.UserManager && rec.OwnerId != userId)
        return Results.Forbid();

    rec.Content = request.Content.Trim();
    rec.IsShared = request.IsShared;
    await repo.UpdateAsync(rec);
    return Results.NoContent();
}).RequireAuthorization("UserPolicy");

// Endpoint para deletar um registro
app.MapDelete("/records/{id}", async (int id, ClaimsPrincipal user, IRecordRepository repo, IUserRepository userRepo) =>
{
    var rec = await repo.GetByIdAsync(id);
    if (rec == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role != UserRole.Admin && current.Role != UserRole.UserManager && rec.OwnerId != userId)
        return Results.Forbid();

    await repo.DeleteAsync(id);
    return Results.NoContent();
}).RequireAuthorization("UserPolicy");

// Endpoint para listar todos os clientes
app.MapGet("/clientes", async (IClienteRepository repo) =>
    Results.Ok(await repo.GetAllAsync())).RequireAuthorization("UserPolicy");

// Endpoint para obter um cliente específico por ID
app.MapGet("/clientes/{id}", async (int id, IClienteRepository repo) =>
{
    var cliente = await repo.GetByIdAsync(id);
    return cliente is null ? Results.NotFound() : Results.Ok(cliente);
}).RequireAuthorization("UserPolicy");

// Endpoint para criar um novo cliente (define IdCriador automaticamente)
app.MapPost("/clientes", async (ClienteCreateDto request, ClaimsPrincipal user, IClienteRepository repo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;

    if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email))
        return Results.BadRequest("Nome e email são obrigatórios.");

    var cliente = new Cliente
    {
        Nome = request.Nome.Trim(),
        Email = request.Email.Trim(),
        IdCriador = userId,
        IdMinhaConta = request.IdMinhaConta
    };
    await repo.AddAsync(cliente);
    return Results.Created($"/clientes/{cliente.Id}", new ClienteDto(cliente.Id, cliente.Nome, cliente.Email, cliente.IdCriador, cliente.IdMinhaConta));
}).RequireAuthorization("UserPolicy");

// Endpoint para atualizar um cliente
app.MapPut("/clientes/{id}", async (int id, ClienteCreateDto request, ClaimsPrincipal user, IClienteRepository repo, IUserRepository userRepo) =>
{
    var cliente = await repo.GetByIdAsync(id);
    if (cliente == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role != UserRole.Admin && current.Role != UserRole.UserManager && cliente.IdCriador != userId)
        return Results.Forbid();

    cliente.Nome = request.Nome.Trim();
    cliente.Email = request.Email.Trim();
    cliente.IdMinhaConta = request.IdMinhaConta;
    await repo.UpdateAsync(cliente);
    return Results.NoContent();
}).RequireAuthorization("UserPolicy");








// Endpoint para listar todas as apresentações
//app.MapGet("/apresentacoes", async (IApresentacaoRepository repo) =>
//  Results.Ok(await repo.GetAllAsync())).RequireAuthorization("UserPolicy");!!!!!!!!!!!!!!!!

// Endpoint para obter uma aprentação específica por ID de Cliente e ID de Talento
//app.MapGet("/apresentacoes/{idCliente}", async (int idc, IApresentacaoRepository repo) =>
//{
//  var apresentacao = await repo.GetByIdAsync(idc);
//  return apresentacao is null ? Results.NotFound() : Results.Ok(apresentacao);
//}).RequireAuthorization("UserPolicy");!!!!!!!!!!!!!!!!!

// Endpoint para criar um nova aprentação (define IdCriador automaticamente)
//app.MapPost("/apresentacoes", async (ApresentacaoCreateDto request, ClaimsPrincipal user, IApresentacaoRepository repo) =>
//{
//  if (string.IsNullOrWhiteSpace(request.IdCliente))
    //      return Results.BadRequest("IdCliente é obrigatório.");

//    var apresentacao = new Apresentacao
    //  {
    //  idCliente = request.IdCliente.Trim(),
    //  DataApresentacao = system.DateTime.UtcNow
    //};
    //await repo.AddAsync(cliente);
    //return Results.Created($"/apresentacao/{apresentacao.id}", new ApresentacaoDto(apresentacao.idClinte, apresentacao.DataApresentacao));
//}).RequireAuthorization("UserPolicy");

// Endpoint para atualizar um cliente
//app.MapPut("/clientes/{id}", async (int id, ApresentacaoCreateDto request, ClaimsPrincipal user, IApresentacaoRepository repo, IUserRepository userRepo) =>
//{
//  var apresentacao = await repo.GetByIdClienteAsync(id);
//  if (apresentacao == null) return Results.NotFound();

//    apresentacao.idClinte = request.IdCliente.Trim();
//  DataApresentacao = request.DataApresentacao.Trim();
//  await repo.UpdateAsync(apresentacao);
//  return Results.NoContent();
//}).RequireAuthorization("UserPolicy");

app.Run();
