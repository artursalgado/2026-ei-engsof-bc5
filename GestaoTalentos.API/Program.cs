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
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
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
builder.Services.AddScoped<IPerfilRepository, PerfilRepository>();
// builder.Services.AddScoped<IRecordRepository, RecordRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddScoped<ITalentoElegivelRepository, TalentoElegivelRepository>();
builder.Services.AddScoped<PropostaMatchingService>();


// Repositórios

builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddScoped<ITalentoElegiveRepository, TalentoElegiveRepository>();
builder.Services.AddScoped<IPerfilRepository, PerfilRepository>();
// Services
builder.Services.AddScoped<IPropostaService, PropostaService>();
builder.Services.AddScoped<ITalentoElegiveService, TalentoElegiveService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();



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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy => policy.RequireRole(UserRole.User.ToString(), UserRole.UserManager.ToString(), UserRole.Admin.ToString()));
    options.AddPolicy("UserManagerPolicy", policy => policy.RequireRole(UserRole.UserManager.ToString(), UserRole.Admin.ToString()));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole(UserRole.Admin.ToString()));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientCors", policy =>
        policy.WithOrigins("http://localhost:5025", "https://localhost:5025")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

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

app.MapPost("/register", async (GestaoTalentos.API.UserRegisterDto request, IUserRepository repo) =>
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
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    var current = await repo.GetByIdAsync(userId);
    return current is null ? Results.NotFound() : Results.Ok(new { current.Id, current.Username, current.Role });
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
    return Results.Created($"/users/{user.Id}", new { user.Id, user.Username, user.Role });
}).RequireAuthorization("UserManagerPolicy");

app.MapGet("/perfis", async (ClaimsPrincipal user, IPerfilRepository repo, IUserRepository userRepo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role == UserRole.User)
    {
        var perfis = await repo.GetVisibleForUserAsync(userId);
        return Results.Ok(perfis.Select(r => new PerfilDto(r.Id, r.OwnerId, r.Content, r.IsShared, r.CreatedAt)));
    }

    var all = await repo.GetAllAsync();
    return Results.Ok(all.Select(r => new PerfilDto(r.Id, r.OwnerId, r.Content, r.IsShared, r.CreatedAt)));
}).RequireAuthorization("UserPolicy");

app.MapGet("/perfis/{id}", async (int id, ClaimsPrincipal user, IPerfilRepository repo, IUserRepository userRepo) =>
{
    var rec = await repo.GetByIdAsync(id);
    if (rec == null) return Results.NotFound();

    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
    var current = await userRepo.GetByIdAsync(userId);
    if (current == null) return Results.Unauthorized();

    if (current.Role == UserRole.Admin || current.Role == UserRole.UserManager || rec.OwnerId == userId || rec.IsShared)
        return Results.Ok(new PerfilDto(rec.Id, rec.OwnerId, rec.Content, rec.IsShared, rec.CreatedAt));

    return Results.Forbid();
}).RequireAuthorization("UserPolicy");

app.MapPost("/perfis", async (PerfilCreateDto request, ClaimsPrincipal user, IPerfilRepository repo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;

    if (string.IsNullOrWhiteSpace(request.Content))
        return Results.BadRequest("Content obrigatório.");

    var perfil = new Perfil { OwnerId = userId, Content = request.Content.Trim(), IsShared = request.IsShared };
    await repo.AddAsync(perfil);
    return Results.Created($"/perfis/{perfil.Id}", new PerfilDto(perfil.Id, perfil.OwnerId, perfil.Content, perfil.IsShared, perfil.CreatedAt));
}).RequireAuthorization("UserPolicy");

app.MapPut("/perfis/{id}", async (int id, PerfilUpdateDto request, ClaimsPrincipal user, IPerfilRepository repo, IUserRepository userRepo) =>
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

app.MapDelete("/perfis/{id}", async (int id, ClaimsPrincipal user, IPerfilRepository repo, IUserRepository userRepo) =>
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
        Area = s.Area == null ? null : new { s.Area.Id, s.Area.Nome },
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

app.MapDelete("/skills/{id:int}", async (int id, ISkillRepository repo) =>
{
    var skill = await repo.GetByIdAsync(id);
    if (skill == null) return Results.NotFound();

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
    var areas = await repo.GetAllAsync();
    return Results.Ok(areas.Select(a => new GestaoTalentos.Shared.DTOs.AreaDto { Id = a.Id, Nome = a.Nome }));
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

app.Run();