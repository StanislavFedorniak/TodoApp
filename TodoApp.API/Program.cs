using Microsoft.EntityFrameworkCore;
using TodoApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Замінюємо AddOpenApi() на AddSwaggerGen()
builder.Services.AddSwaggerGen(); 
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<TodoApp.Core.Interfaces.ICategoryService, TodoApp.Services.CategoryService>();
builder.Services.AddScoped<TodoApp.Core.Interfaces.ITodoTaskService, TodoApp.Services.TodoTaskService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Додаємо Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();