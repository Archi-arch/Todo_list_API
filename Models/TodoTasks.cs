namespace My_todo_API.Models;


public class TodoTask
{
    public int Id {get;set;}
    public string Title {get; set;} = string.Empty;
    public bool IsComplete {get;set;}

    public int CategoryId {get;  set;}

    public Category? Category{get; set;}
}