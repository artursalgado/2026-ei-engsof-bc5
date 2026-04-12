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
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT gerado pelo endpoint /login."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuração do contexto da base de dados com PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;" 
        + "Port=5432;Database=gestaotalentos;Username=postgres;Password=postgres"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPerfilRepository, PerfilRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPaisRepository, PaisRepository>();

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
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("user", "usermanager", "admin"));
    options.AddPolicy("UserManagerPolicy", policy => policy.RequireRole("usermanager", "admin"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
});

var app = builder.Build();

// Configuração de middleware para desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Inicialização da base de dados e criação de utilizador admin se não existir
await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();
    
    var roleRepository = services.GetRequiredService<IRoleRepository>();
    //Criar roles se não existirem
    if (!await roleRepository.AnyAsync())
    {
        var roles = new List<Role>
        {
            new Role { Nome = "admin" },
            new Role { Nome = "usermanager" },
            new Role { Nome = "user" }
        };

        foreach (var role in roles)
        {
            await roleRepository.AddAsync(role);
        }
    }

    var userRepository = services.GetRequiredService<IUserRepository>();
    //Criar utilizador admin se não existir
    if (!await userRepository.AnyAsync())
    {
        // Procurar o Role Admin (IMPORTANTE)
        var adminRole = await roleRepository.GetByNomeAsync("admin");

        if (adminRole == null)
            throw new Exception("Role Admin não encontrada.");
        var role = await roleRepository.GetByNomeAsync("user");
        var admin = new User
        {
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
            RoleId = role.Id
        };
        await userRepository.AddAsync(admin);
    }
    
    var areaRepository = services.GetRequiredService<IAreaRepository>();
    //Criar areas se não existirem
    if (!await areaRepository.AnyAsync())
    {
        var areas = new List<Area>
        {
            new Area { Nome = "react" },
            new Area { Nome = "c++" },
            new Area { Nome = "timemanagement," }
        };

        foreach (var area in areas)
        {
            await areaRepository.AddAsync(area);
        }
    }
    var paisRepository = services.GetRequiredService<IPaisRepository>();
    //Criar paises se não existirem
    if (!await paisRepository.AnyAsync())
    {
        var paises = new List<Pais>
        {
            new Pais { Nome = "portugal" },
            new Pais { Nome = "inglaterra" },
            new Pais { Nome = "frança" }
        };

        foreach (var pais in paises)
        {
            await paisRepository.AddAsync(pais);
        }
    } 
}

// Endpoint para registro de novo utilizador
app.MapPost("/register", async (UserRegisterDto request, IUserRepository repo, 
    IRoleRepository roleRepository) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        return Results.BadRequest("Username e password são obrigatórios.");

    if (await repo.GetByUsernameAsync(request.Username) != null)
        return Results.Conflict("Username já existe.");
    
    var role = await roleRepository.GetByNomeAsync("user");
    var user = new User
    {
        Username = request.Username.Trim(),
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        RoleId = role.Id
    };

    await repo.AddAsync(user);
    return Results.Created($"/users/{user.Id}", new { user.Id, user.Username, user.RoleId });
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
    return current is null ? Results.NotFound() : Results.Ok(new
    {
        current.Id, current.Username, current.RoleId, current.Role
    });
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

// Endpoint para listar perfis visíveis para o utilizador
app.MapGet("/perfis", async (ClaimsPrincipal user, IPerfilRepository repo, IUserRepository userRepo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role.Nome == "user")
    {
        var perfis = await repo.GetVisibleForUserAsync(userId);
        return Results.Ok(perfis.Select(r => new PerfilDto(r.Id, r.OwnerId, r.Content, r.IsShared, r.CreatedAt)));
    }

    var all = await repo.GetAllAsync();
    return Results.Ok(all.Select(r => new PerfilDto(r.Id, r.OwnerId, r.Content, r.IsShared, r.CreatedAt)));
}).RequireAuthorization("UserPolicy");

// Endpoint para obter um perfil específico por ID
app.MapGet("/perfis/{id}", async (int id, ClaimsPrincipal user, IPerfilRepository repo, 
    IUserRepository userRepo) =>
{
    var rec = await repo.GetByIdAsync(id);
    if (rec == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role.Nome == "admin" || current.Role.Nome == "usermanager" || 
        rec.OwnerId == userId || rec.IsShared)
        return Results.Ok(new PerfilDto(rec.Id, rec.OwnerId, rec.Content, rec.IsShared, rec.CreatedAt));
    //falta tambem adicionar se o user for cliente e for apresentado
    return Results.Forbid();
}).RequireAuthorization("UserPolicy");

