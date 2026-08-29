using System;

namespace TodoApp.Core.Entities;

public class TodoTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;

    // Foreign Key for Category
    public Guid? CategoryId { get; set; }
    
    // Navigation property
    public Category? Category { get; set; }
}