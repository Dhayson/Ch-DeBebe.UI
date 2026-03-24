using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class PresenteEndpoints
{
    public static void MapPresenteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/presente").WithTags("Presentes");
        // Usa ClaimsPrincipal para recuperar o Id do token JWT
        group.MapPost("/adicionar", [Authorize] async (PresenteDTO req, ClaimsPrincipal user, AppDbContext db, IConfiguration config) =>
        {
            var adminId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var service = new PresenteService(db, config);
            (var result, string error, int code) = await service.AdicionarPresenteAsync(adminId, req.ChaDeBebeEventoId, req);
            if (code != 201)
            {
                return Results.Json(new { Message = error }, JsonSerializerOptions.Default, null, code);
            }
            return Results.Accepted($"/api/presente/{result!.Id}", result);
        }).RequireAuthorization();

        group.MapPost("/atualizar/{presenteId:int}", [Authorize] async (
            int presenteId,
            [FromBody] PresenteDTO req,
            ClaimsPrincipal user,
            AppDbContext db,
            IConfiguration config
        ) =>
        {
            var adminId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var service = new PresenteService(db, config);
            (var result, string error, int code) = await service.AtualizarPresenteAsync(
                adminId,
                req.ChaDeBebeEventoId,
                presenteId,
                req);
            if (code != 201)
            {
                return Results.Json(new { Message = error }, JsonSerializerOptions.Default, null, code);
            }
            return Results.Accepted($"/api/presente/{result!.Id}", result);
        }).RequireAuthorization();

        // É um Post pois utiliza o body
        group.MapPost("/deletar", [Authorize] async (
            [FromBody] ReqPresenteDTO req, ClaimsPrincipal user, AppDbContext db, IConfiguration config
        ) =>
        {
            // 1. Extrai o ID do usuário usando o Helper
            var usuarioId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var service = new PresenteService(db, config);
            (var result, string error, int code) = await service.RemoverPresenteAsync(usuarioId, req.ChaDeBebeEventoId, req.presenteId);
            if (code != 204)
            {
                return Results.Json(new { Message = error }, JsonSerializerOptions.Default, null, code);
            }
            return Results.NoContent();
        }).RequireAuthorization();

        group.MapPost("/adicionar-imagem", [Authorize] async (
            IFormFile arquivo, [FromForm] ReqPresenteDTO req, ClaimsPrincipal user, AppDbContext db, IConfiguration config
        ) =>
        {
            var usuarioId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var service = new PresenteService(db, config);
            (var result, string error, int code) = await service.AdicionarImagemPresenteAsync(
                usuarioId, req.presenteId, req.ChaDeBebeEventoId, arquivo);
            if (code != 200)
            {
                return Results.Json(new { Message = error }, JsonSerializerOptions.Default, null, code);
            }
            return Results.Ok(result);
        }).RequireAuthorization().DisableAntiforgery();
    }
}