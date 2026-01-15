using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Models;

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

    [HttpGet("by-project/{projectId}")]
    public IActionResult GetByProject(Guid projectId)
    {
        var tasks = _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .ToList();

        return Ok(tasks);
    }
}
