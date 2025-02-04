using System;
using Volo.Abp.Domain.Entities;

namespace UpdaterServer.File;

public class VersionFile : Entity
{
    public Guid VersionId { get; protected set; }
    public Guid FileMetadataId { get; protected set; }

    private VersionFile()
    {
    }

    public VersionFile(Guid versionId, Guid fileMetadataId)
    {
        VersionId = versionId;
        FileMetadataId = fileMetadataId;
    }


    public override object[] GetKeys()
    {
        return [VersionId, FileMetadataId];
    }
}