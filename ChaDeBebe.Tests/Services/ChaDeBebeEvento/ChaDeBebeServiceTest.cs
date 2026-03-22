using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;

public class ChaDeBebeServiceTest
{
    private readonly AppDbContext _db;
    private readonly ChaDeBebeService _service;

    public ChaDeBebeServiceTest()
    {
        // Setup do Banco em Memória para isolamento total
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _service = new ChaDeBebeService(_db);
    }

    [Fact]
    public async Task Criar_DeveVincularAdminCorretamente_ESalvarNoBanco()
    {
        var adminId = 123;
        var request = new CriarChaDeBebeDTO("Chá do Ben", DateTime.Now.AddDays(30));

        var resultado = await _service.Criar(request, adminId);

        Assert.NotNull(resultado);
        Assert.Equal(adminId, resultado.AdminId);
        Assert.Equal("Chá do Ben", resultado.Nome);

        // Verifica se persistiu de fato no "banco"
        var chaNoBanco = await _db.ChasDeBebe.FindAsync(resultado.Id);
        Assert.NotNull(chaNoBanco);
        Assert.Equal(adminId, chaNoBanco.AdminId);
    }

    [Fact]
    public async Task InscreverConvidado_NaoDevePermitirQueAdminSeInscrevaNoProprioCha()
    {
        var adminId = 1;
        var cha = new ChaDeBebeEvento(adminId, "Meu Chá", DateTime.Now.AddDays(20));
        _db.ChasDeBebe.Add(cha);
        await _db.SaveChangesAsync();

        (ChaDeBebeEvento? meu_cha, string error, int code) = await _service.Entrar(cha.Id, adminId);

        Assert.Equal("Você é o dono deste evento.", error);
        Assert.Equal(400, code);
        Assert.Null(meu_cha);
    }

    [Fact]
    public async Task InscreverConvidado_DeveLancarErro_QuandoCodigoDeConviteNaoExistir()
    {
        var codigoInexistente = 456;
        var usuarioId = 99;

        // Validamos que o serviço lança uma exceção (ou retorna null/erro) quando o convite é falso
        (ChaDeBebeEvento? meu_cha, string error, int code) = await
            _service.Entrar(codigoInexistente, usuarioId);

        Assert.Equal("Convite inválido.", error);
        Assert.Equal(404, code);
        Assert.Null(meu_cha);
    }

    [Fact]
    public async Task InscreverConvidado_DeveLancarAviso_QuandoUsuarioJaEstiverInscrito()
    {
        var adminId = 1;
        var convidadoId = 2;
        var cha = new ChaDeBebeEvento(adminId, "Meu Chá de Teste", DateTime.Now.AddDays(20));
        _db.ChasDeBebe.Add(cha);

        // Simula que o usuário já se inscreveu uma vez
        _db.UsuarioChaDeBebe.Add(new UsuarioChaDeBebe
        {
            UsuarioId = convidadoId,
            ChaDeBebeId = cha.Id
        });

        await _db.SaveChangesAsync();

        // Tentativa de inscrição repetida deve manter a integridade
        (ChaDeBebeEvento? meu_cha, string error, int code) = await
            _service.Entrar(cha.Id, convidadoId);

        Assert.Equal("Você já faz parte deste chá!", error);
        Assert.Equal(200, code);
    }
}