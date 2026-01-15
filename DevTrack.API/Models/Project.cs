namespace DevTrack.API.Models;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public List<TaskItem> Tasks { get; set; } = new();
}
