// Programa principal da API GestaoTalentos
// Configura e executa uma aplicação ASP.NET Core com APIs mínimas para gestão de
// utilizadores, registros, clientes e apresentações.
// Inclui autenticação JWT, autorização baseada em roles e acesso à base de dados PostgreSQL.

using GestaoTalentos.API;
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

// Evitar o mapeamento de nomes de claims para schemas SOAP
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços da API
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

// Configuração do contexto da base de dados com PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Port=5432;Database=gestaotalentos;Username=postgres;Password=postgres"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPerfilRepository, PerfilRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPaisRepository, PaisRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = "role",
            NameClaimType = "name"
        };
    });

// Configuração de políticas de autorização baseadas em roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("user", "usermanager", "admin"));
    options.AddPolicy("UserManagerPolicy", policy => policy.RequireRole("usermanager", "admin"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
});
// erro cors
// builder.Services.AddCors removido (consolidado acima)
//
var app = builder.Build();

// Configuração de middleware para desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
// app.UseHttpsRedirection(); // Comentado para evitar perda de token em redirecionamentos locais
app.UseAuthentication();
app.UseAuthorization();

// Inicialização da base de dados e criação de utilizador admin se não existir
await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    
    // TRUQUE TEMPORÁRIO PARA LIMPAR A BASE DE DADOS (já utilizado - manter comentado!):
    //await context.Database.EnsureDeletedAsync();
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
    var userId = int.TryParse(user.FindFirstValue("sub"), out var id) ? id : 0;
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
// Endpoints de Utilizadores (Focado apenas no Login/Me)
app.MapGet("/perfis", async (ClaimsPrincipal user, IPerfilRepository repo, IUserRepository userRepo) =>
{
    var userIdStr = user.FindFirstValue("sub");
    var userId = int.TryParse(userIdStr, out var id) ? id : 0;
    
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role.Nome == "user")
    {
        var perfis = await repo.GetVisibleForUserAsync(userId);
        return Results.Ok(perfis.Select(r => new PerfilDto(r.Id, r.OwnerId, r.Content, r.IsShared, r.CreatedAt)));
    }
    List<Perfil> perfis;
    if (current.Role == UserRole.User && current.TipoUtilizador == TipoUtilizador.Cliente)
        perfis = await repo.GetPublicAsync();
    else if (current.Role == UserRole.User)
        perfis = await repo.GetByOwnerAsync(userId);
    else
        perfis = await repo.GetAllAsync();

    return Results.Ok(perfis.Select(p => MapPerfilToDto(p)));
}).RequireAuthorization("UserPolicy");

// NOVO: Sugestões de empresas baseadas no que já existe na BD (Para evitar hardcoded)
app.MapGet("/perfis/empresas-sugestoes", async (AppDbContext context) =>
{
    var empresas = await context.ExperienciasProfissionais
        .Select(e => e.Empresa)
        .Distinct()
        .OrderBy(n => n)
        .ToListAsync();
    return Results.Ok(empresas);
}).RequireAuthorization("UserPolicy");

// Endpoint para obter um perfil específico por ID
app.MapGet("/perfis/{id}", async (int id, ClaimsPrincipal user, IPerfilRepository repo, 
    IUserRepository userRepo) =>
{
    var perfil = await repo.GetByIdAsync(id);
    if (perfil == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue("sub"), out var uid) ? uid : 0;
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
    foreach(var exp in request.Experiencias) {
        if (exp.AnoFim.HasValue && exp.AnoFim < exp.AnoInicio)
            return Results.BadRequest($"Na empresa '{exp.Empresa}', o ano de fim ({exp.AnoFim}) não pode ser anterior ao início ({exp.AnoInicio}).");
    }

    // Validação de sobreposição de datas
    var erroData = ValidarSobreposicaoExperiencias(request.Experiencias);
    if (erroData != null) return Results.BadRequest(erroData);

    if (request.Skills.Any(s => s.AnosExperiencia < 1))
        return Results.BadRequest("As skills devem ter pelo menos 1 ano de experiência.");

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

    return Results.Created(
        $"/perfis/{perfil.Id}",
        new PerfilDto(perfil.Id, perfil.OwnerId, perfil.Content, perfil.IsShared, perfil.CreatedAt)
    );
    return Results.Created($"/perfis/{perfil.Id}", MapPerfilToDto(perfil));
}).RequireAuthorization("UserPolicy");

// Endpoint para atualizar um perfil
app.MapPut("/perfis/{id}", async (int id, PerfilUpdateDto request, ClaimsPrincipal user,
    IPerfilRepository repo, IUserRepository userRepo, IPaisRepository paisRepo) =>
{
    var perfil = await repo.GetByIdAsync(id);
    if (perfil == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue("sub"), out var uid) ? uid : 0;
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
    foreach(var exp in request.Experiencias) {
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

// Endpoint para deletar um perfil
app.MapDelete("/perfis/{id}", async (int id, ClaimsPrincipal user, IPerfilRepository repo, IUserRepository userRepo) =>
{
    var perfil = await repo.GetByIdAsync(id);
    if (perfil == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue("sub"), out var uid) ? uid : 0;
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



// ======================
// SKILLS
// ======================

// GET skills (com Area)
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


// POST skills (criar)
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


// PUT skills (update)
app.MapPut("/skills/{id:int}", async (int id, UpdateSkillDto dto, ISkillRepository repo) =>
{
    var skill = await repo.GetByIdAsync(id);
    if (skill == null) return Results.NotFound();

    var nome = (dto.Nome ?? "").Trim();

    if (nome.Length < 2 || nome.Length > 100)
        return Results.BadRequest("O nome deve ter entre 2 e 100 caracteres.");

    if (dto.AreaId < 1)
        return Results.BadRequest("Área inválida.");

    // evitar nome duplicado (ignorando a própria skill)
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

app.Run();

// ====================================
// FUNÇÕES AUXILIARES
// ====================================

// Converte a entidade Perfil para o DTO de resposta completo
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
        e.Id, e.Titulo, e.Empresa, e.AnoInicio, e.AnoFim
    }),
    Skills = p.PerfilSkills.Select(ps => new
    {
        ps.SkillId,
        SkillNome = ps.Skill?.Nome,
        ps.AnosExperiencia
    })
};

// Algoritmo detetive de sobreposicao temporal de experiencias profissionais
// Retorna mensagem de erro se houver conflito, ou null se ficar limpo
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
