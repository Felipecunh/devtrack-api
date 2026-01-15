using DevTrack.API.Controllers.Base;
using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.API.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : BaseController
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Create(CreateTaskDto dto)
    {
        var project = _context.Projects
            .FirstOrDefault(p => p.Id == dto.ProjectId && p.UserId == UserId);

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
        var tasks = _context.Tasks
            .Include(t => t.Project)
            .Where(t => t.ProjectId == projectId && t.Project.UserId == UserId)
            .ToList();

        return Ok(tasks);
    }
}
