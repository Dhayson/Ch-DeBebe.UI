using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ChaDeBebe.Tests.Services.ChaDeBebeEvento
{
    public class PresenteServiceTest
    {
        private readonly AppDbContext _db;
        private readonly PresenteService _presenteService;
        private readonly ChaDeBebeService _chaDeBebeService;

        public PresenteServiceTest()
        {
            // Setup do Banco em Memória para isolamento total
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _db = new AppDbContext(options);
            _presenteService = new PresenteService(_db);
            _chaDeBebeService = new ChaDeBebeService(_db);
        }

        public async Task<int> CriarChaDeBebe(int adminId, string Nome)
        {
            var request = new CriarChaDeBebeDTO(Nome, DateTime.Now.AddDays(30));

            var resultado = await _chaDeBebeService.Criar(request, adminId);
            return resultado!.Id;
        }

        [Fact]
        public async Task CriarPresente()
        {
            var chaId = await CriarChaDeBebe(123, "Nome Cha");
            var presente = new PresenteDTO("Carrinho de Brinquedo", "Carrinho vermelho", null, null, 3, 50.00m, 0m);

            (Presente? meuPresente, string mes, int code) = await _presenteService.AdicionarPresenteAsync(123, chaId, presente);
            Assert.NotNull(meuPresente);
            Assert.Equal("Carrinho de Brinquedo", meuPresente.Nome);
            Assert.Equal(201, code);

            var presenteBanco = await _db.Presentes.FindAsync(meuPresente.Id);
            Assert.NotNull(presenteBanco);
            Assert.Equal(presenteBanco.Id, meuPresente.Id);
        }

        [Fact]
        public async Task DeletarPresente()
        {
            await CriarPresente();

            // Verificar algum presente no banco
            var presenteId = 1;
            var presenteBanco = await _db.Presentes.FindAsync(presenteId);
            Assert.NotNull(presenteBanco);
            Assert.Equal(presenteBanco.Id, presenteId);

            // Deletar
            var result = await _presenteService.RemoverPresenteAsync(123, 1, presenteId);

            // Verificar ausência
            var naoPresenteBanco = await _db.Presentes.FindAsync(presenteId);
            Assert.Null(naoPresenteBanco);

        }

        [Fact]
        public async Task DeletarPresenteInvalido()
        {
            await CriarPresente();

            // Deletar invalidamente
            var presenteId = 999;
            (var result, string error, int code) = await _presenteService.RemoverPresenteAsync(123, 1, presenteId);

            // Assert
            Assert.False(result);
            Assert.Equal(404, code);
        }

        [Fact]
        public async Task DeletarPresenteInvalido2()
        {
            // Deletar invalidamente, mas com mismatch de AdminId e ChaId
            var presenteId = 999;
            (var result, string error, int code) = await _presenteService.RemoverPresenteAsync(123, 1, presenteId);

            // Assert
            Assert.False(result);
            Assert.Equal(400, code);
        }
    }
}