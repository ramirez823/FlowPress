
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowPress.Models;

public class Secret
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Key { get; set; } = string.Empty;   // Ej: "ApiKey", "ClientSecret"

    [Required]
    public string Value { get; set; } = string.Empty; // El valor secreto (encriptado)

    // Relación opcional con una fuente (si el secreto pertenece a una fuente específica)
    public int? SourceId { get; set; }
    public Source? Source { get; set; }

    // Metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
