namespace UpdaterServer.Sts;

public class StsDto(string accessKeyId, string accessKeySecret, string securityToken, string expiration)
{
    public string AccessKeyId { get; set; } = accessKeyId;
    public string AccessKeySecret { get; set; } = accessKeySecret;
    public string SecurityToken { get; set; } = securityToken;
    public string Expiration { get; set; } = expiration;
}