using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Linq;

namespace UpdaterServer.File;

public class FileMetadataManager(
    IRepository<FileMetadata> fileMetadataRepository,
    IGuidGenerator guidGenerator,
    IAsyncQueryableExecuter asyncExecutor
): DomainService
{
    public async Task<FileMetadata> CreateAsync(string path, string hash, long size, string url)
    {
        var c = await fileMetadataRepository.CountAsync(f => f.Hash == hash && f.Size == size && f.Path == path);
        if (c > 0)
        {
            throw new BusinessException(FileMetadataErrorCodes.FileAlreadyExists)
                .WithData("path", path)
                .WithData("hash", hash);
        }

        var query = await fileMetadataRepository.GetQueryableAsync();
        var q = query.Where(f => f.Hash == hash);
        var fileMetadata = await asyncExecutor.FirstOrDefaultAsync(q);
        if (fileMetadata is null)
        {
            return new FileMetadata(
                guidGenerator.Create(),
                path,
                hash,
                size,
                url
            );
        }

        // if file hash is the same, we need to ensure that the url is the same too so that 
        // in oss storage, we don't have multiple files with the same hash
        if (fileMetadata.Url != url)
        {
            throw new BusinessException(FileMetadataErrorCodes.SameHashDifferentUrl)
                .WithData("path", path)
                .WithData("hash", hash)
                .WithData("requestedUrl", url)
                .WithData("existingUrl", fileMetadata.Url);
        }

        return new FileMetadata(
            guidGenerator.Create(),
            path,
            hash,
            size,
            url
        );
    }
}