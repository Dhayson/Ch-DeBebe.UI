public class UsuarioTests
{
    [Fact]
    public void Construtor_DeveFazerHashDaSenha_QuandoCriarNovoUsuario()
    {
        var senhaPura = "MinhaSenhaSuperSegura123";

        var usuario = new Usuario("Teste", "teste@email.com", senhaPura);

        usuario.HashSenha.Should().NotBe(senhaPura); // Garante que não salvou em texto puro
        usuario.HashSenha.Should().StartWith("$2a$"); // Garante que usou BCrypt
    }

    [Fact]
    public void VerificarSenha_DeveRetornarTrue_QuandoSenhaForCorreta()
    {
        var usuario = new Usuario("Teste", "teste@email.com", "Senha123");

        var isValida = usuario.VerificarSenha("Senha123");
        var isValidaIncorreta = usuario.VerificarSenha("SenhaErrada");

        isValida.Should().BeTrue();
        isValidaIncorreta.Should().BeFalse();
    }
}