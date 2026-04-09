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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Port=5432;Database=gestaotalentos;Username=postgres;Password=postgres123"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRecordRepository, RecordRepository>();
//
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();

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
// erro cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientCors", policy =>
        policy.WithOrigins("http://localhost:5025", "https://localhost:5025")
              .AllowAnyHeader()
              .AllowAnyMethod());
});
//
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("ClientCors");
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

app.MapPost("/records", async (RecordCreateDto request, ClaimsPrincipal user, IRecordRepository repo) =>
{
    var userId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;

    if (string.IsNullOrWhiteSpace(request.Content))
        return Results.BadRequest("Content obrigatório.");

    var record = new Record { OwnerId = userId, Content = request.Content.Trim(), IsShared = request.IsShared };
    await repo.AddAsync(record);
    return Results.Created($"/records/{record.Id}", new RecordDto(record.Id, record.OwnerId, record.Content, record.IsShared, record.CreatedAt));
}).RequireAuthorization("UserPolicy");

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
        Area = s.Area == null ? null : new { s.Area.Id, s.Area.Nome },
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

app.Run();
