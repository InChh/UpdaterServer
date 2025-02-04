using Volo.Abp.Application.Dtos;

namespace UpdaterServer.Application;

public class GetApplicationListDto: PagedAndSortedResultRequestDto
{
    // Used to search application by name or description
    public string? Filter { get; set; }
    
}