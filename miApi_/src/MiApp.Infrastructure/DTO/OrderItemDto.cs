namespace MiApp.Infrastructure.DTO;

/// <summary>
/// DTO para crear un item en una orden
/// </summary>
public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// DTO para obtener información de un item en una orden
/// </summary>
public class GetOrderItemDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}
