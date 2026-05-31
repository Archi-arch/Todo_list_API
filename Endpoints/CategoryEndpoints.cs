using My_todo_API.Models;
using My_todo_API.dto;
using My_todo_API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace My_todo_API.Endpoints;


public static class CategoryEndpoints
{

    public static void MapCategoryEndpoints(this WebApplication app)
    {
        app.MapGet("/categories", (AppDbContext db) =>
        {
            var responese = db.Categories.Select(Category => new GetCategoryDto(

                Category.CategoryId,
                Category.CategoryName

            )).ToList();


            return Results.Ok(responese);
        });


        app.MapPost("/categories", (CreateCategoryDto dto, AppDbContext db) =>
        {

            var existingCategory = db.Categories.Any(c => 
            c.CategoryName.ToLower() == dto.CategoryName.ToLower());

            if (existingCategory)
            {
                return Results.BadRequest($"Category with this name wa exists");
            }

            var newCategory = new Category
            {
                CategoryName = dto.CategoryName
            };

            db.Categories.Add(newCategory);
            db.SaveChanges();

            return Results.Ok($"Category created");

        });


        app.MapPut("/categories/{Id}", (int Id, UpdateCategoryDto dto, AppDbContext db) =>
        {
            var existingCategory = db.Categories.FirstOrDefault(c => c.CategoryId == Id);


            if (existingCategory == null)
            {
                return Results.NotFound($"Category witnh{Id} not found");
            }

            existingCategory.CategoryName = dto.CategoryName;
            db.SaveChanges();

            return Results.NoContent();

        });


        app.MapDelete("/categories/{Id}", (int Id, AppDbContext db) =>
        {
            var existingCategory = db.Categories.FirstOrDefault(c => c.CategoryId == Id);


            if (existingCategory == null)
            {
                return Results.NotFound($"Category witnh{Id} not found");
            }

            var hasTasks = db.Tasks.Any(t => t.Categories.Any(c => c.CategoryId == Id));

            if (hasTasks)
            {
                return Results.BadRequest($"Category has  tasks");
            }

            db.Categories.Remove(existingCategory);
            db.SaveChanges();
            return Results.Ok($"Category {Id} deleted");
        });
    }
}