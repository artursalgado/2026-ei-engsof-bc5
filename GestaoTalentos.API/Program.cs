using GestaoTalentos.API;
using GestaoTalentos.API.Services;
using GestaoTalentos.Domain;
using GestaoTalentos.Shared;
using GestaoTalentos.Infrastructure;
using GestaoTalentos.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5025", "https://localhost:5025")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Port=5432;Database=gestaotalentos;Username=postgres;Password=postgres123"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
//builder.Services.AddScoped<IPerfilRepository, PerfilRepository>();
builder.Services.AddScoped<GestaoTalentos.Infrastructure.IPerfilRepository, PerfilRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddScoped<ITalentoElegivelRepository, TalentoElegivelRepository>();
builder.Services.AddScoped<PropostaMatchingService>();

// Services
builder.Services.AddScoped<IPropostaService, PropostaService>();
builder.Services.AddScoped<ITalentoElegiveService, TalentoElegiveService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "MudaIstoParaSegredoMuitoForte#2026";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "GestaoTalentosApi";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = "role",
            NameClaimType = "name"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy => policy.RequireClaim("role", "User", "UserManager", "Admin"));
    options.AddPolicy("UserManagerPolicy", policy => policy.RequireClaim("role", "UserManager", "Admin"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("role", "Admin"));
});

//

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
// app.UseHttpsRedirection(); // Comentado para nao perder token em redirecionamentos locais
app.UseAuthentication();
app.UseAuthorization();

await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    // await context.Database.EnsureCreatedAsync();
    await context.Database.MigrateAsync();
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

app.MapPost("/register", async (GestaoTalentos.API.UserRegisterDto request, IUserRepository repo, IClienteRepository clienteRepo) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        return Results.BadRequest("Username e password são obrigatórios.");

    if (await repo.GetByUsernameAsync(request.Username) != null)
        return Results.Conflict("Username já existe.");

    var user = new User
    {
        Username = request.Username.Trim(),
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        Role = UserRole.User,
        TipoUtilizador = request.TipoUtilizador
    };

    await repo.AddAsync(user);

    // Se se registar como Cliente, criar automaticamente o registo na tabela Clientes
    if (request.TipoUtilizador == TipoUtilizador.Cliente)
    {
        var cliente = new Cliente
        {
            Nome = user.Username,
            Email = "",
            IdCriador = user.Id,
            IdMinhaConta = user.Id
        };
        await clienteRepo.AddAsync(cliente);
    }

    return Results.Created($"/users/{user.Id}", new { user.Id, user.Username, user.Role });
});

app.MapPost("/login", async (GestaoTalentos.API.UserLoginDto request, IUserRepository repo) =>
{
    var user = await repo.GetByUsernameAsync(request.Username);
    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        return Results.Unauthorized();

    var token = JwtTokenHelper.GenerateToken(user, jwtKey, jwtIssuer);
    return Results.Ok(new { token });
});

app.MapGet("/users/me", async (ClaimsPrincipal user, IUserRepository repo) =>
{
    var userId = int.TryParse(user.FindFirstValue("sub"), out var id) ? id : 0;
    var current = await repo.GetByIdAsync(userId);
    return current is null ? Results.NotFound() : Results.Ok(new { current.Id, current.Username, current.Role, current.TipoUtilizador });
}).RequireAuthorization();

app.MapGet("/users", async (IUserRepository repo) =>
    Results.Ok(await repo.GetAllAsync())).RequireAuthorization("UserManagerPolicy");

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
    await repo.SaveChangesAsync(); // NOVA LINHA
    return Results.Created($"/users/{user.Id}", new { user.Id, user.Username, user.Role });
}).RequireAuthorization("UserManagerPolicy");

