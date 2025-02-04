using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace UpdaterServer.Application;

public class ApplicationAppService(
    ApplicationManager applicationManager,
    IRepository<Application> applicationRepository
)
    : ApplicationService, IApplicationAppService
{
    public async Task<ApplicationDto> CreateAsync(CreateUpdateApplicationDto input)
    {
        var app = await applicationManager.CreateAsync(input.Name, input.Description);
        app = await applicationRepository.InsertAsync(app);
        return ObjectMapper.Map<Application, ApplicationDto>(app);
    }

    public async Task<ApplicationDto> UpdateAsync(Guid id, CreateUpdateApplicationDto input)
    {
        var app = await applicationRepository.GetAsync(a => a.Id == id);
        await applicationManager.UpdateAsync(app, input.Name, input.Description);
        await applicationRepository.UpdateAsync(app);
        return ObjectMapper.Map<Application, ApplicationDto>(app);
    }

    public async Task<ApplicationDto> DeleteAsync(Guid id)
    {
        var app = await applicationRepository.GetAsync(a => a.Id == id);
        await applicationRepository.DeleteAsync(app);
        return ObjectMapper.Map<Application, ApplicationDto>(app);
    }

    public async Task<ApplicationDto> GetAsync(Guid id)
    {
        var app = await applicationRepository.GetAsync(a => a.Id == id);
        return ObjectMapper.Map<Application, ApplicationDto>(app);
    }

    public async Task<PagedResultDto<ApplicationDto>> GetListAsync(GetApplicationListDto input)
    {
        var query = await applicationRepository.GetQueryableAsync();

        if (!input.Filter.IsNullOrWhiteSpace())
        {
            query = query.Where(a => a.Name.Contains(input.Filter) || a.Description.Contains(input.Filter));
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.Skip(input.SkipCount).Take(input.MaxResultCount);
        var apps = await AsyncExecuter.ToListAsync(query);
        return new PagedResultDto<ApplicationDto>(
            totalCount,
            ObjectMapper.Map<List<Application>, List<ApplicationDto>>(apps)
        );
    }
}