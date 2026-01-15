using DevTrack.API.Controllers.Base;
using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.API.Controllers;

[Authorize]
[Route("api/projects")]
public class ProjectsController : BaseController
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }

    //  GET: api/projects
    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _context.Projects
            .Where(p => p.UserId == UserId)
            .Include(p => p.Tasks)
            .ToListAsync();

        return Ok(projects);
    }

    //  POST: api/projects
    [HttpPost]
    public async Task<IActionResult> CreateProject(CreateProjectDto dto)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            UserId = UserId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, project);
    }

    // GET: api/projects/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId);

        if (project == null)
            return NotFound("Project not found or not yours");

        return Ok(project);
    }
}
