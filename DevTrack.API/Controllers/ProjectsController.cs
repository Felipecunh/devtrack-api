using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Models;

namespace DevTrack.API.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/projects
    [HttpGet]
    public IActionResult GetAll()
    {
        var projects = _context.Projects
            .Include(p => p.Tasks)
            .ToList();

        return Ok(projects);
    }

    // GET /api/projects/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var project = _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefault(p => p.Id == id);

        if (project == null)
            return NotFound();

        return Ok(project);
    }

    // POST /api/projects
    [HttpPost]
    public IActionResult Create(CreateProjectDto dto)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name
        };

        _context.Projects.Add(project);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }
}
