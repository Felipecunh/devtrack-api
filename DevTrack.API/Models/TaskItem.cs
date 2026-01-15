namespace DevTrack.API.Models;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;

    // FK
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
