using System.Collections.Generic;

namespace TodoApp.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    // Navigation property for Entity Framework Core
    public ICollection<TodoTask> Tasks { get; set; } = new List<TodoTask>();
}