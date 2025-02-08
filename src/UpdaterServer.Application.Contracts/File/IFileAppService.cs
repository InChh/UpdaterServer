using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace UpdaterServer.File;

public interface IFileAppService : IApplicationService
{
    Task<FileMetadataDto> CreateAsync(CreateFileMetadataDto input);
    Task<FileMetadataDto> GetAsync(GetFileRequestDto input);
    Task<FileUrlDto> GetFileUrlAsync(GetFileRequestDto input);
}