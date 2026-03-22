using Moq;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly Mock<IConfiguration> _configMock;

    public TokenServiceTests()
    {
        // Mock das configurações do appsettings.json
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(x => x["JwtSettings:Secret"]).Returns("Chave_Super_Secreta_De_Teste_Com_32_Chars");
        _configMock.Setup(x => x["JwtSettings:ExpiracaoHoras"]).Returns("1");

        _tokenService = new TokenService(_configMock.Object);
    }

    [Fact]
    public void GerarToken_DeveRetornarTokenValido_QuandoUsuarioForPassado()
    {
        var usuario = new Usuario("Dev", "dev@techshare.com", "Senha123");

        var token = _tokenService.GerarToken(usuario);

        token.Should().NotBeNullOrWhiteSpace();

        // Decodificando para verificar o conteúdo
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Issuer.Should().BeNull(); // Como não definimos Issuer no config, deve ser null
        jwtToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == usuario.Email);
        jwtToken.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == usuario.Nome);
    }
}