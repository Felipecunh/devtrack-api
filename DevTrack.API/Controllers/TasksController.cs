using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.API.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Create(CreateTaskDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var project = _context.Projects
            .FirstOrDefault(p => p.Id == dto.ProjectId && p.UserId == userId);

        if (project == null)
            return NotFound("Project not found or not yours");

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            ProjectId = project.Id
        };

        _context.Tasks.Add(task);
        _context.SaveChanges();

        return Created("", task);
    }

    [HttpGet("by-project/{projectId}")]
    public IActionResult GetByProject(Guid projectId)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var tasks = _context.Tasks
            .Where(t => t.ProjectId == projectId && t.Project.UserId == userId)
            .ToList();

        return Ok(tasks);
    }
}
