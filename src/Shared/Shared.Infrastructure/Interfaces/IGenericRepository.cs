using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Shared.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetByAsync(Expression<Func<T, bool>> expression);
        Task DeleteRangeAsync(ICollection<T> entities);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdStringAsync(string id);
        Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<T> GetByNameAsync(string name);
        Task SaveChangesAsync();
        IDbContextTransaction BeginTransaction();
        void Commit();
        void RollBack();
        IQueryable<T> GetTableNoTracking();
        IQueryable<T> GetTableAsTracking();
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        Task<T> UpdateAsync(T entity);
        Task UpdateRangeAsync(ICollection<T> entities);
        Task DeleteAsync(T entity);
        Task<bool> DeleteByIdAsync(int id);
        Task<bool> DeleteByIdStringAsync(string id);
        string Exsitance(string message);
    }
}
