using System;
using System.Collections.Generic;
using System.Linq;
using UpdaterServer.File;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace UpdaterServer.ApplicationVersion;

public class ApplicationVersion : FullAuditedAggregateRoot<Guid>
{
    public Guid ApplicationId { get; private set; }

    private string _versionNumber = string.Empty;

    public string VersionNumber
    {
        get => _versionNumber;
        set
        {
            value.CheckVersionNumber();
            _versionNumber = value;
        }
    }

    public string? Description { get; set; }

    public ICollection<VersionFile> Files { get; private set; } = [];

    public bool IsActive { get; private set; }

    private ApplicationVersion()
    {
    }

    public ApplicationVersion(Guid id, Guid applicationId, string versionNumber, string? description) :
        base(id)
    {
        ApplicationId = applicationId;
        VersionNumber = versionNumber;
        Description = description;
        IsActive = false;
    }

    public void AddFile(Guid fileMetadataId)
    {
        Check.NotNull(fileMetadataId, nameof(fileMetadataId));

        if (IsInFile(fileMetadataId))
        {
            return;
        }

        Files.Add(new VersionFile(Id, fileMetadataId));
    }

    public void AddFiles(ICollection<Guid> fileMetadataIds)
    {
        Check.NotNullOrEmpty(fileMetadataIds, nameof(fileMetadataIds));

        foreach (var fileMetadataId in fileMetadataIds)
        {
            AddFile(fileMetadataId);
        }
    }

    public void RemoveFile(Guid fileMetadataId)
    {
        Check.NotNull(fileMetadataId, nameof(fileMetadataId));

        if (!IsInFile(fileMetadataId))
        {
            return;
        }

        Files.RemoveAll(x => x.FileMetadataId == fileMetadataId);
    }

    public void RemoveAllFiles()
    {
        Files.RemoveAll(x => x.VersionId == Id);
    }

    public void RemoveAllFilesExceptGivenIds(List<Guid> fileMetadataIds)
    {
        Check.NotNullOrEmpty(fileMetadataIds, nameof(fileMetadataIds));
        Files.RemoveAll(x => !fileMetadataIds.Contains(x.FileMetadataId));
    }


    private bool IsInFile(Guid fileMetadataId)
    {
        return Files.Any(f => f.FileMetadataId == fileMetadataId);
    }

    public void Active()
    {
        IsActive = true;
    }
    
    public void Deactive()
    {
        IsActive = false;
    }
}