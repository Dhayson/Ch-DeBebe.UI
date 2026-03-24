using System.ComponentModel.DataAnnotations;

public record PresenteDTO(
    [Required]
    string Nome,
    string? Descricao,
    string? LinkSugerido,
    string? PathImage,

    [Required]
    int ChaDeBebeEventoId,
    decimal? QuantidadeTotal,
    decimal? Preco
);

public record ReqPresenteDTO(
    [Required]
    int ChaDeBebeEventoId,
    [Required]
    int presenteId
);