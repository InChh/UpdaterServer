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
            // Check version number is valid, it should be in a specific format(xx.xx.xx).
            var strings = value.Split('.');
            if (strings.Length != 3)
            {
                throw new BusinessException(
                    ApplicationVersionErrorCodes.VersionNumberInvalid).WithData("versionNumber", value);
            }

            // The first number of version should be current year and the second number should be current month.
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString();
            if (strings[0] != year || strings[1] != month)
            {
                throw new BusinessException(
                    ApplicationVersionErrorCodes.VersionNumberInvalid).WithData("versionNumber", value);
            }

            // The third number should be greater than 0.
            if (int.Parse(strings[2]) <= 0)
            {
                throw new BusinessException(
                    ApplicationVersionErrorCodes.VersionNumberInvalid).WithData("versionNumber", value);
            }

            _versionNumber = value;
        }
    }

    public string? Description { get; set; }

    public ICollection<VersionFile> Files { get; private set; } = [];

    public bool IsActive { get; set; }

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
}