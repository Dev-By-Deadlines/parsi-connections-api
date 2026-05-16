namespace Connecions.Api.Dtos;

public record PaginatedResponse<T>(
        List<T> Items,
        int Page,
        int Limit,
        long Total,
        int TotalPages
        )
{ }
