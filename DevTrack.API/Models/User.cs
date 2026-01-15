namespace DevTrack.API.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Relacionamento futuro
    public List<Project> Projects { get; set; } = new();
}
