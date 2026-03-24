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
    }
}