using Domain.Entities;
using Domain.Entities.Repositories;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Create(Usuario usuario, CancellationToken ct = default)
    {
        await _context.Usuarios.AddAsync(usuario, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExisteEmail(string email, CancellationToken ct = default)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);
    }

    public async Task<Usuario?> GetByEmail(string email, CancellationToken ct = default)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant() && u.Ativo, ct);
    }

    public async Task<Usuario?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.Ativo, ct);
    }
}
