namespace My_todo_API.dto;

public record GetTaskDto(
    int Id,
    string Title,
    bool IsComplete,
    List<string> CategoryNames,
    DateTime? Deadline
    
    

);