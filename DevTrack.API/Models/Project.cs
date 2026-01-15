namespace DevTrack.API.Models;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // 1 Project -> N Tasks
    public List<TaskItem> Tasks { get; set; } = new();
}
