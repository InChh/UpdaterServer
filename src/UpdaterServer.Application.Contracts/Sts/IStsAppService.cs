using System.Threading.Tasks;
using UpdaterServer.File;
using Volo.Abp.Application.Services;

namespace UpdaterServer.Sts;

public interface IStsAppService: IApplicationService
{
    Task<StsDto> GetUploadAsync();
    Task<StsDto> GetDownloadAsync();
}