namespace My_todo_API.dto;

public record GetTaskDto(
    int Id,
    string Title,
    bool IsComplete,
    string CategoryName,
    DateTime? Deadline
    
    

);