app.MapGet("/perfis", async (ClaimsPrincipal user, GestaoTalentos.Infrastructure.IPerfilRepository repo, IUserRepository userRepo) =>
{
    var userIdStr = user.FindFirstValue("sub");
    var userId = int.TryParse(userIdStr, out var id) ? id : 0;

    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();
    //--
    IEnumerable<Perfil> perfis;
    if (current.Role == UserRole.User && current.TipoUtilizador == TipoUtilizador.Cliente)
        perfis = await repo.GetPublicAsync();
    else if (current.Role == UserRole.User)
        perfis = await repo.GetByOwnerAsync(userId);
    else
        perfis = await repo.GetAllAsync();
    //--

    return Results.Ok(perfis.Select(p => MapPerfilToDto(p)));
}).RequireAuthorization("UserPolicy");


app.MapGet("/perfis/empresas-sugestoes", async (AppDbContext context) =>
{
    var empresas = await context.ExperienciasProfissionais
        .Select(e => e.Empresa)
        .Distinct()
        .OrderBy(n => n)
        .ToListAsync();
    return Results.Ok(empresas);
}).RequireAuthorization("UserPolicy");

app.MapGet("/perfis/{id}", async (int id, ClaimsPrincipal user, GestaoTalentos.Infrastructure.IPerfilRepository repo, IUserRepository userRepo) =>
{
    var perfil = await repo.GetByIdAsync(id);
    if (perfil == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue("sub"), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role == UserRole.Admin || current.Role == UserRole.UserManager || perfil.OwnerId == userId || perfil.IsShared)
        return Results.Ok(MapPerfilToDto(perfil));

    return Results.Forbid();
}).RequireAuthorization("UserPolicy");

app.MapPost("/perfis", async (PerfilCreateDto request, ClaimsPrincipal user, GestaoTalentos.Infrastructure.IPerfilRepository repo) =>
{
    var userId = int.TryParse(user.FindFirstValue("sub"), out var uid) ? uid : 0;

    // Validações básicas
    if (string.IsNullOrWhiteSpace(request.Nome))
        return Results.BadRequest("Nome é obrigatório.");
    if (string.IsNullOrWhiteSpace(request.Email))
        return Results.BadRequest("Email é obrigatório.");
    if (string.IsNullOrWhiteSpace(request.Pais))
        return Results.BadRequest("País é obrigatório.");
    if (request.PrecoPorHora <= 0)
        return Results.BadRequest("Preço por hora deve ser maior que zero.");

    // Validação de intervalo de datas (Ano Fim >= Ano Inicio)
    foreach (var exp in request.Experiencias)
    {
        if (exp.AnoFim.HasValue && exp.AnoFim < exp.AnoInicio)
            return Results.BadRequest($"Na empresa '{exp.Empresa}', o ano de fim ({exp.AnoFim}) não pode ser anterior ao início ({exp.AnoInicio}).");
    }

    // Validação de sobreposição de datas
    var erroData = ValidarSobreposicaoExperiencias(request.Experiencias);
    if (erroData != null) return Results.BadRequest(erroData);

    if (request.Skills.Any(s => s.AnosExperiencia < 1))
        return Results.BadRequest("As skills devem ter pelo menos 1 ano de experiência.");

    var perfil = new Perfil
    {
        Nome = request.Nome.Trim(),
        Email = request.Email.Trim(),
        Pais = request.Pais.Trim(),
        PrecoPorHora = request.PrecoPorHora,
        IsShared = request.IsShared,
        OwnerId = userId,
        Experiencias = request.Experiencias.Select(e => new ExperienciaProfissional
        {
            Titulo = e.Titulo.Trim(),
            Empresa = e.Empresa.Trim(),
            AnoInicio = e.AnoInicio,
            AnoFim = e.AnoFim
        }).ToList(),
        PerfilSkills = request.Skills.Select(s => new PerfilSkill
        {
            SkillId = s.SkillId,
            AnosExperiencia = s.AnosExperiencia
        }).ToList()
    };

    await repo.AddAsync(perfil);
    await repo.SaveChangesAsync(); // NOVA LINHA
    return Results.Created($"/perfis/{perfil.Id}", MapPerfilToDto(perfil));
}).RequireAuthorization("UserPolicy");

