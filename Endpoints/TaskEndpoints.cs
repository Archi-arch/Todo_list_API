using My_todo_API.Models;
using My_todo_API.dto;
using My_todo_API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace My_todo_API.Endpoints;


public static class Endpoints
{


    public static void MapTaskEndpoints(this WebApplication app)
    {




        app.MapPost("/task", (CreateTaskDto dto, AppDbContext db) =>
        {
            var selectedCategories = db.Categories
                    .Where(c => dto.CategoryIds.Contains(c.CategoryId))
                    .ToList();


            if (selectedCategories.Count != dto.CategoryIds.Count)
            {
                return Results.BadRequest("Деякі з вказаних категорій не існують!");
            }

            var deadlineError = ValidateDeadline(dto.Deadline);
            if (deadlineError != null) return Results.BadRequest(deadlineError);

            var newTask = new TodoTask
            {

                Title = dto.Title,
                IsComplete = false,
                Categories = selectedCategories,
                Deadline = dto.Deadline
            };

            db.Tasks.Add(newTask);
            db.SaveChanges();

            var taskDto = new GetTaskDto(
                newTask.Id,
                newTask.Title,
                newTask.IsComplete,
                newTask.Categories.Select(c => c.CategoryName).ToList(),
                newTask.Deadline
            );

            return Results.Created($"/task/{newTask.Id}", taskDto);


        });



        app.MapPut("/task/{id}", (int id, UpdateTaskDto dto, AppDbContext db) =>
        {
            var existingTask = db.Tasks
                .Include(t => t.Categories)
                .FirstOrDefault(t => t.Id == id && !t.IsDeleted);


            if (existingTask == null)
            {
                return Results.NotFound($"Task with id {id} not found");
            }
            var selectedCategories = db.Categories
                            .Where(c => dto.CategoryIds.Contains(c.CategoryId))
                            .ToList();

            if (selectedCategories.Count != dto.CategoryIds.Count)
            {
                return Results.BadRequest("Деякі з вказаних категорій не існують!");
            }

            var deadlineError = ValidateDeadline(dto.Deadline);
            if (deadlineError != null) return Results.BadRequest(deadlineError);

            existingTask.Title = dto.Title;
            existingTask.IsComplete = dto.IsComplete;
            existingTask.Categories.Clear();
            existingTask.Categories.AddRange(selectedCategories);
            existingTask.Deadline = dto.Deadline;

            db.SaveChanges();

            return Results.NoContent();

        });


        app.MapGet("/task", (AppDbContext db, string? search = null, int page = 1, int pageSize = 15) =>
        {


            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 15;
            if (pageSize > 100) pageSize = 100;

            int skipCount = (page - 1) * pageSize;

            var query = db.Tasks.Include(t => t.Categories).Where(t => !t.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.ToLower().Trim();
                query = query.Where(t => t.Title.ToLower().Contains(cleanSearch));
            }

            var response = query
            .Include(t => t.Categories)
            .OrderByDescending(t => t.Id)
            .Skip(skipCount)
            .Take(pageSize)
            .Select(task => new GetTaskDto(
                task.Id,
                task.Title,
                task.IsComplete,
                (!task.IsComplete && task.Deadline != null && DateTime.UtcNow > task.Deadline)
                        ? new List<string> { "Протерміновано" }
                        : (task.Categories.Any()
                        ? task.Categories.Select(c => c.CategoryName).ToList()
                        : new List<string> { "Без категорії" }),
                task.Deadline

            )).ToList();


            return Results.Ok(response);
        });




        app.MapDelete("/task/{id}", (int id, AppDbContext db) =>
        {
            var existingTask = db.Tasks.FirstOrDefault(e => e.Id == id && !e.IsDeleted);

            if (existingTask == null)
            {
                return Results.NotFound($"Task with id {id} not found");
            }


            existingTask.IsDeleted = true;
            db.SaveChanges();
            return Results.Ok($"Task with id - {id} deleted successfully");

        });



        app.MapGet("/task/category/{categoryId}", (int categoryId, AppDbContext db, int page = 1, int pageSize = 15) =>
        {

            var categoryExists = db.Categories.Any(c => c.CategoryId == categoryId);
            if (!categoryExists)
            {
                return Results.NotFound($"Категорії з ID {categoryId} не існує");
            }

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 15;

            int skipCount = (page - 1) * pageSize;


            var response = db.Tasks
                .Include(t => t.Categories)
                .Where(task => task.Categories.Any(c => c.CategoryId == categoryId) && !task.IsDeleted)
                .OrderByDescending(t => t.Id)
                .Skip(skipCount)
                .Take(pageSize)
                .Select(task => new GetTaskDto(
                    task.Id,
                    task.Title,
                    task.IsComplete,
                    task.Categories.Select(c => c.CategoryName).ToList(),
                    task.Deadline

                )).ToList();

            return Results.Ok(response);
        });


        app.MapGet("/tasks/overdue", (AppDbContext db, int page = 1, int pageSize = 15) =>
        {

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 15;

            int skipCount = (page - 1) * pageSize;


            var response = db.Tasks
                .Include(t => t.Categories)
                .Where(task => !task.IsComplete && task.Deadline != null && DateTime.UtcNow > task.Deadline && !task.IsDeleted)
                .OrderByDescending(t => t.Id)
                .Skip(skipCount)
                .Take(pageSize)
                .Select(task => new GetTaskDto(
                    task.Id,
                    task.Title,
                    task.IsComplete,
                    new List<string> { "Протерміновано" },
                    task.Deadline
                )).ToList();

            return Results.Ok(response);
        });

    }



    private static string? ValidateDeadline(DateTime? deadline)
    {
        if (deadline == null) return null;

        if (deadline < DateTime.UtcNow)
        {
            return "Dedline cant be past";
        }
        if (deadline > DateTime.UtcNow.AddDays(365))
        {
            return "Deadline cant be future more than 365 days";
        }

        return null;
    }
}

