namespace My_todo_API.Models;


public class Category
{
    public int CategoryId {get; set;}
    public string CategoryName {get; set;} = string.Empty;

    public List<TodoTask> Tasks {get; set;} = new();
}