using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using System.Linq.Expressions;
using System.Data.Entity;

namespace ChangerMints.Database
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal DbContext Context;
        internal IDbSet<TEntity> DbSet;

        public Repository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public virtual void Delete(TEntity entity) {
            DbSet.Attach(entity);
            DbSet.Remove(entity);
        }

        public void Delete(object id) {
            throw new NotImplementedException();
        }

        public virtual TEntity FindById(object id)
        {
            return DbSet.Find(id);
        }

      /*  public virtual void InsertGraph(TEntity entity)
        {
            DbSet.Add(entity);
        }*/

        public virtual void Insert(TEntity entity) {
            DbSet.Add(entity);
        }

        public virtual RepositoryQuery<TEntity> Query() {
            var repositoryGetFluentHelper =
                new RepositoryQuery<TEntity>(this);
            return repositoryGetFluentHelper;
        }

        public virtual void Update(TEntity entity)
        {
           // DbSet.Attach(entity);
        }

   /*     public virtual void Delete(object id)
        {
            var entity = DbSet.Find(id);
            var objectState = entity as ObjectState;
            if (objectState != null)
                objectState.State = ObjectState.Deleted;
            Delete(entity);
        }*/
        internal IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>>
                includeProperties = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (includeProperties != null)
                includeProperties.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null)
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);

            return query;
        }
    }
}
