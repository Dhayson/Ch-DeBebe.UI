using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class ChaDeBebeEndpoints
{
    public static void MapChaDeBebeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/cha_de_bebe").WithTags("Chá de Bebê");
        // Usa ClaimsPrincipal para recuperar o Id do token JWT
        group.MapPost("/criar", [Authorize] async (CriarChaDeBebeDTO req, ClaimsPrincipal user, AppDbContext db) =>
        {
            var adminId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var service = new ChaDeBebeService(db);
            var result = await service.Criar(req, adminId);
            if (result == null)
            {
                return Results.Conflict("Erro ao criar Chá de Bebê: nome de evento duplicado.");
            }
            return Results.Created($"/api/cha/{result.Id}", result);
        }).RequireAuthorization();

        group.MapPost("/entrar/{inviteCode:int}", [Authorize] async (
            int inviteCode,
            ClaimsPrincipal user,
            AppDbContext db
        ) =>
        {
            // 1. Extrai o ID do usuário usando o Helper
            var usuarioId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var service = new ChaDeBebeService(db);

            (ChaDeBebeEvento? cha, string error, int code) = await service.Entrar(inviteCode, usuarioId);
            if (code != 202)
            {
                return Results.Json(new { Message = error }, JsonSerializerOptions.Default, null, code);
            }
            return Results.Accepted($"/api/cha/{cha!.Id}", new { error, chaDeBebe = cha });
        }).RequireAuthorization();

        group.MapGet("/meus_chas", [Authorize] async (AppDbContext db, ClaimsPrincipal user) =>
        {
            var adminId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var meusChas = await db.ChasDeBebe.AsNoTracking()
                .Where(c => c.AdminId == adminId)
                .Select(c => new
                {
                    c.Id,
                    c.Nome,
                    c.DataEvento,
                    QtdPresentes = c.Presentes.Count // Exemplo de contagem rápida
                })
                .ToListAsync();

            return Results.Ok(meusChas);
        }).RequireAuthorization();

        group.MapGet("/chas_inscrito", [Authorize] async (AppDbContext db, ClaimsPrincipal user) =>
        {
            var adminId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var meusChas = await db.UsuarioChaDeBebe.AsNoTracking()
                .Where(u => u.UsuarioId == adminId)
                .Select(u => new
                {
                    u.ChaDeBebe!.Id,
                    u.ChaDeBebe!.AdminId,
                    Admin = u.ChaDeBebe!.Admin!.Nome,
                    AdminEmail = u.ChaDeBebe!.Admin!.Email,
                    u.ChaDeBebe!.Nome,
                    u.ChaDeBebe!.DataEvento,
                    QtdPresentes = u.ChaDeBebe!.Presentes.Count // Exemplo de contagem rápida
                })
                .ToListAsync();

            return Results.Ok(meusChas);
        }).RequireAuthorization();
    }
}