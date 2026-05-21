using MiApp.Domain.Entities;
using MiApp.Infrastructure.DTO;
using MiApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los productos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetProductDto>>> GetAll()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products.Select(p => new GetProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            CategoryId = p.CategoryId,
            CreatedAt = p.CreatedAt
        }));
    }

    /// <summary>
    /// Obtiene un producto por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetProductDto>> GetById(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        return Ok(new GetProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            CreatedAt = product.CreatedAt
        });
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GetProductDto>> Create(CreateProductDto dto)
    {
        try
        {
            var product = new Product(dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, new GetProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un producto
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        try
        {
            if (!string.IsNullOrEmpty(dto.Name))
                product.GetType().GetProperty("Name")?.SetValue(product, dto.Name);
            
            if (!string.IsNullOrEmpty(dto.Description))
                product.GetType().GetProperty("Description")?.SetValue(product, dto.Description);
            
            if (dto.Price.HasValue)
                product.UpdatePrice(dto.Price.Value);
            
            if (dto.Stock.HasValue)
                product.GetType().GetProperty("Stock")?.SetValue(product, dto.Stock);

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un producto
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