app.MapPut("/perfis/{id}", async (int id, PerfilUpdateDto request, ClaimsPrincipal user, GestaoTalentos.Infrastructure.IPerfilRepository repo, IUserRepository userRepo) =>
{
    var perfil = await repo.GetByIdAsync(id);
    if (perfil == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue("sub"), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role != UserRole.Admin && current.Role != UserRole.UserManager && perfil.OwnerId != userId)
        return Results.Forbid();

    // Validações básicas
    if (string.IsNullOrWhiteSpace(request.Nome))
        return Results.BadRequest("Nome é obrigatório.");
    if (string.IsNullOrWhiteSpace(request.Email))
        return Results.BadRequest("Email é obrigatório.");
    if (string.IsNullOrWhiteSpace(request.Pais))
        return Results.BadRequest("País é obrigatório.");
    if (request.PrecoPorHora <= 0)
        return Results.BadRequest("Preço por hora deve ser maior que zero.");

    // Validação de intervalo de datas (Ano Fim >= Ano Inicio)
    foreach (var exp in request.Experiencias)
    {
        if (exp.AnoFim.HasValue && exp.AnoFim < exp.AnoInicio)
            return Results.BadRequest($"Na empresa '{exp.Empresa}', o ano de fim ({exp.AnoFim}) não pode ser anterior ao início ({exp.AnoInicio}).");
    }

    // Validação de sobreposição de datas
    var erroData = ValidarSobreposicaoExperiencias(request.Experiencias);
    if (erroData != null) return Results.BadRequest(erroData);

    if (request.Skills.Any(s => s.AnosExperiencia < 1))
        return Results.BadRequest("As skills devem ter pelo menos 1 ano de experiência.");

    // Atualizar os dados do perfil (o repositório trata de apagar os antigos)
    perfil.Nome = request.Nome.Trim();
    perfil.Email = request.Email.Trim();
    perfil.Pais = request.Pais.Trim();
    perfil.PrecoPorHora = request.PrecoPorHora;
    perfil.IsShared = request.IsShared;
    perfil.Experiencias = request.Experiencias.Select(e => new ExperienciaProfissional
    {
        PerfilId = id,
        Titulo = e.Titulo.Trim(),
        Empresa = e.Empresa.Trim(),
        AnoInicio = e.AnoInicio,
        AnoFim = e.AnoFim
    }).ToList();
    perfil.PerfilSkills = request.Skills.Select(s => new PerfilSkill
    {
        PerfilId = id,
        SkillId = s.SkillId,
        AnosExperiencia = s.AnosExperiencia
    }).ToList();

    await repo.UpdateAsync(perfil);
    return Results.NoContent();
}).RequireAuthorization("UserPolicy");

app.MapDelete("/perfis/{id}", async (int id, ClaimsPrincipal user, GestaoTalentos.Infrastructure.IPerfilRepository repo, IUserRepository userRepo) =>
{
    var perfil = await repo.GetByIdAsync(id);
    if (perfil == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue("sub"), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role != UserRole.Admin && current.Role != UserRole.UserManager && perfil.OwnerId != userId)
        return Results.Forbid();

    await repo.DeleteAsync(id);
    await repo.SaveChangesAsync(); // NOVA LINHA
    return Results.NoContent();
}).RequireAuthorization("UserPolicy");

// ======================
// SKILLS
// ======================

app.MapGet("/skills", async (ISkillRepository repo) =>
{
    var skills = await repo.GetAllWithAreaAsync();
    return Results.Ok(skills.Select(s => new
    {
        s.Id,
        s.Nome,
        s.AreaId,
        AreaNome = s.Area == null ? "" : s.Area.Nome,
        s.CriadoEm,
        s.AtualizadoEm
    }));
}).RequireAuthorization("UserPolicy");

