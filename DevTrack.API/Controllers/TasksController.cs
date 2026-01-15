using Microsoft.AspNetCore.Mvc;
using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Models;

namespace DevTrack.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    // POST /api/tasks
    [HttpPost]
    public IActionResult Create(CreateTaskDto dto)
    {
        var projectExists = _context.Projects.Any(p => p.Id == dto.ProjectId);
        if (!projectExists)
            return NotFound("Project not found");

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            ProjectId = dto.ProjectId
        };

        _context.Tasks.Add(task);
        _context.SaveChanges();

        return Created("", task);
    }

    // GET /api/tasks/by-project/{projectId}
    [HttpGet("by-project/{projectId}")]
    public IActionResult GetByProject(Guid projectId)
    {
        var tasks = _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .ToList();

        return Ok(tasks);
    }
}
