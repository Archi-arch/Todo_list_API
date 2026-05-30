using System.ComponentModel.DataAnnotations;

namespace My_todo_API.dto;


public record CreateCategoryDto
(
    [Required][StringLength(15)] string CategoryName
);