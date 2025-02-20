using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace UpdaterServer.File;

public interface IFileAppService : IApplicationService
{
    Task<FileMetadataDto> CreateAsync(CreateFileMetadataDto input);
    Task<FileMetadataDto> GetByHashAsync(GetFileRequestDto input);
    Task<FileMetadataDto> GetByIdAsync(Guid id);
    Task<List<FileMetadataDto>> GetByVersionIdAsync(Guid versionId);
    Task<PagedResultDto<FileMetadataDto>> GetListAsync(GetFilesRequestDto input);
    Task<FileUrlDto> GetFileUrlAsync(GetFileRequestDto input);
}