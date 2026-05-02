namespace GestaoTalentos.Infrastructure;


using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using GestaoTalentos.Domain;

public class AuditEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var http = context.HttpContext;
        var method = http.Request.Method;
        var path = http.Request.Path.Value?.ToLower() ?? "";

        if (path.Contains("/login") || path.Contains("/register"))
            return await next(context);

        if (method != "POST" && method != "PUT" && method != "DELETE")
            return await next(context);

        var user = http.User;

        if (!user.Identity?.IsAuthenticated ?? true)
            return await next(context);

        var userId = int.TryParse(user.FindFirst("sub")?.Value, out var uid) ? uid : 0;
        var username = user.Identity?.Name ?? "unknown";

        var entityType = GetEntity(path);

        var entityId = 0;
        int.TryParse(http.Request.RouteValues["id"]?.ToString(), out entityId);

        var db = http.RequestServices.GetRequiredService<AppDbContext>();

        string? oldValues = null;

        if (method == "PUT" || method == "DELETE")
            oldValues = await GetEntitySnapshot(db, entityType, entityId);

        var result = await next(context);

        var status = http.Response.StatusCode;

        if (!http.Response.StatusCode.ToString().StartsWith("2"))
            return result;

        string? newValues = null;

        if (method == "POST" || method == "PUT")
            newValues = await GetEntitySnapshot(db, entityType, entityId);

        var audit = http.RequestServices.GetRequiredService<AuditManager>();

        await audit.NotifyAsync(new Log
        {
            UserId = userId,
            Username = username,

            CommandName = GetCommand(method, entityType),

            EntityType = entityType,
            EntityId = entityId,
            EntityName = await GetEntityName(db, entityType, entityId),

            OldValues = oldValues,
            NewValues = newValues
        });

        return result;
    }

    private string GetEntity(string path)
    {
        if (path.Contains("/skills")) return "Skill";
        if (path.Contains("/clientes")) return "Cliente";
        if (path.Contains("/perfis")) return "Perfil";
        if (path.Contains("/users")) return "User";
        if (path.Contains("/propostas")) return "Proposta";
        if (path.Contains("/experiencias")) return "Experiencia";

        return "Unknown";
    }

    private string GetCommand(string method, string entity)
    {
        return method switch
        {
            "POST" => $"Create{entity}",
            "PUT" => $"Update{entity}",
            "DELETE" => $"Delete{entity}",
            _ => $"Action{entity}"
        };
    }

    private async Task<string> GetEntityName(AppDbContext db, string type, int id)
    {
        switch (type)
        {
            case "Skill":
                return (await db.Skills.FindAsync(id))?.Nome ?? "";
            case "Cliente":
                return (await db.Clientes.FindAsync(id))?.Nome ?? "";
            case "Perfil":
                return (await db.Perfis.FindAsync(id))?.Nome ?? "";
            case "User":
                return (await db.Users.FindAsync(id))?.Username ?? "";
            default:
                return "";
        }
    }

    private async Task<string?> GetEntitySnapshot(AppDbContext db, string type, int id)
    {
        object? entity = type switch
        {
            "Skill" => await db.Skills.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id),
            "Cliente" => await db.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id),
            "Perfil" => await db.Perfis.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id),
            "User" => await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id),
            _ => null
        };

        return entity == null ? null : JsonSerializer.Serialize(entity);
    }
}