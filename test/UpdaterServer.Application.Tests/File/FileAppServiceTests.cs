using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Xunit;

namespace UpdaterServer.File;

public class FileAppServiceTests<TStartupModule> : UpdaterServerApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IFileAppService _fileAppService;

    public FileAppServiceTests()
    {
        _fileAppService = GetRequiredService<IFileAppService>();
    }

    [Fact]
    public async Task CreateAsync_Should_Work()
    {
        var input = new CreateFileMetadataDto
        {
            Path = "test.txt",
            Hash = "test.txt-hash",
            Size = 1024,
            Url = "https://test.com/test.txt-hash"
        };

        var fileMetadata = await _fileAppService.CreateAsync(input);

        fileMetadata.ShouldNotBeNull();
        fileMetadata.Path.ShouldBe(input.Path);
        fileMetadata.Hash.ShouldBe(input.Hash);
        fileMetadata.Size.ShouldBe(input.Size);
        fileMetadata.Url.ShouldBe(input.Url);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_BusinessException_When_File_Already_Exists()
    {
        var input = new CreateFileMetadataDto
        {
            Path = "test.txt",
            Hash = "test.txt-hash",
            Size = 1024,
            Url = "https://test.com/test.txt-hash"
        };

        await _fileAppService.CreateAsync(input);

        var exception = await _fileAppService.CreateAsync(input).ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(FileMetadataErrorCodes.FileAlreadyExists);
        exception.Data["path"].ShouldBe(input.Path);
        exception.Data["hash"].ShouldBe(input.Hash);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_BusinessException_When_Same_Hash_Different_Url()
    {
        var input = new CreateFileMetadataDto
        {
            Path = "test.txt",
            Hash = "test.txt-hash",
            Size = 1024,
            Url = "https://test.com/test.txt-hash"
        };

        await _fileAppService.CreateAsync(input);

        var exception = await _fileAppService.CreateAsync(new CreateFileMetadataDto
        {
            Path = "foo/test.txt",
            Hash = "test.txt-hash",
            Size = 1024,
            Url = "https://test.com/test.txt-hash-different"
        }).ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(FileMetadataErrorCodes.SameHashDifferentUrl);
        exception.Data["path"].ShouldBe("foo/test.txt");
        exception.Data["hash"].ShouldBe(input.Hash);
        exception.Data["requestedUrl"].ShouldBe("https://test.com/test.txt-hash-different");
        exception.Data["existingUrl"].ShouldBe("https://test.com/test.txt-hash");
    }

    [Fact]
    public async Task GetAsync_Should_Work()
    {
        var input = new CreateFileMetadataDto
        {
            Path = "test.txt",
            Hash = "test.txt-hash",
            Size = 1024,
            Url = "https://test.com/test.txt-hash"
        };

        var fileMetadata = await _fileAppService.CreateAsync(input);

        var fileMetadataFromDb = await _fileAppService.GetAsync(new GetFileRequestDto
        {
            Hash = input.Hash,
            Size = input.Size
        });

        fileMetadataFromDb.ShouldNotBeNull();
        fileMetadataFromDb.Path.ShouldBe(input.Path);
        fileMetadataFromDb.Hash.ShouldBe(input.Hash);
        fileMetadataFromDb.Size.ShouldBe(input.Size);
        fileMetadataFromDb.Url.ShouldBe(input.Url);

        fileMetadataFromDb = await _fileAppService.GetAsync(new GetFileRequestDto
        {
            Hash = input.Hash
        });

        fileMetadataFromDb.ShouldNotBeNull();
        fileMetadataFromDb.Path.ShouldBe(input.Path);
        fileMetadataFromDb.Hash.ShouldBe(input.Hash);
        fileMetadataFromDb.Size.ShouldBe(input.Size);
        fileMetadataFromDb.Url.ShouldBe(input.Url);

        await _fileAppService.GetAsync(new GetFileRequestDto
            {
                Hash = "foo"
            })
            .ShouldThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task GetListAsync_Should_Work()
    {
        var input = new GetFilesRequestDto()
        {
            MaxResultCount = UpdaterServerTestConsts.TestFileMetadataCount
        };

        var fileMetadata = await _fileAppService.GetListAsync(input);

        fileMetadata.ShouldNotBeNull();
        fileMetadata.TotalCount.ShouldBe(UpdaterServerTestConsts.TestFileMetadataCount);
        fileMetadata.Items.Count.ShouldBe(UpdaterServerTestConsts.TestFileMetadataCount);
    }

    [Fact]
    public async Task GetFileUrlAsync_Should_Work()
    {
        var input = new CreateFileMetadataDto
        {
            Path = "test.txt",
            Hash = "test.txt-hash",
            Size = 1024,
            Url = "https://test.com/test.txt-hash"
        };

        await _fileAppService.CreateAsync(input);

        var fileUrl = await _fileAppService.GetFileUrlAsync(new GetFileRequestDto
        {
            Hash = input.Hash,
            Size = input.Size
        });

        fileUrl.ShouldNotBeNull();
        fileUrl.Hash.ShouldBe(input.Hash);
        fileUrl.Url.ShouldBe(input.Url);

        fileUrl = await _fileAppService.GetFileUrlAsync(new GetFileRequestDto
        {
            Hash = input.Hash
        });

        fileUrl.ShouldNotBeNull();
        fileUrl.Hash.ShouldBe(input.Hash);
        fileUrl.Url.ShouldBe(input.Url);

        await _fileAppService.GetFileUrlAsync(new GetFileRequestDto
            {
                Hash = "foo"
            })
            .ShouldThrowAsync<EntityNotFoundException>();
    }
}