using System.ComponentModel.DataAnnotations;

namespace DevTrack.API.DTOs;

public class UpdateProjectDto
{
    [Required(ErrorMessage = "Project name is required")]
    [MaxLength(100, ErrorMessage = "Project name must be at most 100 characters")]
    public string Name { get; set; } = string.Empty;
}
