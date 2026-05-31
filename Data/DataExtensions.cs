using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using My_todo_API.Models;

namespace My_todo_API.Data;


public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        DbContext.Database.Migrate();
    }


    public static void AddTodoDb(this WebApplicationBuilder builder)
    {

        const string connString = "Data Source=todo.db";

        builder.Services.AddSqlite<AppDbContext>(
            connString,
            optionsAction: options => options.UseSeeding((context, _) =>
            {


                if (!context.Set<Category>().Any())
                {
                    
                    var workCategory = new Category { CategoryName = "Робота" };
                    var personalCategory = new Category { CategoryName = "Особисте" };

                    
                    context.Set<Category>().AddRange(workCategory, personalCategory);
                    context.SaveChanges(); 

                    
                    if (!context.Set<TodoTask>().Any())
                    {
                        context.Set<TodoTask>().AddRange(
                            new TodoTask
                            {
                                Title = "Вітаємо! Це твоє перше завдання з бази даних",
                                IsComplete = false,
                                Categories = new List<Category> { workCategory, personalCategory }
                            }
                        );

                        context.SaveChanges();
                    }
                }
            })
        );
    }
}