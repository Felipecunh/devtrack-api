using System.ComponentModel.DataAnnotations;

namespace DevTrack.API.DTOs;

public class CreateTaskDto
{
    [Required(ErrorMessage = "Task title is required")]
    [MaxLength(150, ErrorMessage = "Task title must be at most 150 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "ProjectId is required")]
    public Guid ProjectId { get; set; }
}
