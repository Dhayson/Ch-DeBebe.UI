using System;
using System.Collections.Generic;
using System.Threading.Tasks;


public class PresenteService
{
    private readonly AppDbContext _db;

    public PresenteService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(Presente?, string, int)> AdicionarPresenteAsync(int AdminId, int ChaDeBebeEventoId, PresenteDTO presente)
    {
        var cha = await _db.ChasDeBebe.FindAsync(ChaDeBebeEventoId);
        if (cha == null)
        {
            return (null, "Evento Inválido", 404);
        }
        if (cha.AdminId != AdminId)
        {
            return (null, "Ação não autorizada", 401);
        }
        var novoPresente = new Presente
        {
            Nome = presente.Nome,
            Descricao = presente.Descricao,
            Preco = presente.Preco,
            ChaDeBebeEventoId = ChaDeBebeEventoId,
            LinkSugerido = presente.LinkSugerido,
            PathImage = presente.PathImage,
            QuantidadeTotal = presente.QuantidadeTotal,
        };

        _db.Presentes.Add(novoPresente);
        await _db.SaveChangesAsync();

        return (novoPresente, "Presente adicionado com sucesso", 201);
    }

    public async Task<(bool, string, int)> RemoverPresenteAsync(int AdminId, int ChaDeBebeEventoId, int presenteId)
    {
        var cha = await _db.ChasDeBebe.FindAsync(ChaDeBebeEventoId);
        if (cha == null || cha.AdminId != AdminId)
        {
            return (false, "Não Autorizado", 400);
        }

        var presenteExistente = await _db.Presentes.FindAsync(presenteId);
        if (presenteExistente == null)
        {
            return (false, "Não Encontrado", 404);
        }

        _db.Presentes.Remove(presenteExistente);
        await _db.SaveChangesAsync();

        return (true, "Presente Deletado", 204);
    }
}