app.MapPost("/skills", async (CreateSkillDto dto, ISkillRepository repo) =>
{
    var nome = (dto.Nome ?? "").Trim();

    if (nome.Length < 2 || nome.Length > 100)
        return Results.BadRequest("O nome deve ter entre 2 e 100 caracteres.");

    if (dto.AreaId < 1)
        return Results.BadRequest("Área inválida.");

    if (await repo.GetByNomeAsync(nome) != null)
        return Results.Conflict("Já existe uma skill com esse nome.");

    var skill = new Skill
    {
        Nome = nome,
        AreaId = dto.AreaId,
        CriadoEm = DateTime.UtcNow,
        AtualizadoEm = DateTime.UtcNow
    };

    await repo.AddAsync(skill);

    return Results.Created($"/skills/{skill.Id}", new { skill.Id, skill.Nome, skill.AreaId });
}).RequireAuthorization("UserManagerPolicy");

app.MapPut("/skills/{id:int}", async (int id, UpdateSkillDto dto, ISkillRepository repo) =>
{
    var skill = await repo.GetByIdAsync(id);
    if (skill == null) return Results.NotFound();

    var nome = (dto.Nome ?? "").Trim();

    if (nome.Length < 2 || nome.Length > 100)
        return Results.BadRequest("O nome deve ter entre 2 e 100 caracteres.");

    if (dto.AreaId < 1)
        return Results.BadRequest("Área inválida.");

    var existing = await repo.GetByNomeAsync(nome);
    if (existing != null && existing.Id != id)
        return Results.Conflict("Já existe uma skill com esse nome.");

    skill.Nome = nome;
    skill.AreaId = dto.AreaId;
    skill.AtualizadoEm = DateTime.UtcNow;

    await repo.UpdateAsync(skill);

    return Results.NoContent();
}).RequireAuthorization("UserManagerPolicy");

// DELETE skills
app.MapDelete("/skills/{id:int}", async (int id, ISkillRepository repo, AppDbContext context) =>
{
    var skill = await repo.GetByIdAsync(id);
    if (skill == null) return Results.NotFound();

    // Requisito: Uma skill só pode ser apagada se não estiver associada a nenhum profissional
    bool estaSendoUsada = await context.PerfilSkills.AnyAsync(ps => ps.SkillId == id);
    if (estaSendoUsada)
        return Results.BadRequest("Não é possível apagar a skill pois já está a ser utilizada por um profissional.");

    try
    {
        await repo.DeleteAsync(id);
        return Results.NoContent();
    }
    catch (Microsoft.EntityFrameworkCore.DbUpdateException)
    {
        return Results.BadRequest("Não é possível apagar a skill pois já está a ser utilizada por um profissional.");
    }
}).RequireAuthorization("UserManagerPolicy");

// ======================
// AREAS
// ======================

app.MapGet("/areas", async (IAreaRepository repo) =>
{
    var areas = await repo.GetAllWithSkillsAsync();
    return Results.Ok(areas.Select(a => new GestaoTalentos.Shared.DTOs.AreaDto
    {
        Id = a.Id,
        Nome = a.Nome,
        CriadoEm = a.CriadoEm,
        TotalSkills = a.Skills.Count
    }));
}).RequireAuthorization("UserPolicy");

app.MapPost("/areas", async (AreaCreateDto dto, IAreaRepository repo) =>
{
    var nome = (dto.Nome ?? "").Trim();

    if (nome.Length < 2 || nome.Length > 100)
        return Results.BadRequest("O nome deve ter entre 2 e 100 caracteres.");

    if (await repo.GetByNomeAsync(nome) != null)
        return Results.Conflict("Já existe uma área com esse nome.");

    var area = new Area
    {
        Nome = nome,
        CriadoEm = DateTime.UtcNow
    };

    await repo.AddAsync(area);

    return Results.Created($"/areas/{area.Id}", new GestaoTalentos.Shared.DTOs.AreaDto { Id = area.Id, Nome = area.Nome, CriadoEm = area.CriadoEm });
}).RequireAuthorization("UserManagerPolicy");

