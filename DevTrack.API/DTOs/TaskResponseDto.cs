using DevTrack.API.Models;

namespace DevTrack.API.DTOs
{
    public class TaskResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }

        public Models.TaskStatus Status { get; set; }
    }
}
