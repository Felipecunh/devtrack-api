namespace DevTrack.API.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!; // Relacionamento com o Project

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