// Endpoint para criar um novo perfil
app.MapPost("/perfis", async (PerfilCreateDto request, ClaimsPrincipal user, 
    IPerfilRepository repo, IPaisRepository paisRepo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;

    if (string.IsNullOrWhiteSpace(request.Content))
        return Results.BadRequest("Content obrigatório.");

    // VALIDAÇÃO Importante
    var pais = await paisRepo.GetByIdAsync(request.PaisId);
    if (pais == null)
        return Results.BadRequest("Pais inválido.");

    var perfil = new Perfil
    {
        OwnerId = userId,
        Content = request.Content.Trim(),
        IsShared = request.IsShared,
        PaisId = request.PaisId
    };

    await repo.AddAsync(perfil);

    return Results.Created(
        $"/perfis/{perfil.Id}",
        new PerfilDto(perfil.Id, perfil.OwnerId, perfil.Content, perfil.IsShared, perfil.CreatedAt)
    );
}).RequireAuthorization("UserPolicy");

// Endpoint para atualizar um perfil
app.MapPut("/perfis/{id}", async (int id, PerfilUpdateDto request, ClaimsPrincipal user,
    IPerfilRepository repo, IUserRepository userRepo, IPaisRepository paisRepo) =>
{
    var rec = await repo.GetByIdAsync(id);
    if (rec == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role.Nome != "admin" && current.Role.Nome != "usermanager" && rec.OwnerId != userId)
        return Results.Forbid();

    rec.Content = request.Content.Trim();
    rec.IsShared = request.IsShared;
    var pais = await paisRepo.GetByIdAsync(request.PaisId);
    if (pais == null)
        return Results.BadRequest("Pais inválido.");
    rec.PaisId = request.PaisId;
    await repo.UpdateAsync(rec);
    return Results.NoContent();
}).RequireAuthorization("UserPolicy");

// Endpoint para deletar um perfil
app.MapDelete("/perfis/{id}", async (int id, ClaimsPrincipal user, IPerfilRepository repo, IUserRepository userRepo) =>
{
    var rec = await repo.GetByIdAsync(id);
    if (rec == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role.Nome != "admin" && current.Role.Nome != "usermanager" && rec.OwnerId != userId)
        return Results.Forbid();

    await repo.DeleteAsync(id);
    return Results.NoContent();
}).RequireAuthorization("UserPolicy");

// Endpoint para listar todas as areas
app.MapGet("/areas", async (IAreaRepository repo) =>
    Results.Ok(await repo.GetAllAsync())).RequireAuthorization("UserPolicy");

// Endpoint para obter uma area específico por nome
app.MapGet("/areas/{nome}", async (string nome, IAreaRepository repo) =>
{
    var area = await repo.GetByNomeAsync(nome);
    return area is null ? Results.NotFound() : Results.Ok(area);
}).RequireAuthorization("UserPolicy");

// Endpoint para criar uma nova area (define Id e DtCriação automaticamente)
app.MapPost("/area", async (AreaCreateDto request, ClaimsPrincipal user, IAreaRepository repo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;

    if (string.IsNullOrWhiteSpace(request.Nome))
        return Results.BadRequest("Nome da area é obrigatório.");

    var area = new Area
    {
        Nome = request.Nome.Trim().ToLower(),
    };
    await repo.AddAsync(area);
    return Results.Created($"/areas/{area.Nome}", new AreaDto(area.Id, area.Nome, area.CreadoEm));
}).RequireAuthorization("AdminPolicy");

// Endpoint para eleminar uma area
app.MapDelete("/areas/{nome}", async (string nome, ClaimsPrincipal user, IAreaRepository repo,
        IUserRepository userRepo) =>
    {
        // 1. Encontrar área pelo nome
        var nomeNormalizado = nome.Trim().ToLower();
        var area = await repo.GetByNomeAsync(nomeNormalizado);
        if (area == null)
            return Results.NotFound();
        // 2. Obter utilizador
        var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
        var current = await userRepo.GetByIdAsync(userId);
        if (current == null)
            return Results.Unauthorized();
        // 3. Verificar admin
        if (current.Role.Nome != "admin")
            return Results.Forbid();
        // 4. Verificar se tem skills
        if (area.Skills != null && area.Skills.Any())
            return Results.Conflict("Área não pode ser eliminada porque tem skills associadas.");
        // 5. Remover
        await repo.DeleteAsync(area.Nome);

        return Results.NoContent();
    })
    .RequireAuthorization("AdminPolicy");

