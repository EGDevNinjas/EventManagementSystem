using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class 
    {
        IQueryable<T> GetAll(); // 
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> predicate);
        public Task<int> CountAsync();
        public IQueryable<T> GetPaged(int pageNumber, int pageSize);
    }
}
