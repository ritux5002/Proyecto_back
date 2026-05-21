namespace MiApp.Infrastructure.DTO;

/// <summary>
/// DTO para crear una nueva orden
/// </summary>
public class CreateOrderDto
{
    public Guid UserId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO para obtener información de una orden
/// </summary>
public class GetOrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<GetOrderItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO para actualizar el estado de una orden
/// </summary>
public class UpdateOrderStatusDto
{
    public string NewStatus { get; set; } = string.Empty;
}