// Endpoint para listar todos os roles
app.MapGet("/roles", async (IRoleRepository repo) =>
    Results.Ok(await repo.GetAllAsync())).RequireAuthorization("UserPolicy");

// Endpoint para obter um role específico por nome
app.MapGet("/roles/{nome}", async (string nome, IRoleRepository repo) =>
{
    var role = await repo.GetByNomeAsync(nome);
    return role is null ? Results.NotFound() : Results.Ok(role);
}).RequireAuthorization("UserPolicy");

// Endpoint para criar um novo role (define Id e DtCriação automaticamente)
app.MapPost("/role", async (RoleCreateDto request, ClaimsPrincipal user, IRoleRepository repo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;

    if (string.IsNullOrWhiteSpace(request.Nome))
        return Results.BadRequest("Nome do role é obrigatório.");

    var role = new Role
    {
        Nome = request.Nome.Trim().ToLower(),
    };
    await repo.AddAsync(role);
    return Results.Created($"/areas/{role.Nome}", new RoleDto(role.Id, role.Nome, role.CreadoEm));
}).RequireAuthorization("AdminPolicy");

// Endpoint para eleminar um role
app.MapDelete("/roles/{nome}", async (string nome, ClaimsPrincipal user, IRoleRepository repo,
        IUserRepository userRepo) =>
    {
        // 1. Encontrar role pelo nome
        var nomeNormalizado = nome.Trim().ToLower();
        var role = await repo.GetByNomeAsync(nomeNormalizado);
        if (role == null)
            return Results.NotFound();
        // 2. Obter utilizador
        var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
        var current = await userRepo.GetByIdAsync(userId);
        if (current == null)
            return Results.Unauthorized();
        // 3. Verificar admin
        if (current.Role.Nome != "admin")
            return Results.Forbid();
        // 4. Verificar se tem users
        if (role.Users != null && role.Users.Any())
            return Results.Conflict("Role não pode ser eliminado porque tem users associados.");
        // 5. Remover
        await repo.DeleteAsync(role.Nome);

        return Results.NoContent();
    })
    .RequireAuthorization("AdminPolicy");

// Endpoint para listar todos os paises
app.MapGet("/paises", async (IPaisRepository repo) =>
    Results.Ok(await repo.GetAllAsync())).RequireAuthorization("UserPolicy");

// Endpoint para obter um pais específico por nome
app.MapGet("/paises/{nome}", async (string nome, IPaisRepository repo) =>
{
    var pais = await repo.GetByNomeAsync(nome);
    return pais is null ? Results.NotFound() : Results.Ok(pais);
}).RequireAuthorization("UserPolicy");

// Endpoint para criar um novo pais (define Id e DtCriação automaticamente)
app.MapPost("/pais", async (PaisCreateDto request, ClaimsPrincipal user, IPaisRepository repo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;

    if (string.IsNullOrWhiteSpace(request.Nome))
        return Results.BadRequest("Nome do pais é obrigatório.");

    var pais = new Pais
    {
        Nome = request.Nome.Trim().ToLower(),
    };
    await repo.AddAsync(pais);
    return Results.Created($"/paises/{pais.Nome}", new PaisDto(pais.Id, pais.Nome, pais.CreadoEm));
}).RequireAuthorization("AdminPolicy");

// Endpoint para eleminar um pais
app.MapDelete("/paises/{nome}", async (string nome, ClaimsPrincipal user, IPaisRepository repo,
        IUserRepository userRepo) =>
    {
        // 1. Encontrar pais pelo nome
        var nomeNormalizado = nome.Trim().ToLower();
        var pais = await repo.GetByNomeAsync(nomeNormalizado);
        if (pais == null)
            return Results.NotFound();
        // 2. Obter utilizador
        var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
        var current = await userRepo.GetByIdAsync(userId);
        if (current == null)
            return Results.Unauthorized();
        // 3. Verificar admin
        if (current.Role.Nome != "admin")
            return Results.Forbid();
        // 4. Verificar se tem perfis
        if (pais.Perfis != null && pais.Perfis.Any())
            return Results.Conflict("Pais não pode ser eliminado porque tem perfis associados.");
        // 5. Remover
        await repo.DeleteAsync(pais.Nome);

        return Results.NoContent();
    })
    .RequireAuthorization("AdminPolicy");



app.Run();
