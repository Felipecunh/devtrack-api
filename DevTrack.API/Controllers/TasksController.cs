using DevTrack.API.Controllers.Base;
using DevTrack.API.Data;
using DevTrack.API.DTOs;
using DevTrack.API.Helpers;
using DevTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Alias para evitar conflito com System.Threading.Tasks.TaskStatus
using TaskItemStatus = DevTrack.API.Models.TaskStatus;

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

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        // Verifica se o projeto existe e se pertence ao usuário autenticado
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.UserId == UserId);

        if (project == null)
            return NotFound(ApiResponse<string>.Fail("Project not found or not yours"));

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            ProjectId = project.Id,
            Status = TaskItemStatus.Pendente,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<TaskResponseDto>.Ok(
            new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                ProjectId = task.ProjectId,
                Status = task.Status
            },
            "Task created successfully"
        ));
    }

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

        // Base da consulta já restringindo pelo usuário dono do projeto
        var baseQuery = _context.Tasks
            .Include(t => t.Project)
            .Where(t =>
                t.ProjectId == projectId &&
                t.Project.UserId == UserId
            );

        // Filtro simples por título
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseQuery = baseQuery.Where(t =>
                t.Title.ToLower().Contains(search.ToLower())
            );
        }

        // Ordenação básica (facilmente extensível)
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
                ProjectId = t.ProjectId,
                Status = t.Status
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskDto dto)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == UserId);

        if (task == null)
            return NotFound(ApiResponse<string>.Fail("Task not found or not yours"));

        // Atualiza apenas o título
        task.Title = dto.Title;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok(
            "Task updated successfully",
            "Success"
        ));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateTaskStatusDto dto)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == UserId);

        if (task == null)
            return NotFound(ApiResponse<string>.Fail("Task not found or not yours"));

        // Atualização isolada do status para evitar efeitos colaterais
        task.Status = dto.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok(
            "Task status updated successfully",
            "Success"
        ));
    }

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
