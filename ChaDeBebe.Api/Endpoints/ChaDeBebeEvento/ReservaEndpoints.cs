using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class ReservaEndpoints
{
    public static void MapReservaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/reserva").WithTags("Reservas");
        group.MapPost("/adicionar", [Authorize] async (ReservaDTO req, ClaimsPrincipal user, AppDbContext db, IConfiguration config) =>
        {
            var usuarioId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var service = new ReservaService(db, config);
            if (req.UsuarioId != usuarioId)
            {
                return Results.Unauthorized();
            }
            (var result, string error, int code) = await service.AdicionarReservaAsync(req);
            if (code != 200)
            {
                return Results.Json(new { Message = error }, JsonSerializerOptions.Default, null, code);
            }
            return Results.Created($"/api/reserva/{result!.Id}", result);
        }).RequireAuthorization();

        group.MapDelete("/deletar/{id}", [Authorize] async (int id, ClaimsPrincipal user, AppDbContext db, IConfiguration config) =>
        {
            var usuarioId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var service = new ReservaService(db, config);
            (var success, string error, int code) = await service.DeletarReservaAsync(id, usuarioId);
            if (code != 204)
            {
                return Results.Json(new { Message = error }, JsonSerializerOptions.Default, null, code);
            }
            return Results.NoContent();
        }).RequireAuthorization();


        group.MapGet("/reservas_cha", async (int chaDeBebeId, AppDbContext db, ClaimsPrincipal user) =>
        {
            var reservas = await db.Reservas.AsNoTracking()
                .Where(r => r.ChaDeBebeEventoId == chaDeBebeId)
                .Select(r => new
                {
                    r.Id,
                    r.DataReserva,
                    r.Quantidade,
                    r.UsuarioId,
                    r.PresenteId,
                })
                .ToListAsync();

            return Results.Ok(reservas);
        });

        group.MapGet("/reservas_presente", async (int presenteId, AppDbContext db, ClaimsPrincipal user) =>
        {
            var reservas = await db.Presentes.AsNoTracking()
                .Include(p => p.Reservas)
                .Where(p => p.Id == presenteId)
                .Select(p => new
                {
                    p.Id,
                    p.ChaDeBebeEventoId,
                    p.EstaEsgotado,
                    p.QuantidadeTotal,
                    p.QuantidadeRestante,
                    Reservas = p.Reservas.Select(r => new
                    {
                        r.Id,
                        r.Quantidade,
                        r.DataReserva,
                        r.UsuarioId,
                        NomeUsuario = r.Usuario!.Nome
                    }),
                })
                .ToListAsync();

            return Results.Ok(reservas);
        });
    }
}