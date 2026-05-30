using Microsoft.EntityFrameworkCore;
using My_todo_API.Models;


namespace My_todo_API.Data;


public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) 
{
    public DbSet<TodoTask> Tasks => Set<TodoTask>(); 

    public DbSet<Category> Categories => Set<Category>();
}