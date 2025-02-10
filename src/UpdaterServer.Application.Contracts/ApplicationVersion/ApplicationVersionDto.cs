using System;
using System.Collections.Generic;
using UpdaterServer.File;
using Volo.Abp.Application.Dtos;

namespace UpdaterServer.ApplicationVersion;

public class ApplicationVersionDto : FullAuditedEntityDto<Guid>
{
    public string VersionNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}