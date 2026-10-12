using System;
using System.Diagnostics.Contracts;

namespace TodoApp.Core.Entities;

public class TodoTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;

    // Foreign Key for Category
    public Guid UserId { get; set; }
    public Guid? CategoryId { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Category? Category { get; set; }
}