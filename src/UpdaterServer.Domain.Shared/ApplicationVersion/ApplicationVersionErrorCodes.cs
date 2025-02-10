namespace UpdaterServer.ApplicationVersion;

public class ApplicationVersionErrorCodes
{
    public const string ApplicationDoesNotExist = "App:2001";
    public const string VersionNumberAlreadyExist = "App:2002";
    public const string VersionNumberInvalid = "App:2003";
    public const string VersionNumberShouldBeNewer = "App:2004";
    public const string FileDoesNotExist = "App:2005";
    public const string VersionDoesNotExist = "App:2006";
}