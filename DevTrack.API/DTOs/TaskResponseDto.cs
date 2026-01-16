namespace DevTrack.API.DTOs;

public class TaskResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public Guid ProjectId { get; set; }
}