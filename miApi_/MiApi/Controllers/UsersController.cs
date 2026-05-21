using MiApp.Domain.Entities;
using MiApp.Infrastructure.DTO;
using MiApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace MiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los usuarios
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetUserDto>>> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users.Select(u => new GetUserDto
        {
            Id = u.Id,
            Email = u.Email,
            Name = u.Name,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        }));
    }

    /// <summary>
    /// Obtiene un usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetUserDto>> GetById(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        return Ok(new GetUserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        });
    }

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<GetUserDto>> Register(CreateUserDto dto)
    {
        // Verificar si el email ya existe
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingUser != null)
            return BadRequest("El email ya está registrado");

        try
        {
            var passwordHash = HashPassword(dto.Password);
            var user = new User(dto.Email, dto.Name, passwordHash);
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new GetUserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un usuario
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        try
        {
            if (!string.IsNullOrEmpty(dto.Name))
                user.GetType().GetProperty("Name")?.SetValue(user, dto.Name);
            
            if (!string.IsNullOrEmpty(dto.Email))
                user.GetType().GetProperty("Email")?.SetValue(user, dto.Email);

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un usuario
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