app.MapPut("/areas/{id:int}", async (int id, AreaCreateDto dto, IAreaRepository repo) =>
{
    var area = await repo.GetByIdAsync(id);
    if (area == null) return Results.NotFound();

    var nome = (dto.Nome ?? "").Trim();

    if (nome.Length < 2 || nome.Length > 100)
        return Results.BadRequest("O nome deve ter entre 2 e 100 caracteres.");

    var existing = await repo.GetByNomeAsync(nome);
    if (existing != null && existing.Id != id)
        return Results.Conflict("Já existe uma área com esse nome.");

    area.Nome = nome;

    await repo.UpdateAsync(area);

    return Results.NoContent();
}).RequireAuthorization("UserManagerPolicy");

app.MapDelete("/areas/{id:int}", async (int id, IAreaRepository repo, AppDbContext context) =>
{
    var area = await repo.GetByIdAsync(id);
    if (area == null) return Results.NotFound();

    bool temSkills = await context.Skills.AnyAsync(s => s.AreaId == id);
    if (temSkills)
        return Results.BadRequest("Não é possível apagar a área pois tem skills associadas.");

    try
    {
        await repo.DeleteAsync(id);
        return Results.NoContent();
    }
    catch (Microsoft.EntityFrameworkCore.DbUpdateException)
    {
        return Results.BadRequest("Não é possível apagar a área pois tem skills associadas.");
    }
}).RequireAuthorization("UserManagerPolicy");

// ====================================
// FUNÇÕES AUXILIARES
// ====================================

static object MapPerfilToDto(Perfil p) => new
{
    p.Id,
    p.OwnerId,
    p.Nome,
    p.Email,
    p.Pais,
    p.PrecoPorHora,
    p.IsShared,
    p.CreatedAt,
    Experiencias = p.Experiencias.Select(e => new
    {
        e.Id,
        e.Titulo,
        e.Empresa,
        e.AnoInicio,
        e.AnoFim
    }),
    Skills = p.PerfilSkills.Select(ps => new
    {
        ps.SkillId,
        SkillNome = ps.Skill?.Nome,
        ps.AnosExperiencia
    })
};


static string? ValidarSobreposicaoExperiencias(List<ExperienciaCreateDto> experiencias)
{
    for (int i = 0; i < experiencias.Count; i++)
    {
        var expA = experiencias[i];
        int fimA = expA.AnoFim ?? DateTime.UtcNow.Year;

        for (int j = i + 1; j < experiencias.Count; j++)
        {
            var expB = experiencias[j];
            int fimB = expB.AnoFim ?? DateTime.UtcNow.Year;

            // Há sobreposição se um período começa antes de outro terminar
            bool sobreposicao = expA.AnoInicio <= fimB && expB.AnoInicio <= fimA;
            if (sobreposicao)
                return $"Sobreposição de datas detetada entre '{expA.Empresa}' ({expA.AnoInicio}-{expA.AnoFim?.ToString() ?? "atual"}) e '{expB.Empresa}' ({expB.AnoInicio}-{expB.AnoFim?.ToString() ?? "atual"}).";
        }
    }
    return null;
}

// --- CLIENTES ---

app.MapGet("/clientes", async (ClaimsPrincipal user, IClienteRepository repo, IUserRepository userRepo) =>
{
    var userId = int.TryParse(user.FindFirstValue("sub"), out var id) ? id : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    var clientes = current.Role == UserRole.User
        ? await repo.GetByCriadorIdAsync(userId)
        : await repo.GetAllAsync();

    return Results.Ok(clientes.Select(c => new ClienteDto(c.Id, c.Nome, c.Email, c.IdCriador, c.IdMinhaConta)));
}).RequireAuthorization("UserPolicy");

