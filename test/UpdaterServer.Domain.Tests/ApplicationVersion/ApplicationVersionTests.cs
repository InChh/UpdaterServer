using System;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Modularity;
using Xunit;

namespace UpdaterServer.ApplicationVersion;

public class ApplicationVersionTests
{
    [Fact]
    public void Should_Set_VersionNumber()
    {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var applicationVersion =
            new ApplicationVersion(Guid.NewGuid(), Guid.NewGuid(), $"{year}.{month}.1", "Test version 1");
        applicationVersion.VersionNumber.ShouldBe($"{year}.{month}.1");
    }

    [Fact]
    public void Should_Throw_Exception_When_VersionNumber_Invalid()
    {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;


        Should.Throw<BusinessException>(() =>
        {
            _ = new ApplicationVersion(Guid.NewGuid(), Guid.NewGuid(), "2022.1.1", "Test version 1");
        });

        Should.Throw<BusinessException>(() =>
        {
            _ = new ApplicationVersion(Guid.NewGuid(), Guid.NewGuid(), $"{year}.{month}.0", "Test version 1");
        });
        
        Should.Throw<BusinessException>(() =>
        {
            _ = new ApplicationVersion(Guid.NewGuid(), Guid.NewGuid(), $"{year}.{month}", "Test version 1");
        });
        
        Should.Throw<BusinessException>(() =>
        {
            _ = new ApplicationVersion(Guid.NewGuid(), Guid.NewGuid(), $"{year}.{month}.1.1", "Test version 1");
        });
        
        Should.Throw<BusinessException>(() =>
        {
            _ = new ApplicationVersion(Guid.NewGuid(), Guid.NewGuid(), $"1234", "Test version 1");
        });
    }
}