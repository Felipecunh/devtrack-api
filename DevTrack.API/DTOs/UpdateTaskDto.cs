using System.ComponentModel.DataAnnotations;

namespace DevTrack.API.DTOs;

public class UpdateTaskDto
{
    [Required(ErrorMessage = "Task title is required")]
    [MaxLength(200, ErrorMessage = "Task title must be at most 200 characters")]
    public string Title { get; set; } = string.Empty;
}
