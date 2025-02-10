using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace UpdaterServer.ApplicationVersion;

public interface IApplicationVersionAppService : IApplicationService
{
    Task<ApplicationVersionDto> CreateAsync(CreateUpdateApplicationVersionDto input);

    Task<ApplicationVersionDto> UpdateAsync(Guid id, CreateUpdateApplicationVersionDto input);

    Task<ApplicationVersionDto> GetLatestAsync(Guid applicationId);

    Task<ApplicationVersionDto> GetAsync(Guid id);

    Task<PagedResultDto<ApplicationVersionDto>> GetListAsync(Guid applicationId,
        PagedAndSortedResultRequestDto input);

    Task<ApplicationVersionFilesDto> GetFilesByVersionNumberAsync(Guid applicationId, string versionNumber);

    Task<ApplicationVersionFilesDto> GetFilesByIdAsync(Guid id);

    Task<ApplicationVersionDto> DeleteAsync(Guid id);

    Task ActiveAsync(Guid id);

    Task DeactiveAsync(Guid id);
}