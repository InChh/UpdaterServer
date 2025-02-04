using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace UpdaterServer.Application;

public interface IApplicationAppService : IApplicationService
{
    Task<ApplicationDto> CreateAsync(CreateUpdateApplicationDto input);

    Task<ApplicationDto> UpdateAsync(Guid id, CreateUpdateApplicationDto input);

    Task<ApplicationDto> DeleteAsync(Guid id);

    Task<ApplicationDto> GetAsync(Guid id);

    Task<PagedResultDto<ApplicationDto>> GetListAsync(GetApplicationListDto input);
}