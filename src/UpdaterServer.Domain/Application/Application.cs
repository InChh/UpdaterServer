using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace UpdaterServer.Application;

public sealed class Application: FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    private Application()
    {
    }

    public Application(Guid id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}