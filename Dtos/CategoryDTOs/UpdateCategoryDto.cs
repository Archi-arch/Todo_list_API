using System.ComponentModel.DataAnnotations;

namespace My_todo_API.dto;


public record UpdateCategoryDto
(
    [Required][StringLength(15)] string CategoryName
);

