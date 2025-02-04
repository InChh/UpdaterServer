using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace UpdaterServer.File;

public class FileMetadata : FullAuditedAggregateRoot<Guid>
{
    public string Path { get; set; }
    public string Hash { get; set; }
    public long Size { get; set; }
    public string Url { get; set; }
}