using Volo.Abp.Application.Dtos;

namespace UpdaterServer.File;

public class GetFilesRequestDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}