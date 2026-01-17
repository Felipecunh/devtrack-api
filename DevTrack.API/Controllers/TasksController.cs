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
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.UserId == UserId);

        if (project == null)
        {
            return NotFound(
                ApiResponse<string>.Fail("Project not found or not yours")
            );
        }

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

        return CreatedAtAction(
            nameof(GetByProject),
            new { projectId = task.ProjectId },
            ApiResponse<TaskResponseDto>.Ok(response, "Task created successfully")
        );
    }

    // PUT: api/tasks/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == UserId);

        if (task == null)
        {
            return NotFound(
                ApiResponse<string>.Fail("Task not found or not yours")
            );
        }

        task.Title = dto.Title;
        await _context.SaveChangesAsync();

        var response = new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            ProjectId = task.ProjectId
        };

        return Ok(
            ApiResponse<TaskResponseDto>.Ok(response, "Task updated successfully")
        );
    }

    // DELETE: api/tasks/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == UserId);

        if (task == null)
        {
            return NotFound(
                ApiResponse<string>.Fail("Task not found or not yours")
            );
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return Ok(
            ApiResponse<string>.Ok(
                "Task deleted successfully",
                "Task deleted successfully"
            )
        );
    }

    // GET: api/tasks/by-project/{projectId}?page=1&pageSize=10
    [HttpGet("by-project/{projectId:guid}")]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var baseQuery = _context.Tasks
            .Where(t => t.ProjectId == projectId && t.Project.UserId == UserId);

        var totalItems = await baseQuery.CountAsync();

        var tasks = await baseQuery
            .OrderBy(t => t.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                ProjectId = t.ProjectId
            })
            .ToListAsync();

        var result = new PagedResultDto<TaskResponseDto>
        {
            Items = tasks,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return Ok(
            ApiResponse<PagedResultDto<TaskResponseDto>>.Ok(
                result,
                "Tasks retrieved successfully"
            )
        );
    }
}
