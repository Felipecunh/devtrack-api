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
            return NotFound(ApiResponse<string>.Fail("Project not found or not yours"));

        return Ok(ApiResponse<ProjectResponseDto>.Ok(
            project,
            "Project retrieved successfully"
        ));
    }

    // POST: api/projects
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

    // PUT: api/projects/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto dto)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId);

        if (project == null)
            return NotFound(ApiResponse<string>.Fail("Project not found or not yours"));

        project.Name = dto.Name;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok(
            "Project updated successfully",
            "Success"
        ));
    }

    // DELETE: api/projects/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId);

        if (project == null)
            return NotFound(ApiResponse<string>.Fail("Project not found or not yours"));

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok(
            "Project deleted successfully",
            "Success"
        ));
    }
}
