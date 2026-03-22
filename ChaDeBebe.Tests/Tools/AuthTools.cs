using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

public static class AuthTools
{
    public static async Task<string> FluxoAuth(HttpClient _client, string Nome, string Email, string Senha)
    {
        // 1. Dados do novo usuário
        var registroRequest = new
        {
            Nome = Nome,
            Email = Email,
            Senha = Senha
        };

        // 2. Registrar o usuário
        var registroResponse = await _client.PostAsJsonAsync("/api/auth/cadastro", registroRequest);

        // 3. Login com os mesmos dados
        var loginRequest = new LoginRequest
        (
            Email,
            Senha
        );
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var resultado = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        // 4. Retornar o token recebido
        return resultado!.Token;
    }
    public static async Task<string> FluxoAuth(HttpClient _client)
    {
        return await FluxoAuth(
            _client,
            "Admin Teste",
            "admin@teste.com",
            "SenhaForte123!"
        );
    }
}