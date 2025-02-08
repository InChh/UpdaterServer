using System.ComponentModel.DataAnnotations;

namespace UpdaterServer.File;

public class CreateFileMetadataDto
{
    [Required]
    [StringLength(FileMetadataConsts.MaxPathLength)]
    public string Path { get; set; } = string.Empty;

    [Required]
    [StringLength(FileMetadataConsts.MaxHashLength)]
    public string Hash { get; set; } = string.Empty;

    [Required] public long Size { get; set; }

    [Required]
    [StringLength(FileMetadataConsts.MaxUrlLength)]
    public string Url { get; set; } = string.Empty;
}