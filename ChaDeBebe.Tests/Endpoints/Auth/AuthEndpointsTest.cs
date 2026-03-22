using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

public class AuthEndpointsTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFactory _factory;

    public AuthEndpointsTests(IntegrationTestFactory factory)
    {
        // Cria o cliente que "conversa" com a API em memória
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_DeveRetornarUnauthorized_QuandoCredenciaisForemInvalidas()
    {
        var loginInvalido = new { Email = "naoexiste@teste.com", Senha = "123" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginInvalido);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task FluxoAutenticacao_DeveRegistrarELogarUsuarioComSucesso()
    {
        // 1. Dados do novo usuário
        var registroRequest = new
        {
            Nome = "Admin Teste",
            Email = "admin@teste.com",
            Senha = "SenhaForte123!"
        };

        // 2. Registrar o usuário
        var registroResponse = await _client.PostAsJsonAsync("/api/auth/cadastro", registroRequest);

        // 3. Registro com sucesso
        registroResponse.StatusCode.Should().Be(HttpStatusCode.Created);


        // 3.5 Testar o banco de dados
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userNoBanco = db.Usuarios.FirstOrDefault(u => u.Email == "admin@teste.com");

            Assert.NotNull(userNoBanco); // Se falhar aqui, o 201 foi "falso" ou o banco resetou
        }

        // 4. Tentar fazer Login com os mesmos dados
        var loginRequest = new LoginRequest
        (
            "admin@teste.com",
            "SenhaForte123!"
        );
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // 5. Assert: Login deve retornar 200 OK e um Token
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var resultado = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        resultado.Should().NotBeNull();
        resultado!.Token.Should().NotBeNullOrWhiteSpace();

        // 6. Validar o token recebido
        var token = resultado!.Token;
        _client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var empty_body = new { };
        var protegidaResponse = await _client.PostAsJsonAsync("/api/auth/verify-token", empty_body);
        protegidaResponse.StatusCode.Should().Be(HttpStatusCode.OK,
        "O token gerado no login deve permitir acesso a rotas protegidas.");
    }

    [Fact]
    public async Task Registrar_DeveRetornarConflict_QuandoEmailJaExistir()
    {
        var registroRequest = new
        {
            Nome = "Primeiro Registro",
            Email = "duplicado@teste.com",
            Senha = "SenhaForte123!"
        };

        // 1. Registra o usuário
        var primeiroRegistro = await _client.PostAsJsonAsync("/api/auth/cadastro", registroRequest);
        primeiroRegistro.StatusCode.Should().Be(HttpStatusCode.Created);

        // 2. Tenta registrar novamente
        var segundoRegistro = await _client.PostAsJsonAsync("/api/auth/cadastro", registroRequest);

        // 3. Deve resultar em conflito
        segundoRegistro.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var conteudo = await segundoRegistro.Content.ReadAsStringAsync();
        conteudo.Should().Contain("E-mail já cadastrado");
    }

    [Fact]
    public async Task AcessoProtegido_DeveRetornarUnauthorized_QuandoTokenForInvalido()
    {
        var tokenInvalido = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.invalid.payload";
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenInvalido);

        var response = await _client.PostAsJsonAsync("/api/auth/verify-token", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

public record LoginResponseDto(string Token, string Email);