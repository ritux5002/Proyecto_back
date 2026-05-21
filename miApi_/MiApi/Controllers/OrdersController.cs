using MiApp.Domain.Entities;
using MiApp.Infrastructure.DTO;
using MiApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todas las órdenes
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetOrderDto>>> GetAll()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .ToListAsync();

        return Ok(orders.Select(o => new GetOrderDto
        {
            Id = o.Id,
            UserId = o.UserId,
            CreatedAt = o.CreatedAt,
            Status = o.Status.ToString(),
            Total = o.Total,
            Items = o.Items.Select(i => new GetOrderItemDto
            {
                Id = i.Id,
                OrderId = i.OrderId,
                ProductId = i.ProductId,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                Subtotal = i.Subtotal
            }).ToList()
        }));
    }

    /// <summary>
    /// Obtiene una orden por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetOrderDto>> GetById(Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        return Ok(new GetOrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            CreatedAt = order.CreatedAt,
            Status = order.Status.ToString(),
            Total = order.Total,
            Items = order.Items.Select(i => new GetOrderItemDto
            {
                Id = i.Id,
                OrderId = i.OrderId,
                ProductId = i.ProductId,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                Subtotal = i.Subtotal
            }).ToList()
        });
    }

    /// <summary>
    /// Obtiene las órdenes de un usuario
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<GetOrderDto>>> GetByUserId(Guid userId)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ToListAsync();

        return Ok(orders.Select(o => new GetOrderDto
        {
            Id = o.Id,
            UserId = o.UserId,
            CreatedAt = o.CreatedAt,
            Status = o.Status.ToString(),
            Total = o.Total,
            Items = o.Items.Select(i => new GetOrderItemDto
            {
                Id = i.Id,
                OrderId = i.OrderId,
                ProductId = i.ProductId,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                Subtotal = i.Subtotal
            }).ToList()
        }));
    }

    /// <summary>
    /// Crea una nueva orden
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GetOrderDto>> Create(CreateOrderDto dto)
    {
        try
        {
            // Verificar que el usuario existe
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                return BadRequest("Usuario no encontrado");

            var order = new Order(dto.UserId);

            // Agregar items a la orden
            foreach (var itemDto in dto.Items)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                    return BadRequest($"Producto {itemDto.ProductId} no encontrado");

                order.AddItem(product, itemDto.Quantity);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, new GetOrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                Total = order.Total,
                Items = order.Items.Select(i => new GetOrderItemDto
                {
                    Id = i.Id,
                    OrderId = i.OrderId,
                    ProductId = i.ProductId,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Subtotal = i.Subtotal
                }).ToList()
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina una orden
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
