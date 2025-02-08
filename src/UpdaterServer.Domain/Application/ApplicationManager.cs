using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace UpdaterServer.Application;

public class ApplicationManager(IGuidGenerator guidGenerator, IRepository<Application> applicationRepository)
    : DomainService
{
    public async Task<Application> CreateAsync(string name, string? description)
    {
        var c = await applicationRepository.CountAsync(i => i.Name == name);
        if (c != 0)
        {
            throw new BusinessException(ApplicationErrorCodes.ApplicationNameShouldBeUnique).WithData("name", name);
        }

        return new Application(guidGenerator.Create(), name, description);
    }

    public async Task UpdateAsync(Application application, string? name, string? description)
    {
        if (name is not null)
        {
            var c = await applicationRepository.CountAsync(i => i.Name == name && i.Id != application.Id);
            if (c != 0)
            {
                throw new BusinessException(ApplicationErrorCodes.ApplicationNameShouldBeUnique).WithData("name", name);
            }

            application.Name = name;
        }

        if (description is not null)
        {
            application.Description = description;
        }
    }
}