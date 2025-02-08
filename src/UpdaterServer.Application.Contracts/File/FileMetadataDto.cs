using System;
using Volo.Abp.Application.Dtos;

namespace UpdaterServer.File;

public class FileMetadataDto: FullAuditedEntityDto<Guid>
{
    public string Path { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Url { get; set; } = string.Empty;
}