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
            var categoryExists = db.Categories.Any(c => c.CategoryId == dto.CategoryId);

            if (!categoryExists)
            {
                return Results.BadRequest($"category wint {dto.CategoryId} not fount");
            }


            if (dto.Deadline != null | dto.Deadline < DateTime.UtcNow)
            {

                if (dto.Deadline > DateTime.UtcNow.AddDays(365))
                {
                    return Results.BadRequest($"Deadline cant be future more than 365 days");
                }
                return Results.BadRequest($"Dedline cant be past");
            }

            var newTask = new TodoTask
            {

                Title = dto.Title,
                IsComplete = false,
                CategoryId = dto.CategoryId,
                Deadline = dto.Deadline
            };

            db.Tasks.Add(newTask);
            db.SaveChanges();
            return Results.Created($"/task/{newTask.Id}", newTask);


        });



        app.MapPut("/task/{id}", (int id, UpdateTaskDto dto, AppDbContext db) =>
        {
            var existingTask = db.Tasks.FirstOrDefault(t => t.Id == id);


            if (existingTask == null)
            {
                return Results.NotFound($"Task with id {id} not found");
            }
            var categoryExists = db.Categories.Any(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
            {
                return Results.BadRequest($"Категорії з ID {dto.CategoryId} не існує!");
            }

            if (dto.Deadline != null | dto.Deadline < DateTime.UtcNow)
            {

                if (dto.Deadline > DateTime.UtcNow.AddDays(365))
                {
                    return Results.BadRequest($"Deadline cant be future more than 365 days");
                }
                return Results.BadRequest($"Dedline cant be past");
            }

            existingTask.Title = dto.Title;
            existingTask.IsComplete = dto.IsComplete;
            existingTask.CategoryId = dto.CategoryId;
            existingTask.Deadline = dto.Deadline;

            db.SaveChanges();

            return Results.NoContent();

        });


        app.MapGet("/task", (AppDbContext db) =>
        {
            var response = db.Tasks.Include(t => t.Category)
            .Select(task => new GetTaskDto(
                task.Id,
                task.Title,
                task.IsComplete,
                (!task.IsComplete && task.Deadline != null && DateTime.UtcNow > task.Deadline)
                ? "Протерміновано"
                : (task.Category != null ? task.Category.CategoryName : "Без категорії"),
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
                .Include(t => t.Category)
                .Where(task => task.CategoryId == categoryId)
                .Select(task => new GetTaskDto(
                    task.Id,
                    task.Title,
                    task.IsComplete,
                    task.Category != null ? task.Category.CategoryName : "Без категорії",
                    task.Deadline

                )).ToList();

            return Results.Ok(response);
        });


        app.MapGet("/tasks/overdue", (AppDbContext db) =>
        {
            var response = db.Tasks
            .Include(t => t.Category)
            .Where(task => !task.IsComplete && task.Deadline != null && DateTime.UtcNow > task.Deadline)
             .Select(task => new GetTaskDto(
                task.Id,
                task.Title,
                task.IsComplete,
                "Протерміновано",
                task.Deadline
             )).ToList();

            return Results.Ok(response);
        });
    }
}