using AutoMapper;
using UpdaterServer.Application;
using UpdaterServer.ApplicationVersion;
using UpdaterServer.File;

namespace UpdaterServer;

public class UpdaterServerApplicationAutoMapperProfile : Profile
{
    public UpdaterServerApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<Application.Application, ApplicationDto>();
        CreateMap<FileMetadata, FileMetadataDto>();
        CreateMap<ApplicationVersion.ApplicationVersion, ApplicationVersionDto>();
    }
}