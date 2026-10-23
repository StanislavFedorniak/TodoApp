using System.ComponentModel.DataAnnotations;

namespace TodoApp.API.Options;

public class RateLimitSettings
{
    public const string SectionName = "RateLimiting";

    [Range(1, 1000)]
    public int LoginPermitLimit { get; init; } = 5;

    [Range(1, 3600)]
    public int LoginWindowSeconds { get; init; } = 60;

    [Range(1, 1000)]
    public int AccountDeletePermitLimit { get; init; } = 3;

    [Range(1, 3600)]
    public int AccountDeleteWindowSeconds { get; init; } = 60;
}