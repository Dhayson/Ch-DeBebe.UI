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
                usuarioId, req.ChaDeBebeEventoId, req.presenteId, arquivo);
            if (code != 200)
            {
                return Results.Json(new { Message = error }, JsonSerializerOptions.Default, null, code);
            }
            return Results.Ok(result);
        }).RequireAuthorization().DisableAntiforgery();

        group.MapGet("/presentes_cha", async (int chaDeBebeId, AppDbContext db, ClaimsPrincipal user) =>
        {
            var baseUrl = $"http://localhost:5000/app/upload/presentes";
            var meusChas = await db.ChasDeBebe.AsNoTracking()
                .Where(c => c.Id == chaDeBebeId)
                .Select(c => new
                {
                    c.Id,
                    c.Nome,
                    c.AdminId,
                    c.DataEvento,
                    Presentes = c.Presentes.Select(p => new
                    {
                        p.Id,
                        p.Nome,
                        p.Descricao,
                        p.LinkSugerido,
                        pathImage = string.IsNullOrEmpty(p.PathImage) ? null : $"{baseUrl}/{p.PathImage}",
                        p.Preco,
                        p.QuantidadeTotal,
                        QuantidadeRestante = p.QuantidadeTotal - p.Reservas.Sum(r => r.Quantidade),
                        Reservas = p.Reservas.Select(r => new
                        {
                            r.Id,
                            r.DataReserva,
                            r.Quantidade,
                            r.Usuario!.Nome
                        })
                    }),
                })
                .FirstOrDefaultAsync();

            return Results.Ok(meusChas);
        });
    }
}