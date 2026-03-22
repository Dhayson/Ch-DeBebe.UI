using Microsoft.EntityFrameworkCore;

public class ChaDeBebeService
{
    private readonly AppDbContext _db;

    public ChaDeBebeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ChaDeBebeEvento?> Criar(CriarChaDeBebeDTO dto, int UserId)
    {
        var novoCha = new ChaDeBebeEvento(UserId, dto.Nome, dto.DataEvento);

        var eventoExistente = await _db.ChasDeBebe
            .FirstOrDefaultAsync(e => e.AdminId == UserId && e.Nome == dto.Nome);

        if (eventoExistente != null)
            return null;

        _db.ChasDeBebe.Add(novoCha);
        await _db.SaveChangesAsync();
        return novoCha;
    }

    public async Task<(ChaDeBebeEvento?, string, int)> Entrar(int ChaDeBebeId, int UsuarioId)
    {
        // 1. Busca o Chá pelo Código de Convite
        var cha = await _db.ChasDeBebe
            .FirstOrDefaultAsync(c => c.Id == ChaDeBebeId);

        if (cha is null)
            return (null, "Convite inválido.", 404);

        // 2. Regra de Negócio: Admin não se convida
        if (cha.AdminId == UsuarioId)
            return (null, "Você é o dono deste evento.", 400);

        // 3. Verifica duplicidade (Idempotência)
        var jaInscrito = await _db.UsuarioChaDeBebe
            .AnyAsync(p => p.UsuarioId == UsuarioId && p.ChaDeBebeId == cha.Id);

        if (jaInscrito)
            return (cha, "Você já faz parte deste chá!", 200);

        // 4. Persistência
        _db.UsuarioChaDeBebe.Add(new UsuarioChaDeBebe
        {
            UsuarioId = UsuarioId,
            ChaDeBebeId = cha.Id
        });

        await _db.SaveChangesAsync();

        return (cha, "Inscrição realizada com sucesso!", 202);
    }
}