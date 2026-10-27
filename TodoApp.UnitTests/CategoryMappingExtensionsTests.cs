using TodoApp.Core.DTOs;
using TodoApp.Core.Entities;
using TodoApp.Core.Mappings;

namespace TodoApp.UnitTests;

public class CategoryMappingExtensionsTests
{
    [Theory]
    [InlineData("Work")]
    [InlineData("Дім і сім'я")]
    public void ToEntity_CopiesNameFromDto(string name)
    {
        var dto = new CategoryCreateDto(name);

        var entity = dto.ToEntity();

        Assert.Equal(name, entity.Name);
    }

    [Fact]
    public void ToEntity_LeavesOwnerForService()
    {
        var entity = new CategoryCreateDto("Work").ToEntity();

        Assert.Equal(Guid.Empty, entity.UserId);
    }

    [Fact]
    public void ToCategoryDto_CopiesIdAndName()
    {
        var entity = new Category { Name = "Work", UserId = Guid.NewGuid() };

        var dto = entity.ToCategoryDto();

        Assert.Equal(new CategoryDto(entity.Id, "Work"), dto);
    }

    [Fact]
    public void MapTo_UpdatesName_KeepsIdAndOwner()
    {
        var ownerId = Guid.NewGuid();
        var category = new Category { Name = "Old", UserId = ownerId };
        var originalId = category.Id;

        new CategoryUpdateDto("New").MapTo(category);

        Assert.Equal("New", category.Name);
        Assert.Equal(originalId, category.Id);
        Assert.Equal(ownerId, category.UserId);
    }
}