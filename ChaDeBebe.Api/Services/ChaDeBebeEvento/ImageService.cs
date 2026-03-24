using Microsoft.Extensions.Configuration;

public class ImageService
{
    private readonly string _storagePath;

    public ImageService(IConfiguration config)
    {
        _storagePath = config["StorageConfig:Path"] ?? "/app/uploads/presentes";
        if (!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
    }

    public async Task<string> SalvarImagem(IFormFile arquivo, int presenteId)
    {
        // Nomeamos o arquivo com o ID para facilitar a recuperação/substituição
        var extensao = Path.GetExtension(arquivo.FileName);
        var nomeArquivo = $"{presenteId}{extensao}";
        var caminhoCompleto = Path.Combine(_storagePath, nomeArquivo);

        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
        {
            await arquivo.CopyToAsync(stream);
        }

        return nomeArquivo; // Retornamos apenas o nome para salvar no banco
    }

    public string? RecuperarImagem(string PathImage)
    {
        // 1. Monta o caminho completo usando a configuração do Storage
        var filePath = Path.Combine(_storagePath, PathImage);

        // 2. Verifica se o arquivo físico realmente existe no disco/volume
        if (!File.Exists(filePath))
        {
            return null;
        }

        // 3. Determina o Content-Type (ex: image/jpeg ou image/png)
        var extensao = Path.GetExtension(filePath).ToLowerInvariant();
        var contentType = extensao switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
        return contentType;
    }
}