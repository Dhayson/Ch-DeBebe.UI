using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

public class PresenteEndpointsTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFactory _factory;

    public PresenteEndpointsTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CriarPresente_FluxoCompleto()
    {
        // 1. Autenticação
        (var token, var chaId) = await AuthTools.FluxoCriarCha(_client);
        // 2. Criar presente
        var presenteDto = new PresenteDTO(
            "Berço",
            "Berço de madeira",
            null,
            null,
            chaId,
            500.00m,
            0m
        );

        var createResponse = await _client.PostAsJsonAsync(
            "/api/presente/adicionar",
            presenteDto
        );

        createResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var createdPresente = await createResponse.Content.ReadFromJsonAsync<Presente>();
        createdPresente.Should().NotBeNull();

        // 3. Validar resposta
        var presenteId = createdPresente?.Id;
        presenteId.Should().NotBeNull();
        var pId = presenteId ?? 0;
        Assert.NotEqual(0, pId);
        // 4. Deletar presente
        var deleteDto = new DelPresenteDTO(chaId, pId);
        var deleteResponse = await _client.PostAsJsonAsync(
            $"/api/presente/deletar",
            deleteDto
        );

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 5. Teste sem autorização
        _client.DefaultRequestHeaders.Remove("Authorization");
        var unauthorizedResponse = await _client.PostAsJsonAsync("/api/presente/adicionar", presenteDto);
        unauthorizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

}


public record PresenteResponse(int Id, string Nome, DateTime DataEvento, int AdminId);
