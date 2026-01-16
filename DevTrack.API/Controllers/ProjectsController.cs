using DevTrack.API.Controllers.Base;
using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Helpers;
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

    // GET: api/projects
    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _context.Projects
            .Where(p => p.UserId == UserId)
            .Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Tasks = p.Tasks.Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    ProjectId = t.ProjectId
                }).ToList()
            })
            .ToListAsync();

        return Ok(ApiResponse<List<ProjectResponseDto>>.Ok(
            projects,
            "Projects retrieved successfully"
        ));
    }

    // POST: api/projects
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            UserId = UserId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var response = new ProjectResponseDto
        {
            Id = project.Id,
            Name = project.Name,
            Tasks = new()
        };

        return CreatedAtAction(
            nameof(GetProjectById),
            new { id = project.Id },
            ApiResponse<ProjectResponseDto>.Ok(
                response,
                "Project created successfully"
            )
        );
    }

    // GET: api/projects/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        var project = await _context.Projects
            .Where(p => p.Id == id && p.UserId == UserId)
            .Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Tasks = p.Tasks.Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    ProjectId = t.ProjectId
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (project == null)
            return NotFound(ApiResponse<string>.Fail(
                "Project not found or not yours"
            ));

        return Ok(ApiResponse<ProjectResponseDto>.Ok(
            project,
            "Project retrieved successfully"
        ));
    }
}
