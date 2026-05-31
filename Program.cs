using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Builder;
using My_todo_API.Models;
using My_todo_API.Data;
using My_todo_API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddTodoDb();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()   // Дозволяємо запити з будь-якого сайту
              .AllowAnyMethod()   // Дозволяємо будь-які методи (GET, POST, PUT, DELETE)
              .AllowAnyHeader();  // Дозволяємо будь-які заголовки
    });
});

var app = builder.Build();



app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


var myTasksList = new List<TodoTask>();


app.MapTaskEndpoints();
app.MapCategoryEndpoints();

app.MigrateDb();

app.Run();

