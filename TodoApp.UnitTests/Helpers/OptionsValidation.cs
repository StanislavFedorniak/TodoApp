using System.ComponentModel.DataAnnotations;

namespace TodoApp.UnitTests.Helpers;

public static class OptionsValidation
{
    public static bool IsValid(object options)
    {
        var context = new ValidationContext(options);

        return Validator.TryValidateObject(options, context,
            new List<ValidationResult>(), validateAllProperties: true);
    }
}
