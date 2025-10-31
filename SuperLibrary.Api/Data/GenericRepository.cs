using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperLibrary.Api.Data.Entities;

namespace SuperLibrary.Api.Data;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
{
    private readonly DataContext _context;

    public GenericRepository(DataContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await SaveAllAsync();
    }

    private async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task DeleteAsync(T entity)
    {
        entity.WasDeleted = true;
        await UpdateAsync(entity);
    }

    public async Task<bool> ExistAsync(int id)
    {
        return await _context.Set<T>().AnyAsync(e => e.Id == id && !e.WasDeleted);
    }

    public IQueryable<T> GetAll()
    {
        return _context.Set<T>().AsNoTracking().Where(e => !e.WasDeleted);
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && !e.WasDeleted);
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await SaveAllAsync();
    }
}
