using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace CherrySeed.Repositories.Ef
{
    public class EfRepository : IRepository
    {
        private readonly DbContext _dbContext;

        public EfRepository(DbContext createDbContextFunc)
        {
            _dbContext = createDbContextFunc;
        }

        public void SaveEntity(object item)
        {
            _dbContext.Entry(item).State = EntityState.Added;
            _dbContext.SaveChanges();
        }

        public void AddOrUpdateEntity<T>(Expression<Func<T, object>> identifier, T item)
        {
            var dbsetmethodinfo = _dbContext.GetType().GetMethod("Set");
            dynamic dbset = dbsetmethodinfo.MakeGenericMethod(typeof(T)).Invoke(_dbContext, null);
            dbset.AddOrUpdate(identifier, item);
            _dbContext.SaveChanges();
        }

        public void RemoveEntities(Type type)
        {
            var dbsetmethodinfo = _dbContext.GetType().GetMethod("Set");
            dynamic dbset = dbsetmethodinfo.MakeGenericMethod(type).Invoke(_dbContext, null);

            foreach (var entity in dbset)
            {
                _dbContext.Entry(entity).state = EntityState.Deleted;
            }
        }

        public object LoadEntity(Type type, object id)
        {
            return _dbContext.Find(type, id);
        }
    }
}