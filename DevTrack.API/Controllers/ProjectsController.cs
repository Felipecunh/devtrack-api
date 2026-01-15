using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.API.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var projects = _context.Projects
            .Where(p => p.UserId == userId)
            .ToList();

        return Ok(projects);
    }

    [HttpPost]
    public IActionResult Create(CreateProjectDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            UserId = userId
        };

        _context.Projects.Add(project);
        _context.SaveChanges();

        return Created("", project);
    }
}
