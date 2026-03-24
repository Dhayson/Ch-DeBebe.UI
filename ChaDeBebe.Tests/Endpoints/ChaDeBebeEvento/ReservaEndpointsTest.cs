using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

public class ReservaEndpointsTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFactory _factory;

    public ReservaEndpointsTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CriarReserva_FluxoCompleto()
    {
        (var token, var chaId, var presenteIds) = await AuthTools.FluxoCriarChaCompletoComPresentes(_client, "Chá do Edu");
        var reservaRequest = new ReservaDTO(1m, DateTime.Now, 2, chaId, presenteIds[1]);
        var response = await _client.PostAsJsonAsync("/api/reserva/adicionar", reservaRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var reserva = await response.Content.ReadFromJsonAsync<ReservaResponse>();
        reserva.Should().NotBeNull();
    }
    [Fact]
    public async Task CriarReserva_SemAutorizacao_RetornaUnauthorized()
    {
        var reservaRequest = new ReservaDTO(1m, DateTime.Now, 2, 1, 1);
        var response = await _client.PostAsJsonAsync("/api/reserva/adicionar", reservaRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriarReserva_UsuarioIdNaoCorresponde_RetornaUnauthorized()
    {
        (var token, var chaId, var presenteIds) = await AuthTools.FluxoCriarChaCompletoComPresentes(_client, "Chá da Letícia");
        var reservaRequest = new ReservaDTO(1m, DateTime.Now, 999, chaId, 999);
        var response = await _client.PostAsJsonAsync("/api/reserva/adicionar", reservaRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriarReserva_PresenteIdNaoCorresponde_RetornaNotFound()
    {
        (var token, var chaId, var presenteIds) = await AuthTools.FluxoCriarChaCompletoComPresentes(_client, "Chá do Leo");
        var reservaRequest = new ReservaDTO(1m, DateTime.Now, 2, chaId, 999);
        var response = await _client.PostAsJsonAsync("/api/reserva/adicionar", reservaRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletarReserva_FluxoCompleto()
    {
        (var token, var chaId, var presenteIds) = await AuthTools.FluxoCriarChaCompletoComPresentes(_client, "Chá do Rafael");
        var reservaRequest = new ReservaDTO(1m, DateTime.Now, 2, chaId, presenteIds[1]);
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var createResponse = await _client.PostAsJsonAsync("/api/reserva/adicionar", reservaRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var reserva = await createResponse.Content.ReadFromJsonAsync<ReservaResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/reserva/deletar/{reserva!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletarReserva_IdInexistente_RetornaErro()
    {
        (var token, var chaId, var presenteIds) = await AuthTools.FluxoCriarChaCompletoComPresentes(_client, "Chá do Carlos");
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.DeleteAsync("/api/reserva/deletar/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}


public record ReservaResponse(int Id, string Nome, DateTime DataEvento, int AdminId);
