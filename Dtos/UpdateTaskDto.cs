using System.ComponentModel.DataAnnotations;

namespace My_todo_API.dto;


public record UpdateTaskDto(

    [Required][StringLength(100)]string Title,
    bool IsComplete,
    int CategoryId

);