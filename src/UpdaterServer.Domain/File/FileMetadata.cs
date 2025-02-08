using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace UpdaterServer.File;

public sealed class FileMetadata : FullAuditedAggregateRoot<Guid>
{
    public string Path { get; set; }
    public string Hash { get; set; }
    public long Size { get; set; }
    public string Url { get; set; }
    
    private FileMetadata()
    {
    }
    
    public FileMetadata(Guid id, string path, string hash, long size, string url)
    {
        Id = id;
        Path = path;
        Hash = hash;
        Size = size;
        Url = url;
    }
}