using System.Collections.Generic;
using UpdaterServer.File;

namespace UpdaterServer.ApplicationVersion;

public class ApplicationVersionFilesDto
{
    public List<FileMetadataDto> Files { get; set; } = [];
}