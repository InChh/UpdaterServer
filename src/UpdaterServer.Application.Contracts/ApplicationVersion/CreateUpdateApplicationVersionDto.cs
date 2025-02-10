using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UpdaterServer.ApplicationVersion;

public class CreateUpdateApplicationVersionDto
{
    [Required]
    public Guid ApplicationId { get; set; }
    
    [Required]
    [StringLength(ApplicationVersionConsts.MaxVersionNumberLength)]
    public string VersionNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(ApplicationVersionConsts.MaxDescriptionLength)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public List<Guid> FileMetadataIds { get; set; } = [];
    
}