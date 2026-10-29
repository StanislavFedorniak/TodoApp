using TodoApp.API.Options;

namespace TodoApp.UnitTests;

public class RateLimitSettingsValidationTests
{
    [Fact]
    public void Validate_WithDefaults_Passes()
    {
        Assert.True(OptionsValidation.IsValid(new RateLimitSettings()));
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(1000, true)]
    [InlineData(1001, false)]
    public void Validate_PermitLimit_MustBeBetween1And1000(int limit, bool expected)
    {
        Assert.Equal(expected, OptionsValidation.IsValid(new RateLimitSettings { LoginPermitLimit = limit }));
        Assert.Equal(expected, OptionsValidation.IsValid(new RateLimitSettings { AccountDeletePermitLimit = limit }));
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(3600, true)]
    [InlineData(3601, false)]
    public void Validate_WindowSeconds_MustBeBetween1And3600(int seconds, bool expected)
    {
        Assert.Equal(expected, OptionsValidation.IsValid(new RateLimitSettings { LoginWindowSeconds = seconds }));
        Assert.Equal(expected, OptionsValidation.IsValid(new RateLimitSettings { AccountDeleteWindowSeconds = seconds }));
    }
}