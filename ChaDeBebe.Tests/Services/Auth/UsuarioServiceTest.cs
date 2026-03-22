using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

public class UsuarioServiceTests
{
    [Fact]
    public async Task BuscarPorEmail_DeveRetornarUsuario_QuandoEmailExistir()
    {
        var db = InMemDatabase.GetDatabase();
        var service = new UsuarioService(db);
        var email = "teste@exemplo.com";
        db.Usuarios.Add(new Usuario("Teste", email, "123456"));
        await db.SaveChangesAsync();

        var resultado = await service.BuscarPorEmail(email);

        resultado.Should().NotBeNull();
        resultado!.Email.Should().Be(email);
    }

    [Fact]
    public async Task BuscarPorEmail_DeveRetornarNull_QuandoEmailNaoExistir()
    {
        var db = InMemDatabase.GetDatabase();
        var service = new UsuarioService(db);

        var resultado = await service.BuscarPorEmail("fantasma@teste.com");

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task BuscarPorId_DeveRetornarUsuario_QuandoIdExistir()
    {
        var db = InMemDatabase.GetDatabase();
        var service = new UsuarioService(db);
        var usuario = new Usuario("Teste", "teste@exemplo.com", "123456");
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var resultado = await service.BuscarPorId(usuario.Id);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(usuario.Id);
        resultado.Email.Should().Be("teste@exemplo.com");
    }

    [Fact]
    public async Task BuscarPorId_DeveRetornarNull_QuandoIdNaoExistir()
    {
        var db = InMemDatabase.GetDatabase();
        var service = new UsuarioService(db);

        var resultado = await service.BuscarPorId(999);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task Criar_DeveCriarNovoUsuario_ComDadosValidos()
    {
        var db = InMemDatabase.GetDatabase();
        var service = new UsuarioService(db);

        var resultado = await service.Criar("Novo User", "novo@exemplo.com", "senha123");

        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("Novo User");
        resultado.Email.Should().Be("novo@exemplo.com");
        var usuarioSalvo = await db.Usuarios.FindAsync(resultado.Id);
        usuarioSalvo.Should().NotBeNull();
    }

    [Fact]
    public async Task Criar_DeveFalhar_QuandoCriarDoisUsuariosComMesmoEmail()
    {
        var db = InMemDatabase.GetDatabase();
        var service = new UsuarioService(db);
        var email = "duplicado@exemplo.com";

        var primeiro = await service.Criar("Usuario Um", email, "senha123");
        var segundo = await service.Criar("Usuario Dois", email, "senha456");

        primeiro.Should().NotBeNull();
        segundo.Should().BeNull();
        var usuariosComEmail = await db.Usuarios.Where(u => u.Email == email).CountAsync();
        usuariosComEmail.Should().Be(1);
    }

    [Fact]
    public async Task Criar_DeveArmazenarSenhaComHash_NaoComSenhaEmTextoPlano()
    {
        var db = InMemDatabase.GetDatabase();
        var service = new UsuarioService(db);
        var senhaPlana = "minhaSenha123";

        var resultado = await service.Criar("Novo User", "hash@exemplo.com", senhaPlana);

        resultado.Should().NotBeNull();
        resultado!.HashSenha.Should().NotBe(senhaPlana);
        resultado.HashSenha.Should().NotBeNullOrEmpty();
    }
}