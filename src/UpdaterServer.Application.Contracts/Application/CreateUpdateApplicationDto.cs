using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace UpdaterServer.Application;

public class CreateUpdateApplicationDto 
{
    [Required]
    [StringLength(ApplicationConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(ApplicationConsts.MaxDescriptionLength)]
    public string? Description { get; set; }
}