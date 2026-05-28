using System.ComponentModel.DataAnnotations;

namespace  My_todo_API.dto;

public record CreateTaskDto(
    
    [Required][StringLength(100)] string Title
);

    