app.MapGet("/clientes/{id}", async (int id, ClaimsPrincipal user, IClienteRepository repo, IUserRepository userRepo) =>
{
    var cliente = await repo.GetByIdAsync(id);
    if (cliente == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue("sub"), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role == UserRole.User && cliente.IdCriador != userId)
        return Results.Forbid();

    return Results.Ok(new ClienteDto(cliente.Id, cliente.Nome, cliente.Email, cliente.IdCriador, cliente.IdMinhaConta));
}).RequireAuthorization("UserPolicy");

app.MapPost("/clientes", async (ClienteCreateDto request, ClaimsPrincipal user, IClienteRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Nome)) return Results.BadRequest("Nome é obrigatório.");
    if (string.IsNullOrWhiteSpace(request.Email)) return Results.BadRequest("Email é obrigatório.");

    var userId = int.TryParse(user.FindFirstValue("sub"), out var id) ? id : 0;
    var cliente = new GestaoTalentos.Domain.Cliente
    {
        Nome = request.Nome.Trim(),
        Email = request.Email.Trim(),
        IdCriador = userId,
        IdMinhaConta = request.IdMinhaConta == 0 ? null : request.IdMinhaConta
    };
    await repo.AddAsync(cliente);
    await repo.SaveChangesAsync(); // NOVA LINHA
    return Results.Created($"/clientes/{cliente.Id}", new ClienteDto(cliente.Id, cliente.Nome, cliente.Email, cliente.IdCriador, cliente.IdMinhaConta));
}).RequireAuthorization("UserPolicy");

app.MapPut("/clientes/{id}", async (int id, ClienteUpdateDto request, ClaimsPrincipal user, IClienteRepository repo, IUserRepository userRepo) =>
{
    var cliente = await repo.GetByIdAsync(id);
    if (cliente == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue("sub"), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role == UserRole.User && cliente.IdCriador != userId)
        return Results.Forbid();

    if (string.IsNullOrWhiteSpace(request.Nome)) return Results.BadRequest("Nome é obrigatório.");
    if (string.IsNullOrWhiteSpace(request.Email)) return Results.BadRequest("Email é obrigatório.");

    cliente.Nome = request.Nome.Trim();
    cliente.Email = request.Email.Trim();
    cliente.IdMinhaConta = request.IdMinhaConta == 0 ? null : request.IdMinhaConta;
    await repo.UpdateAsync(cliente);
    return Results.NoContent();
}).RequireAuthorization("UserPolicy");

app.MapDelete("/clientes/{id}", async (int id, ClaimsPrincipal user, IClienteRepository repo, IUserRepository userRepo) =>
{
    var cliente = await repo.GetByIdAsync(id);
    if (cliente == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue("sub"), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role == UserRole.User && cliente.IdCriador != userId)
        return Results.Forbid();

    await repo.DeleteAsync(id);
    return Results.NoContent();
}).RequireAuthorization("UserPolicy");

// ======================
// PROPOSTAS DE TRABALHO
// ======================

app.MapGet("/propostas", async (IPropostaRepository repo) =>
{
    var propostas = await repo.GetAllWithSkillsAsync();
    return Results.Ok(propostas.Select(p => new
    {
        p.Id,
        p.Nome,
        p.AreaId,
        Area = p.Area == null ? null : new { p.Area.Id, p.Area.Nome },
        p.DescricaoTrabalho,
        p.NumeroTotalHoras,
        p.PrecoHoraMedio,
        ValorEstimadoTotal = p.NumeroTotalHoras * p.PrecoHoraMedio,
        p.CriadoEm,
        p.AtualizadoEm,
        SkillsNecessarias = p.SkillsNecessarias.Select(sn => new
        {
            sn.Id,
            sn.SkillId,
            sn.NivelMinimoRequerido,
            Skill = sn.Skill == null ? null : new { sn.Skill.Id, sn.Skill.Nome }
        })
    }));
}).RequireAuthorization("UserPolicy");

