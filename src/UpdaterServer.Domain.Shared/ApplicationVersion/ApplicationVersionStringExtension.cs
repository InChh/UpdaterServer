using Volo.Abp;

namespace UpdaterServer.ApplicationVersion;

public static class ApplicationVersionStringExtension
{
    public static void CheckVersionNumber(this string value)
    {
        // Check version number is valid, it should be in a specific format(xx.xx.xx).
        var strings = value.Split('.');
        if (strings.Length != 3)
        {
            throw new BusinessException(
                ApplicationVersionErrorCodes.VersionNumberInvalid).WithData("versionNumber", value);
        }

        // The numbers should be greater equal than 0.
        foreach (var numberStr in strings)
        {
            if (int.TryParse(numberStr, out var number) && number < 0)
            {
                throw new BusinessException(
                    ApplicationVersionErrorCodes.VersionNumberInvalid).WithData("versionNumber", value);
            }
        }
    }
}