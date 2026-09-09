using TodoApp.Core.DTOs;
using TodoApp.Core.Entities;

namespace TodoApp.Core.Mappings;

public static class CategoryMappingExtensions
{
    public static Category ToEntity(this CategoryCreateDto request)
    {
        return new Category
        {
            Name = request.Name
        };
    }

    public static CategoryDto ToCategoryDto(this Category entity)
    {
        return new CategoryDto(entity.Id, entity.Name);
    }

    public static void MapTo(this CategoryUpdateDto request, Category category)
    {
        category.Name = request.Name;
    }
}