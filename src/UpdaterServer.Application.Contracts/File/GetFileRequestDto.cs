using System.ComponentModel.DataAnnotations;

namespace UpdaterServer.File;

public class GetFileRequestDto
{
    [Required] public string Hash { get; set; } = string.Empty;
    public long? Size { get; set; }
    public string? Path { get; set; }
}