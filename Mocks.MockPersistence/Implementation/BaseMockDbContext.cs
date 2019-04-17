using Mocks.Entities.BaseEntityContract;
using Mocks.MockPersistence.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mocks.MockPersistence.Implementation
{
    public class BaseMockDbContext<TKey> : IDbContext<TKey>
    {
        private IDbContextUtility<TKey> DbContextUtility { get; set; }
        public BaseMockDbContext(IDbContextUtility<TKey> dBContextUtility)
        {
            DbContextUtility = dBContextUtility;
        }

        public void Add<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<TKey>
        {
            ICollection<TEntity> entityCollection = (ICollection<TEntity>)GetCollection(typeof(TEntity));
            entity.Id = DbContextUtility.GenerateNewKey();
            entityCollection.Add(entity);
        }

        public IEnumerable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> cond)
            where TEntity : class, IKeyIdentity<TKey>
        {
            IEnumerable<TEntity> entityCollection = (IEnumerable<TEntity>)GetList(typeof(TEntity));
            return entityCollection?.Where(cond.Compile());
        }

        protected IEnumerable<object> GetList(Type type)
        {
            Type DbContextType = this.GetType();
            foreach (PropertyInfo propertyInfo in DbContextType.GetProperties())
            {
                if (propertyInfo.PropertyType.GetInterface(typeof(IEnumerable<>).Name) != null && propertyInfo.Name == type.Name)
                    return (IEnumerable<object>)propertyInfo.GetValue(this);
            }
            throw new Exception($"{type.Name} are not declare in context as {nameof(IEnumerable<object>)}");
        }

        protected object GetCollection(Type type)
        {
            Type DbContextType = this.GetType();
            foreach (PropertyInfo propertyInfo in DbContextType.GetProperties())
            {
                if (propertyInfo.PropertyType.Name == typeof(ICollection<>).Name && propertyInfo.Name == type.Name)
                    return propertyInfo.GetValue(this);
            }
            throw new Exception($"{type.Name} are not declare in context as ICollection<>");
        }

        protected void UpdateCollection(Type type, object collection)
        {
            PropertyInfo prop = this.GetType().GetProperty(type.Name);
            prop.SetValue(this, collection);
        }

        public void Update<TEntity>(TEntity entity)
            where TEntity : class, IKeyIdentity<TKey>
        {
            ICollection<TEntity> collection = (ICollection<TEntity>)GetCollection(typeof(TEntity));
            List<TEntity> nCollection = new List<TEntity>();

            foreach (var _entity in collection)
            {
                if (_entity.Id.Equals(entity.Id))
                    continue;
                nCollection.Add(_entity);
            }
            nCollection.Add(entity);
            UpdateCollection(typeof(TEntity), nCollection);
        }

        void IDbContext<TKey>.Remove<TEntity>(TEntity entity)
        {
            ICollection<TEntity> collection = (ICollection<TEntity>)GetCollection(typeof(TEntity));
            TEntity entityStored = null;
            foreach (var elem in collection)
            {
                if(elem.Id.Equals(entity.Id))
                {
                    entityStored = elem;
                    break;
                }
            }

            if(entityStored != null)
                collection.Remove(entityStored);
        }
    }
}
