namespace DevTrack.API.DTOs;

public class ProjectResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<TaskResponseDto> Tasks { get; set; } = new();
}
