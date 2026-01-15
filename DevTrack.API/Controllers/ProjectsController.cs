using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Models;

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
        return Ok(_context.Projects.ToList());
    }

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

        return Created("", project);
    }
}
