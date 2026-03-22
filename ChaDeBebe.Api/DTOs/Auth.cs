using System.ComponentModel.DataAnnotations;

public record RegistroRequest(
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100)]
    string Nome,

    [Required]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string Email,

    [Required]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    string Senha
);

public record LoginRequest(
    [Required]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string Email,

    [Required]
    string Senha
);