using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class AuthEndpoints
{
    // Injeção de dependência com IEndpointRouteBuilder
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Autenticação");
        group.MapPost("/cadastro", async (RegistroRequest req, AppDbContext db) =>
        {
            var service = new UsuarioService(db);
            var existente = await service.BuscarPorEmail(req.Email);
            if (existente != null) return Results.Conflict("E-mail já cadastrado.");

            var user = await service.Criar(req.Nome, req.Email, req.Senha);
            if (user == null)
            {
                return Results.BadRequest("Nada foi salvo");
            }
            return Results.Created($"/api/usuarios/{user.Id}", new { user.Id, user.Nome });
        });

        // Endpoint de Login
        group.MapPost("/login", async (LoginRequest req, AppDbContext db, TokenService tokenService) =>
        {
            var service = new UsuarioService(db);
            var user = await service.BuscarPorEmail(req.Email);

            // Verificamos o usuário e validamos o Hash da senha
            if (user == null || !user.VerificarSenha(req.Senha))
            {
                return Results.Unauthorized();
            }

            var token = tokenService.GerarToken(user); // Criar JWT

            return Results.Ok(new
            {
                token = token,
                usuario = new { user.Id, user.Nome, user.Email }
            });
        });

        group.MapPost("verify-token", async () =>
        {
            return Results.Ok();
        }).RequireAuthorization();
    }
}