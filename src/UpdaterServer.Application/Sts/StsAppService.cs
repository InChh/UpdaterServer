using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace UpdaterServer.Sts;

public class StsAppService(AlibabaCloud.SDK.Sts20150401.Client stsClient) : ApplicationService, IStsAppService
{
    [Authorize]
    public async Task<StsDto> GetUploadAsync()
    {
        var request = new AlibabaCloud.SDK.Sts20150401.Models.AssumeRoleRequest()
        {
            RoleArn = Environment.GetEnvironmentVariable(StsEnvironmentNameConsts.UploadRamRoleArn),
            RoleSessionName = "UpdaterAdminUploadSession"
        };
        var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
        var response = await stsClient.AssumeRoleWithOptionsAsync(request, runtime);
        if (response.StatusCode != 200)
        {
            throw new BusinessException(null, "Failed to get STS credentials");
        }

        var credentials = response.Body.Credentials;
        return new StsDto(
            credentials.AccessKeyId,
            credentials.AccessKeySecret,
            credentials.SecurityToken,
            credentials.Expiration
        );
    }

    public async Task<StsDto> GetDownloadAsync()
    {
        var request = new AlibabaCloud.SDK.Sts20150401.Models.AssumeRoleRequest()
        {
            RoleArn = Environment.GetEnvironmentVariable(StsEnvironmentNameConsts.DownloadRamRoleArn),
            RoleSessionName = "UpdaterAdminDownloadSession"
        };
        var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
        var response = await stsClient.AssumeRoleWithOptionsAsync(request, runtime);
        if (response.StatusCode != 200)
        {
            throw new BusinessException(null, "Failed to get STS credentials");
        }

        var credentials = response.Body.Credentials;
        return new StsDto(
            credentials.AccessKeyId,
            credentials.AccessKeySecret,
            credentials.SecurityToken,
            credentials.Expiration
        );
    }
}