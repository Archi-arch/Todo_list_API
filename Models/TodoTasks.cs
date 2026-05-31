namespace My_todo_API.Models;


public class TodoTask
{
    public int Id {get;set;}
    public string Title {get; set;} = string.Empty;
    public bool IsComplete {get;set;}

    public List<Category> Categories { get; set; } = new();

    public DateTime? Deadline{get;set;}
    public bool IsDeleted { get; set; } = false;
 
}