using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

public class ChaDeBebeEndpointsTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFactory _factory;

    public ChaDeBebeEndpointsTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CriarEntrar_FluxoCompleto()
    {
        // 1. Autenticação
        var token = await AuthTools.FluxoAuth(_client);
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var novoCha = new { Nome = "Chá da Alice", DataEvento = DateTime.Now.AddDays(15) };

        // 2. Testar Endpoint /criar
        var responseCriar = await _client.PostAsJsonAsync("/api/cha_de_bebe/criar", novoCha);

        // 3. Criar
        Assert.Equal(HttpStatusCode.Created, responseCriar.StatusCode);
        var chaCriado = await responseCriar.Content.ReadFromJsonAsync<ChaDeBebeResponse>();
        Assert.NotNull(chaCriado?.Id);

        // 4. Testar Endpoint /entrar
        var responseEntrar = await _client.PostAsync($"/api/cha_de_bebe/entrar/{chaCriado.Id}", null);
        var jsonBruto = await responseEntrar.Content.ReadAsStringAsync();

        // 5. Entrar deve falhar por ser o próprio Admin
        Assert.Equal(HttpStatusCode.BadRequest, responseEntrar.StatusCode);

        // 6. Autenticação para entrar com outro usuário
        var token2 = await AuthTools.FluxoAuth(_client, "Fulano", "fulano@gmail.com", "Senha123456");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token2);
        var responseEntrar2 = await _client.PostAsync($"/api/cha_de_bebe/entrar/{chaCriado.Id}", null);
        Assert.Equal(HttpStatusCode.Accepted, responseEntrar2.StatusCode);
    }

    [Fact]
    public async Task Entrar_ComCodigoInvalido_DeveRetornar404()
    {
        // 1. Autenticação
        var token = await AuthTools.FluxoAuth(_client);
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Requisicão
        int id_aleatoria = 4567;
        var response = await _client.PostAsync($"/api/chas/entrar/{id_aleatoria}", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Entrar_DuasVezes_DeveRetornarOk()
    {
        // 1. Autenticação: Admin cria o evento
        var token = await AuthTools.FluxoAuth(_client);
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var novoCha = new { Nome = "Chá da Maria", DataEvento = DateTime.Now.AddDays(20) };
        var responseCriar = await _client.PostAsJsonAsync("/api/cha_de_bebe/criar", novoCha);
        var chaCriado = await responseCriar.Content.ReadFromJsonAsync<ChaDeBebeResponse>();

        // 2. Autenticação: Outro usuário
        var token2 = await AuthTools.FluxoAuth(_client, "Beltrano", "beltrano@gmail.com", "Senha123456");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token2);

        // 3. Primeira entrada: deve ter sucesso
        var responseEntrar1 = await _client.PostAsync($"/api/cha_de_bebe/entrar/{chaCriado.Id}", null);
        Assert.Equal(HttpStatusCode.Accepted, responseEntrar1.StatusCode);

        // 4. Segunda entrada: deve ser idempotente
        var responseEntrar2 = await _client.PostAsync($"/api/cha_de_bebe/entrar/{chaCriado.Id}", null);
        Assert.Equal(HttpStatusCode.OK, responseEntrar2.StatusCode);
    }
}


public record ChaDeBebeResponse(int Id, string Nome, DateTime DataEvento, int AdminId);
