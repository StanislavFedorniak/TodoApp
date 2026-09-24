using Microsoft.EntityFrameworkCore;
using TodoApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Замінюємо AddOpenApi() на AddSwaggerGen()
builder.Services.AddSwaggerGen(); 
builder.Services.AddControllers();

// Налаштування CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

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

app.UseRouting();

// UseCors має бути розташований ПІСЛЯ UseRouting та ПЕРЕД UseAuthorization
app.UseCors("AllowAngularDev");

app.UseAuthorization();
app.MapControllers();

app.Run();