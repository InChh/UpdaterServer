using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpdaterServer.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace UpdaterServer.File;

public class FileMetadataRepository(IDbContextProvider<UpdaterServerDbContext> dbContextProvider)
    : EfCoreRepository<UpdaterServerDbContext, FileMetadata>(dbContextProvider), IFileMetadataRepository
{
    public async Task<FileMetadata> GetByIdAsync(Guid id)
    {
        return await GetAsync(f => f.Id == id);
    }

    public async Task<FileMetadata> GetByHashAsync(string hash, long? size)
    {
        return await GetAsync(f => f.Hash == hash && f.Size == size);
    }

    public async Task<List<FileMetadata>> GetByVersionIdAsync(Guid versionId)
    {
        var context = await GetDbContextAsync();
        // Join the FileMetadata and ApplicationVersion table using the VersionFiles table.
        var query = from file in context.Set<FileMetadata>()
            join versionFile in context.Set<VersionFile>() on file.Id equals versionFile.FileMetadataId
            where versionFile.VersionId == versionId
            select file;
        return await AsyncExecuter.ToListAsync(query);
    }
}