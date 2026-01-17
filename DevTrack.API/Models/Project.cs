namespace DevTrack.API.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!; // Relacionamento com o User

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<TaskItem> Tasks { get; set; } = new();
    }
}
