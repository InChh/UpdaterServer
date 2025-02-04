using System;
using Volo.Abp.Application.Dtos;

namespace UpdaterServer.Application;

public class ApplicationDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}