using DevTrack.API.Controllers.Base;
using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.API.Controllers;

[Authorize]
[Route("api/tasks")]
public class TasksController : BaseController
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/tasks
    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.UserId == UserId);

        if (project == null)
            return NotFound("Project not found or not yours");

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            ProjectId = project.Id
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var response = new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            ProjectId = task.ProjectId
        };

        return Created("", response);
    }

    // GET: api/tasks/by-project/{projectId}
    [HttpGet("by-project/{projectId}")]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        var tasks = await _context.Tasks
            .Where(t => t.ProjectId == projectId && t.Project.UserId == UserId)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                ProjectId = t.ProjectId
            })
            .ToListAsync();

        return Ok(tasks);
    }
}
