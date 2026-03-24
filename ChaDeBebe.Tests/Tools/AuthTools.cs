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

    public static async Task<(string, int)> FluxoCriarCha(HttpClient _client)
    {
        return await FluxoCriarChaCompleto(_client, "Chá da Alice");
    }

    public static async Task<(string, int)> FluxoCriarChaCompleto(HttpClient _client, string nome)
    {
        string token = await FluxoAuth(_client);
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        var novoCha = new { Nome = nome, DataEvento = DateTime.Now.AddDays(15) };
        var responseCriar = await _client.PostAsJsonAsync("/api/cha_de_bebe/criar", novoCha);
        Assert.Equal(HttpStatusCode.Created, responseCriar.StatusCode);

        var chaCriado = await responseCriar.Content.ReadFromJsonAsync<ChaDeBebeResponse>();
        return (token, chaCriado!.Id);
    }

    public static async Task<int> FluxoCriarPresente(HttpClient _client, string Nome, string desc, int chaId, decimal quantidade, decimal preco)
    {
        var presenteDto = new PresenteDTO(
            Nome,
            desc,
            null,
            null,
            chaId,
            500.00m,
            1m
        );
        var createResponse = await _client.PostAsJsonAsync(
            "/api/presente/adicionar",
            presenteDto
        );
        var createdPresente = await createResponse.Content.ReadFromJsonAsync<Presente>();
        var presenteId = createdPresente?.Id ?? 0;
        Assert.NotEqual(0, presenteId);
        return presenteId;
    }

    public static async Task<(string, int, IList<int>)> FluxoCriarChaCompletoComPresentes(HttpClient _client, string nome)
    {
        string token = await FluxoAuth(_client);
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        var novoCha = new { Nome = nome, DataEvento = DateTime.Now.AddDays(15) };
        var responseCriar = await _client.PostAsJsonAsync("/api/cha_de_bebe/criar", novoCha);
        Assert.Equal(HttpStatusCode.Created, responseCriar.StatusCode);

        var chaCriado = await responseCriar.Content.ReadFromJsonAsync<ChaDeBebeResponse>();

        int presente1_ = await FluxoCriarPresente(_client, "Presente 1", "Descrição 1", chaCriado!.Id, 1m, 500.00m);
        int presente2_ = await FluxoCriarPresente(_client, "Presente 2", "Descrição 2", chaCriado!.Id, 1m, 400.00m);
        int presente3_ = await FluxoCriarPresente(_client, "Presente 3", "Descrição 3", chaCriado!.Id, 30m, 300.00m);
        int presente4_ = await FluxoCriarPresente(_client, "Presente 4", "Descrição 4", chaCriado!.Id, 10m, 200.00m);
        int presente5_ = await FluxoCriarPresente(_client, "Presente 5", "Descrição 5", chaCriado!.Id, 1m, 100.00m);

        // Trocar para conta de usuário
        string token2 = await FluxoAuth(_client, "User teste", "User@email.com", "123");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token2);

        var responseEntrar = await _client.PostAsync($"/api/cha_de_bebe/entrar/{chaCriado.Id}", null);
        Assert.Equal(HttpStatusCode.Accepted, responseEntrar.StatusCode);

        return (token2, chaCriado!.Id, [presente1_, presente2_, presente3_, presente4_, presente5_]);
    }
}