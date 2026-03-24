using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

namespace ChaDeBebe.Tests.Services.ChaDeBebeEvento
{
    public class ReservaServiceTest
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly PresenteService _presenteService;
        private readonly ReservaService _ReservaService;
        private readonly ChaDeBebeService _chaDeBebeService;

        public ReservaServiceTest()
        {
            // Setup do Banco em Memória para isolamento total
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _db = new AppDbContext(options);
            _config = new ConfigurationBuilder().Build();
            _presenteService = new PresenteService(_db, _config);
            _ReservaService = new ReservaService(_db, _config);
            _chaDeBebeService = new ChaDeBebeService(_db);
        }

        public async Task<(int, IList<Presente?>)> CriarChaDeBebeComPresentes(int adminId, string Nome)
        {
            var request = new CriarChaDeBebeDTO(Nome, DateTime.Now.AddDays(30));

            var resultado = await _chaDeBebeService.Criar(request, adminId);
            var presente1 = new PresenteDTO("Berço", null, null, null, resultado!.Id, 1m, 10m);
            var presente2 = new PresenteDTO("Carrinho", null, null, null, resultado!.Id, 1m, 20m);
            var presente3 = new PresenteDTO("Fraldas", null, null, null, resultado!.Id, 30m, 1m);

            (var presente1_, _, _) = await _presenteService.AdicionarPresenteAsync(adminId, resultado!.Id, presente1);
            (var presente2_, _, _) = await _presenteService.AdicionarPresenteAsync(adminId, resultado!.Id, presente2);
            (var presente3_, _, _) = await _presenteService.AdicionarPresenteAsync(adminId, resultado!.Id, presente3);

            return (resultado!.Id, [presente1_, presente2_, presente3_]);
        }
        [Fact]
        public async Task CriarReserva_ComDadosValidos_DeveRetornarSucesso()
        {
            int userId = 2;
            (int chaDeBebeId, var presentes) = await CriarChaDeBebeComPresentes(userId, "Cha de Bebe Teste");
            await _chaDeBebeService.Entrar(chaDeBebeId, 1);

            var criarReservaDTO = new ReservaDTO(1m, DateTime.Now, 1, chaDeBebeId, presentes[2]!.Id);
            (var resultado, string error, int code) = await _ReservaService.AdicionarReservaAsync(criarReservaDTO);
            Assert.Equal(200, code);
            Assert.NotNull(resultado);
            Assert.Equal(1m, resultado.Quantidade);
            var presente = resultado.Presente!;
            Assert.Equal(1, presente!.Reservas.Count);
            Assert.Equal(29m, presente.QuantidadeRestante);
            var presente_db = await _db.Presentes.FindAsync(resultado.Presente!.Id);
            Assert.Equal(1, presente_db!.Reservas.Count);
            Assert.Equal(29m, presente_db.QuantidadeRestante);
        }

        [Fact]
        public async Task DeletarReserva_ComIdValido_DeveRetornarSucesso()
        {
            int adminId = 1;
            int userId = 2;
            (int chaDeBebeId, var presentes) = await CriarChaDeBebeComPresentes(adminId, "Cha de Bebe Teste");
            await _chaDeBebeService.Entrar(chaDeBebeId, userId);

            var criarReservaDTO = new ReservaDTO(1m, DateTime.Now, userId, chaDeBebeId, presentes[1]!.Id);
            (var resultado, string error, int code) = await _ReservaService.AdicionarReservaAsync(criarReservaDTO);
            Assert.Equal(200, code);
            Assert.NotNull(resultado);
            Assert.NotEqual(0, resultado!.Id);

            (var resultado2, string error2, int code2) = await _ReservaService.DeletarReservaAsync(resultado!.Id, userId);
            Assert.True(resultado2);

            var presenteAfterDelete = await _db.Presentes.FirstOrDefaultAsync(p => p.Id == presentes[1]!.Id);
            Assert.Empty(presenteAfterDelete!.Reservas);
        }

        [Fact]
        public async Task CriarReserva_ComQuantidadeInsuficiente_DeveRetornarErro()
        {
            int userId = 2;
            (int chaDeBebeId, var presentes) = await CriarChaDeBebeComPresentes(userId, "Cha de Bebe Teste");
            await _chaDeBebeService.Entrar(chaDeBebeId, 1);

            var criarReservaDTO = new ReservaDTO(100m, DateTime.Now, 1, chaDeBebeId, presentes[1]!.Id);
            (var resultado, string error, int code) = await _ReservaService.AdicionarReservaAsync(criarReservaDTO);

            Assert.Null(resultado);
            Assert.Equal(409, code);
        }
        // TODO: mais testes
    }
}