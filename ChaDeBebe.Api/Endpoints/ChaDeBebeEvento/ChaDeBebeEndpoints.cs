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
    }
}