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

            if (dto.Deadline != null)
            {
                if (dto.Deadline < DateTime.UtcNow)
                {
                    return Results.BadRequest("Dedline cant be past");
                }
                if (dto.Deadline > DateTime.UtcNow.AddDays(365))
                {
                    return Results.BadRequest("Deadline cant be future more than 365 days");
                }
            }

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
            var existingTask = db.Tasks.FirstOrDefault(t => t.Id == id);


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

            if (dto.Deadline != null)
            {
                if (dto.Deadline < DateTime.UtcNow)
                {
                    return Results.BadRequest("Dedline cant be past");
                }
                if (dto.Deadline > DateTime.UtcNow.AddDays(365))
                {
                    return Results.BadRequest("Deadline cant be future more than 365 days");
                }
            }

            existingTask.Title = dto.Title;
            existingTask.IsComplete = dto.IsComplete;
            existingTask.Categories.Clear();
            existingTask.Categories.AddRange(selectedCategories);
            existingTask.Deadline = dto.Deadline;

            db.SaveChanges();

            return Results.NoContent();

        });


        app.MapGet("/task", (AppDbContext db) =>
        {
            var response = db.Tasks.Include(t => t.Categories)
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
            var existingTask = db.Tasks.FirstOrDefault(e => e.Id == id);

            if (existingTask == null)
            {
                return Results.NotFound($"Task with id {id} not found");
            }


            db.Tasks.Remove(existingTask);
            db.SaveChanges();
            return Results.Ok($"Task with id - {id} deleted successfully");

        });



        app.MapGet("/task/category/{categoryId}", (int categoryId, AppDbContext db) =>
        {

            var categoryExists = db.Categories.Any(c => c.CategoryId == categoryId);
            if (!categoryExists)
            {
                return Results.NotFound($"Категорії з ID {categoryId} не існує");
            }


            var response = db.Tasks
                .Include(t => t.Categories)
                .Where(task => task.Categories.Any(c => c.CategoryId == categoryId))
                .Select(task => new GetTaskDto(
                    task.Id,
                    task.Title,
                    task.IsComplete,
                    task.Categories.Select(c => c.CategoryName).ToList(),
                    task.Deadline

                )).ToList();

            return Results.Ok(response);
        });


        app.MapGet("/tasks/overdue", (AppDbContext db) =>
        {
            var response = db.Tasks
                .Include(t => t.Categories)
                .Where(task => !task.IsComplete && task.Deadline != null && DateTime.UtcNow > task.Deadline)
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
}