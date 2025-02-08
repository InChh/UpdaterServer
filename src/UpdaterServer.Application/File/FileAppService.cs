using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace UpdaterServer.File;

public class FileAppService(
    IRepository<FileMetadata> fileMetadataRepository,
    FileMetadataManager fileMetadataManager
)
    : ApplicationService, IFileAppService
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