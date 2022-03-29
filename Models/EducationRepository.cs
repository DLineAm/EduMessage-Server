using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SignalIRServerTest.Models
{
    public class EducationRepository<TEntity> where TEntity : class
    {
        internal EducationContext Context;
        internal DbSet<TEntity> DbSet;

        public EducationRepository(EducationContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        //params Expression<Func<TEntity, TProperty>>[] includeProperties

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var prop in includeProperties.Split(
                         new[]{','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(prop);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        private IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet;
            return includeProperties
                .Aggregate(query, (curr, prop) => curr.Include(prop));
        }

        public virtual TEntity GetById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual TEntity GetByExpression(Expression<Func<TEntity, bool>> expression)
        {
            return DbSet.FirstOrDefault(expression);
        }

        public virtual EntityEntry<TEntity> Insert(TEntity entity)
        {
            return DbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }

            DbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            if (Context.Entry(entityToUpdate).State == EntityState.Detached)
            {
                DbSet.Attach(entityToUpdate);
            }
            //DbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

    }
}