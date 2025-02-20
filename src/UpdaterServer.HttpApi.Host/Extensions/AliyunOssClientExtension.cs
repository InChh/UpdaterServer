// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class AliyunOssClientExtension
{
    public static IServiceCollection AddAliyunOssClient(this IServiceCollection services,
        AlibabaCloud.OpenApiClient.Models.Config config)
    {
        var client = new AlibabaCloud.SDK.Sts20150401.Client(config);
        return services.AddSingleton(client);
    }
}