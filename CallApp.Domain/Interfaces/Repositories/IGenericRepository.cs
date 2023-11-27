using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<bool> Any(Expression<Func<T, bool>> predicate);

        Task<T?> GetById(int Id);
        Task<T?> FindFirst(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllWhereAsync(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();
        IQueryable<T> GetAllWhere(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetPagedResult(IQueryable<T> entity, int? page, int? count);

        Task Add(T entity, CancellationToken cancellationToken = default);
        Task AddRange(IEnumerable<T> entity, CancellationToken cancellationToken = default);

        Task Update(T entity, CancellationToken cancellationToken = default);
        Task UpdateRange(IEnumerable<T> entity, CancellationToken cancellationToken = default);

        Task Delete(T entity, CancellationToken cancellationToken = default);
        Task DeleteRange(IEnumerable<T> entity, CancellationToken cancellationToken = default);
        Task SoftDelete(T entity, CancellationToken cancellationToken);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
