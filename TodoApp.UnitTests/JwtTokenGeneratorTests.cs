using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Core.Entities;
using TodoApp.Services;
using TodoApp.Services.Options;

namespace TodoApp.UnitTests;

public class JwtTokenGeneratorTests
{
    private const string Issuer = "todoapp-tests";
    private const string Audience = "todoapp-tests-clients";
    private const string Key = "unit-tests-signing-key-0123456789-abcdef";

    private static JwtTokenGenerator CreateGenerator(int expirationMinutes = 60)
    {
        var settings = new JwtSettings
        {
            Issuer = Issuer,
            Audience = Audience,
            Key = Key,
            ExpirationMinutes = expirationMinutes
        };

        return new JwtTokenGenerator(Options.Create(settings));
    }

    private static User CreateUser() => new() { Email = "user@example.com" };

    private static TokenValidationParameters CreateValidationParameters(string key = Key) => new()
    {
        ValidIssuer = Issuer,
        ValidAudience = Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ClockSkew = TimeSpan.Zero
    };

    [Fact]
    public async Task Generate_ReturnsTokenAcceptedByValidationRules()
    {
        var response = CreateGenerator().Generate(CreateUser());

        var result = await new JsonWebTokenHandler()
            .ValidateTokenAsync(response.Token, CreateValidationParameters());

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Generate_ReturnsTokenRejectedWithDifferentKey()
    {
        var response = CreateGenerator().Generate(CreateUser());

        var result = await new JsonWebTokenHandler()
            .ValidateTokenAsync(response.Token, CreateValidationParameters("another-signing-key-0123456789-abcdefgh"));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Generate_PutsUserIdAndEmailIntoClaims()
    {
        var user = CreateUser();

        var jwt = new JsonWebToken(CreateGenerator().Generate(user).Token);

        Assert.Equal(user.Id.ToString(), jwt.GetClaim(JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(user.Email, jwt.GetClaim(JwtRegisteredClaimNames.Email).Value);
        Assert.Equal(Issuer, jwt.Issuer);
        Assert.Contains(Audience, jwt.Audiences);
    }

    [Fact]
    public void Generate_DoesNotLeakPasswordHash()
    {
        var user = CreateUser();
        user.PasswordHash = "secret-password-hash";

        var jwt = new JsonWebToken(CreateGenerator().Generate(user).Token);

        Assert.DoesNotContain(jwt.Claims, c => c.Value == user.PasswordHash);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(60)]
    [InlineData(1440)]
    public void Generate_SetsExpirationFromSettings(int minutes)
    {
        var before = DateTime.UtcNow;
        var response = CreateGenerator(minutes).Generate(CreateUser());
        var after = DateTime.UtcNow;

        Assert.InRange(response.ExpiresAt, before.AddMinutes(minutes), after.AddMinutes(minutes));

        var jwt = new JsonWebToken(response.Token);
        Assert.InRange(jwt.ValidTo, response.ExpiresAt.AddSeconds(-1), response.ExpiresAt.AddSeconds(1));
    }

    [Fact]
    public void Generate_ForSameUserTwice_ProducesDifferentJti()
    {
        var generator = CreateGenerator();
        var user = CreateUser();

        var first = new JsonWebToken(generator.Generate(user).Token);
        var second = new JsonWebToken(generator.Generate(user).Token);

        Assert.NotEqual(first.Id, second.Id);
    }
}