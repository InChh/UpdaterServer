using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace UpdaterServer.File;

public class FileAppService(
    IRepository<FileMetadata> fileMetadataRepository,
    FileMetadataManager fileMetadataManager
)
    : UpdaterServerAppService, IFileAppService
{
    public async Task<FileMetadataDto> CreateAsync(CreateFileMetadataDto input)
    {
        var fileMetadata = await fileMetadataManager.CreateAsync(
            input.Path,
            input.Hash,
            input.Size,
            input.Url
        );

        await fileMetadataRepository.InsertAsync(fileMetadata);

        return ObjectMapper.Map<FileMetadata, FileMetadataDto>(fileMetadata);
    }

    public async Task<FileMetadataDto> GetAsync(GetFileRequestDto input)
    {
        var query = await fileMetadataRepository.GetQueryableAsync();
        query = query.Where(f => f.Hash == input.Hash);
        query = query.WhereIf(input.Size.HasValue, f => f.Size == input.Size);
        var fileMetadata = await AsyncExecuter.FirstOrDefaultAsync(query);
        if (fileMetadata is null)
        {
            throw new EntityNotFoundException(typeof(FileMetadata));
        }

        return ObjectMapper.Map<FileMetadata, FileMetadataDto>(fileMetadata);
    }

    public async Task<PagedResultDto<FileMetadataDto>> GetListAsync(GetFilesRequestDto input)
    {
        var query = await fileMetadataRepository.GetQueryableAsync();
        query = query
            .WhereIf(input.Filter != null, f => f.Path.Contains(input.Filter!));

        var totalCount = await AsyncExecuter.CountAsync(query);
        query = query.OrderBy(input.Sorting ?? nameof(FileMetadata.Path));
        query = query.PageBy(input.SkipCount, input.MaxResultCount);

        var fileMetadatas = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<FileMetadataDto>(
            totalCount,
            ObjectMapper.Map<List<FileMetadata>, List<FileMetadataDto>>(fileMetadatas)
        );
    }

    public async Task<FileUrlDto> GetFileUrlAsync(GetFileRequestDto input)
    {
        var query = await fileMetadataRepository.GetQueryableAsync();
        query = query.Where(f => f.Hash == input.Hash);
        query = query.WhereIf(input.Size.HasValue, f => f.Size == input.Size);
        var fileMetadata = await AsyncExecuter.FirstOrDefaultAsync(query);
        if (fileMetadata is null)
        {
            throw new EntityNotFoundException(typeof(FileMetadata));
        }

        return new FileUrlDto
        {
            Hash = fileMetadata.Hash,
            Url = fileMetadata.Url
        };
    }
}