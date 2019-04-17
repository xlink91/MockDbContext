using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mocks.MockPersistence.Contract
{
    public interface IDbContext<TKey>
    {
        IEnumerable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> cond)
            where TEntity : class, IKeyIdentity<TKey>;
        void Add<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<TKey>;
        void Update<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<TKey>;
        void Remove<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<TKey>;
    }
}
