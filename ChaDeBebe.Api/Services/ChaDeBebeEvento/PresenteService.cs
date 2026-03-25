using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


public class PresenteService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public PresenteService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
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
            Preco = presente.Preco ?? 0,
            ChaDeBebeEventoId = ChaDeBebeEventoId,
            LinkSugerido = presente.LinkSugerido,
            PathImage = presente.PathImage,
            QuantidadeTotal = presente.QuantidadeTotal ?? 0,
        };

        _db.Presentes.Add(novoPresente);
        await _db.SaveChangesAsync();

        return (novoPresente, "Presente adicionado com sucesso", 201);
    }

    public async Task<(Presente?, string, int)> AtualizarPresenteAsync(
        int AdminId,
        int ChaDeBebeEventoId,
        int presenteId,
        PresenteDTO presente
    )
    {
        var cha = await _db.ChasDeBebe.FindAsync(ChaDeBebeEventoId);
        if (cha == null || cha.AdminId != AdminId)
        {
            return (null, "Não Autorizado", 400);
        }

        var presenteExistente = await _db.Presentes.FindAsync(presenteId);
        if (presenteExistente == null)
        {
            return (null, "Não Encontrado", 404);
        }
        presenteExistente.Nome = presente.Nome ?? presenteExistente.Nome;
        presenteExistente.Descricao = presente.Descricao ?? presenteExistente.Descricao;
        presenteExistente.Preco = presente.Preco > 0 ? (presente.Preco ?? 0) : presenteExistente.Preco;
        presenteExistente.LinkSugerido = presente.LinkSugerido ?? presenteExistente.LinkSugerido;
        presenteExistente.QuantidadeTotal = presente.QuantidadeTotal > 0 ? (presente.QuantidadeTotal ?? 0) : presenteExistente.QuantidadeTotal;

        _db.Presentes.Update(presenteExistente);
        await _db.SaveChangesAsync();

        return (presenteExistente, "Presente atualizado com sucesso", 201);
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

    public async Task<(string?, string, int)> AdicionarImagemPresenteAsync(
        int AdminId,
        int ChaDeBebeEventoId,
        int presenteId,
        IFormFile imagem
    )
    {
        var cha = await _db.ChasDeBebe.FirstOrDefaultAsync(cha => cha.Id == ChaDeBebeEventoId);
        Console.WriteLine(ChaDeBebeEventoId);
        if (cha == null)
        {
            return (null, "Não Autorizado 1", 400);
        }
        if (cha.AdminId != AdminId)
        {
            return (null, "Não Autorizado 2", 400);
        }

        var presenteExistente = await _db.Presentes.FindAsync(presenteId);
        if (presenteExistente == null)
        {
            return (null, "Não Encontrado", 404);
        }

        if (imagem.Length == 0) return (null, "Arquivo vazio.", 400);

        var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();

        if (!extensoesPermitidas.Contains(extensao))
            return (null, "Formato de imagem inválido.", 400);

        var storagePath = _config["StorageConfig:Path"] ?? "uploads/presentes";
        if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);

        // Geramos um nome único para evitar conflitos e cache antigo
        var nomeArquivo = $"presente_{presenteId}_{Guid.NewGuid()}{extensao}";
        var caminhoCompleto = Path.Combine(storagePath, nomeArquivo);

        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
        {
            await imagem.CopyToAsync(stream);
        }

        // 4. Atualização do Banco de Dados
        // Se já existia uma foto antiga, você pode opcionalmente deletar o arquivo aqui
        presenteExistente.PathImage = nomeArquivo;
        await _db.SaveChangesAsync();

        return (caminhoCompleto, "Sucesso", 200);
    }
}
