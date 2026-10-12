namespace TodoApp.Core.Entities;

public class Category : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation properties for Entity Framework Core
    public User User { get; set; } = null!;
    public ICollection<TodoTask> Tasks { get; set; } = new List<TodoTask>();
}