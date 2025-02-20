using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace UpdaterServer.File;

public interface IFileMetadataRepository : IRepository<FileMetadata>
{
    public Task<FileMetadata> GetByIdAsync(Guid id);
    public Task<FileMetadata> GetByHashAsync(string hash, long? size);
    public Task<List<FileMetadata>> GetByVersionIdAsync(Guid versionId);
}