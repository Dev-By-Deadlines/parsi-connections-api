using Connecions.Api.Models;

namespace Connecions.Api.Dtos;

public record StatsDto(
    int TotalPlayers,
    double WinRate,
    double AverageRemainingHealth,
    double PlayerPercentile,
    int PlayerHealth,
    Outcomes PlayerOutcome
);
