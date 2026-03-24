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
        (var token, var chaId) = await AuthTools.FluxoCriarChaCompletoComPresentes(_client, "Chá do Edu");
        var reservaRequest = new ReservaDTO(1m, DateTime.Now, 2, chaId, 1);
        var response = await _client.PostAsJsonAsync("/api/reserva/adicionar", reservaRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var reserva = await response.Content.ReadFromJsonAsync<ReservaResponse>();
        reserva.Should().NotBeNull();
    }
    // TODO: mais testes
}


public record ReservaResponse(int Id, string Nome, DateTime DataEvento, int AdminId);
