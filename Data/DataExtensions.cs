using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using My_todo_API.Models;

namespace My_todo_API.Data;


public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app) // сстворюємо клас для міграцій
    {
        using var scope = app.Services.CreateScope(); // створюємо скоп але для чого ?
        var DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // нічог оне зрозуміло

        DbContext.Database.Migrate(); // запустити міграцію ?
    }


public static void AddTodoDb(this WebApplicationBuilder builder) // весь код нижче зовісм незрозумілий мені 
    {
        
        const string connString = "Data Source=todo.db"; // а де якийсь пароль логін чи цьог онетреба ?

        builder.Services.AddSqlite<AppDbContext>(
            connString,
            optionsAction: options => options.UseSeeding((context, _) =>
            {
                
                if (!context.Set<TodoTask>().Any())
                {
                    
                    context.Set<TodoTask>().AddRange(
                        new TodoTask { Title = "Вітаємо! Це твоє перше завдання з бази даних", IsComplete = false }
                    );

                    context.SaveChanges();
                }
            })
        );
    }
}