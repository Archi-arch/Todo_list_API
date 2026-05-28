using Microsoft.EntityFrameworkCore;
using My_todo_API.Models;


namespace My_todo_API.Data;


public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) // з цього щ о тут написано нічого толком не зрозуміло ми створиил клас передали в нього параметри якісь чому так що ознає :Dbcontext ?
{
    public DbSet<TodoTask> Tasks => Set<TodoTask>(); // тут щоь зовсім нічог онеясно 
}