app.MapGet("/propostas/{id:int}", async (int id, IPropostaRepository repo, ITalentoElegivelRepository talentoRepo) =>
{
    var proposta = await repo.GetByIdWithSkillsAsync(id);
    if (proposta == null) return Results.NotFound();

    var talentos = await talentoRepo.GetByPropostaIdOrderedByValorAsync(id);

    return Results.Ok(new
    {
        proposta.Id,
        proposta.Nome,
        proposta.AreaId,
        Area = proposta.Area == null ? null : new { proposta.Area.Id, proposta.Area.Nome },
        proposta.DescricaoTrabalho,
        proposta.NumeroTotalHoras,
        proposta.PrecoHoraMedio,
        ValorEstimadoTotal = proposta.NumeroTotalHoras * proposta.PrecoHoraMedio,
        proposta.CriadoEm,
        proposta.AtualizadoEm,
        SkillsNecessarias = proposta.SkillsNecessarias.Select(sn => new
        {
            sn.Id,
            sn.SkillId,
            sn.NivelMinimoRequerido,
            Skill = sn.Skill == null ? null : new { sn.Skill.Id, sn.Skill.Nome }
        }),
        TalentosElegiveis = talentos.Select(te => new
        {
            te.Id,
            te.PerfilId,
            te.ValorEstimado,
            Perfil = te.Perfil == null ? null : new { te.Perfil.Id, te.Perfil.OwnerId }
        }).OrderBy(te => te.ValorEstimado)
    });
}).RequireAuthorization("UserPolicy");

app.MapPost("/propostas", async (CreatePropostaDto dto, IPropostaRepository repo, PropostaMatchingService matchingService, ITalentoElegivelRepository talentoRepo, AppDbContext context) =>
{
    if (string.IsNullOrWhiteSpace(dto.Nome))
        return Results.BadRequest("Nome é obrigatório");

    if (await repo.GetByNomeAsync(dto.Nome) != null)
        return Results.Conflict("Já existe uma proposta com esse nome");

    var proposta = new Proposta
    {
        Nome = dto.Nome.Trim(),
        AreaId = dto.AreaId,
        DescricaoTrabalho = dto.DescricaoTrabalho.Trim(),
        NumeroTotalHoras = dto.NumeroTotalHoras,
        PrecoHoraMedio = dto.PrecoHoraMedio,
        CriadoEm = DateTime.UtcNow,
        AtualizadoEm = DateTime.UtcNow
    };

    await repo.AddAsync(proposta);

    foreach (var skillDto in dto.SkillsNecessarias)
    {
        var skillNecessaria = new SkillNecessaria
        {
            SkillId = skillDto.SkillId,
            PropostaId = proposta.Id,
            NivelMinimoRequerido = skillDto.AnosExperienciaMinimo,
            CriadoEm = DateTime.UtcNow
        };
        await context.SkillsNecessarias.AddAsync(skillNecessaria);
    }

    await context.SaveChangesAsync();

    var talentosElegiveis = await matchingService.IdentificarTalentosElegiveisAsync(proposta.Id, proposta.PrecoHoraMedio);
    foreach (var talento in talentosElegiveis)
    {
        await talentoRepo.AddAsync(talento);
    }

    return Results.Created($"/propostas/{proposta.Id}", new { proposta.Id, proposta.Nome });
}).RequireAuthorization("UserManagerPolicy");

