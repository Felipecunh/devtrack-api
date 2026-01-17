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
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.UserId == UserId);

        if (project == null)
            return NotFound(ApiResponse<string>.Fail("Project not found or not yours"));

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            ProjectId = project.Id
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<TaskResponseDto>.Ok(
            new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                ProjectId = task.ProjectId
            },
            "Task created successfully"
        ));
    }

    // GET: api/tasks/by-project/{projectId}?page=1&pageSize=10&search=test&orderBy=title
    [HttpGet("by-project/{projectId:guid}")]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null
    )
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var baseQuery = _context.Tasks
            .Include(t => t.Project)
            .Where(t =>
                t.ProjectId == projectId &&
                t.Project.UserId == UserId
            );

        if (!string.IsNullOrWhiteSpace(search))
        {
            baseQuery = baseQuery.Where(t =>
                t.Title.ToLower().Contains(search.ToLower())
            );
        }

        baseQuery = orderBy?.ToLower() switch
        {
            "title" => baseQuery.OrderBy(t => t.Title),
            _ => baseQuery.OrderBy(t => t.Title)
        };

        var totalItems = await baseQuery.CountAsync();

        var tasks = await baseQuery
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
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };

        return Ok(ApiResponse<PagedResultDto<TaskResponseDto>>.Ok(
            result,
            "Tasks retrieved successfully"
        ));
    }

    // PUT: api/tasks/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskDto dto)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == UserId);

        if (task == null)
            return NotFound(ApiResponse<string>.Fail("Task not found or not yours"));

        task.Title = dto.Title;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok(
            "Task updated successfully",
            "Success"
        ));
    }

    // DELETE: api/tasks/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == UserId);

        if (task == null)
            return NotFound(ApiResponse<string>.Fail("Task not found or not yours"));

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok(
            "Task deleted successfully",
            "Success"
        ));
    }
}
