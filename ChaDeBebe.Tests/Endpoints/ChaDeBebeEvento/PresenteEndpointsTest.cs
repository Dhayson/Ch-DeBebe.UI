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
        var deleteDto = new ReqPresenteDTO(chaId, pId);
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

    [Fact]
    public async Task AtualizarPresente_FluxoCompleto()
    {
        // 1. Autenticação
        (var token, var chaId) = await AuthTools.FluxoCriarChaCompleto(_client, "Chá do Edu");

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

        var createdPresente = await createResponse.Content.ReadFromJsonAsync<Presente>();
        var presenteId = createdPresente?.Id ?? 0;
        Assert.NotEqual(0, presenteId);

        // 3. Atualizar presente
        var updateDto = new
        {
            Nome = "Berço Premium",
            ChaDeBebeEventoId = chaId,
        };

        var updateResponse = await _client.PostAsJsonAsync(
            $"/api/presente/atualizar/{presenteId}",
            updateDto
        );

        updateResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var updatedPresente = await updateResponse.Content.ReadFromJsonAsync<Presente>();
        updatedPresente?.Nome.Should().Be("Berço Premium");
    }

}


public record PresenteResponse(int Id, string Nome, DateTime DataEvento, int AdminId);
