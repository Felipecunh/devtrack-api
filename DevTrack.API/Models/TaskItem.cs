using DevTrack.API.Models;

namespace DevTrack.API.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;
        public TaskStatus Status { get; set; } = TaskStatus.Pendente;

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
