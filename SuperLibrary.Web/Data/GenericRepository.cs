using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
{
    private readonly DataContext _context;

    public GenericRepository(DataContext context)
    {
        _context = context;
    }

    /// <summary>
    /// (C)reates an Entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await SaveAllAsync();
    }

    /// <summary>
    /// Saves all Data asyncronasly
    /// </summary>
    /// <returns></returns>
    private async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Soft (D)eletes the Entity and Updates it
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task DeleteAsync(T entity)
    {
        entity.WasDeleted = true;
        await UpdateAsync(entity);
    }

    /// <summary>
    /// Checks if Entity exists and returns true or false (Soft Deleted will always return false)
    /// </summary>
    /// <param name="id"></param>
    /// <returns>bool</returns>
    public async Task<bool> ExistAsync(int id)
    {
        return await _context.Set<T>().AnyAsync(e => e.Id == id && !e.WasDeleted);
    }

    /// <summary>
    /// (R)eads all Entities (Except Soft Deleted)
    /// </summary>
    /// <returns>Entity</returns>
    public IQueryable<T> GetAll()
    {
        return _context.Set<T>().AsNoTracking().Where(e => !e.WasDeleted);
    }

    /// <summary>
    /// (R)eads a specific Entity by Id and returns it (Except Soft Deleted)
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Entity</returns>
    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && !e.WasDeleted);
    }

    /// <summary>
    /// (U)pdates all changes made to the Entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await SaveAllAsync();
    }
}