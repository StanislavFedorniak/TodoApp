using System.ComponentModel.DataAnnotations;
using TodoApp.Services.Options;

namespace TodoApp.UnitTests;

public class JwtSettingsValidationTests
{
    private static bool IsValid(JwtSettings settings)
    {
        var context = new ValidationContext(settings);

        return Validator.TryValidateObject(settings, context,
            new List<ValidationResult>(), validateAllProperties: true);
    }

    private static JwtSettings CreateSettings(
        string issuer = "todoapp",
        string audience = "todoapp-clients",
        int keyLength = 32,
        int expirationMinutes = 60) => new()
    {
        Issuer = issuer,
        Audience = audience,
        Key = new string('k', keyLength),
        ExpirationMinutes = expirationMinutes
    };

    [Fact]
    public void Validate_WithAllRequiredValues_Passes()
    {
        Assert.True(IsValid(CreateSettings()));
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(31, false)]
    [InlineData(32, true)]
    [InlineData(64, true)]
    public void Validate_KeyLength_RequiresAtLeast32Characters(int keyLength, bool expected)
    {
        Assert.Equal(expected, IsValid(CreateSettings(keyLength: keyLength)));
    }

    [Theory]
    [InlineData("", "todoapp-clients")]
    [InlineData("todoapp", "")]
    public void Validate_WithEmptyIssuerOrAudience_Fails(string issuer, string audience)
    {
        Assert.False(IsValid(CreateSettings(issuer: issuer, audience: audience)));
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(1440, true)]
    [InlineData(1441, false)]
    public void Validate_ExpirationMinutes_MustBeWithinOneDay(int minutes, bool expected)
    {
        Assert.Equal(expected, IsValid(CreateSettings(expirationMinutes: minutes)));
    }
}