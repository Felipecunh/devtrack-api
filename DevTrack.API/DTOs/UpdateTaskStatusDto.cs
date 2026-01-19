using System.ComponentModel.DataAnnotations;

namespace DevTrack.API.DTOs
{
    public class UpdateTaskStatusDto
    {
        [Required]
        public Models.TaskStatus Status { get; set; }
    }
}