app.MapPut("/propostas/{id:int}", async (int id, UpdatePropostaDto dto, IPropostaRepository repo, PropostaMatchingService matchingService, ITalentoElegivelRepository talentoRepo, AppDbContext context) =>
{
    var proposta = await repo.GetByIdWithSkillsAsync(id);
    if (proposta == null) return Results.NotFound();

    proposta.Nome = dto.Nome.Trim();
    proposta.AreaId = dto.AreaId;
    proposta.DescricaoTrabalho = dto.DescricaoTrabalho.Trim();
    proposta.NumeroTotalHoras = dto.NumeroTotalHoras;
    proposta.PrecoHoraMedio = dto.PrecoHoraMedio;
    proposta.AtualizadoEm = DateTime.UtcNow;

    context.SkillsNecessarias.RemoveRange(proposta.SkillsNecessarias);

    foreach (var skillDto in dto.SkillsNecessarias)
    {
        var skillNecessaria = new SkillNecessaria
        {
            SkillId = skillDto.SkillId,
            PropostaId = proposta.Id,
            NivelMinimoRequerido = skillDto.AnosExperienciaMinimo
        };
        proposta.SkillsNecessarias.Add(skillNecessaria);
    }

    await repo.UpdateAsync(proposta);

    await talentoRepo.DeleteByPropostaIdAsync(id);
    var talentosElegiveis = await matchingService.IdentificarTalentosElegiveisAsync(id, proposta.PrecoHoraMedio);
    foreach (var talento in talentosElegiveis)
    {
        await talentoRepo.AddAsync(talento);
    }

    return Results.NoContent();
}).RequireAuthorization("UserManagerPolicy");

app.MapDelete("/propostas/{id:int}", async (int id, IPropostaRepository repo) =>
{
    var proposta = await repo.GetByIdAsync(id);
    if (proposta == null) return Results.NotFound();

    try
    {
        await repo.DeleteAsync(id);
        return Results.NoContent();
    }
    catch (Microsoft.EntityFrameworkCore.DbUpdateException)
    {
        return Results.BadRequest("Não é possível apagar a proposta");
    }
}).RequireAuthorization("UserManagerPolicy");

// ======================
// DASHBOARD (US-09)
// ======================

app.MapGet("/dashboard", async (AppDbContext context) =>
{
    var agora = DateTime.UtcNow;
    var quatroSemanasAtras = agora.AddDays(-28);

    var totalPerfis = await context.Perfis.CountAsync();
    var totalPropostas = await context.Propostas.CountAsync();

    var perfisUltimasQuatroSemanas = await context.Perfis
        .CountAsync(p => p.CreatedAt >= quatroSemanasAtras);

    var propostas = await context.Propostas.ToListAsync();
    var valorTotalEstimado = propostas.Sum(p => p.NumeroTotalHoras * p.PrecoHoraMedio);

    var totalTalentosElegiveis = await context.TalentosElegiveis.CountAsync();

    // Taxa de aprovação: % de propostas com pelo menos 1 talento elegível
    var propostasComTalentos = await context.TalentosElegiveis
        .Select(te => te.PropostaId)
        .Distinct()
        .CountAsync();

    var taxaAprovacao = totalPropostas > 0
        ? Math.Round((double)propostasComTalentos / totalPropostas * 100, 1)
        : 0.0;

    // Detalhes de cada proposta com métricas 176h/mês
    var propostasDetalhadas = propostas.Select(p => new
    {
        p.Id,
        p.Nome,
        p.NumeroTotalHoras,
        p.PrecoHoraMedio,
        ValorEstimado = p.NumeroTotalHoras * p.PrecoHoraMedio,
        MesesEquivalentes = Math.Round(p.NumeroTotalHoras / 176.0, 2),
        ValorMensalEquivalente = Math.Round((double)(p.PrecoHoraMedio * 176), 2)
    }).OrderByDescending(p => p.ValorEstimado).ToList();

    return Results.Ok(new
    {
        TotalPerfis = totalPerfis,
        TotalPropostas = totalPropostas,
        ValorTotalEstimadoPropostas = valorTotalEstimado,
        PerfisUltimasQuatroSemanas = perfisUltimasQuatroSemanas,
        TotalTalentosElegiveis = totalTalentosElegiveis,
        TaxaAprovacao = taxaAprovacao,
        PropostasDetalhadas = propostasDetalhadas
    });
}).RequireAuthorization("UserManagerPolicy");

app.Run();