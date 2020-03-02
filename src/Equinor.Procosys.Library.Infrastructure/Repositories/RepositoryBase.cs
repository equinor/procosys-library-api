﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Domain;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Library.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : EntityBase, IAggregateRoot
    {
        protected readonly DbSet<TEntity> Set;
        protected readonly IQueryable<TEntity> DefaultQuery;

        protected RepositoryBase(DbSet<TEntity> set)
            : this(set, set)
        {
        }

        protected RepositoryBase(DbSet<TEntity> set, IQueryable<TEntity> defaultQuery)
        {
            Set = set;
            DefaultQuery = defaultQuery;
        }

        public virtual void Add(TEntity entity) =>
            Set.Add(entity);

        public Task<bool> Exists(int id) =>
            DefaultQuery.AnyAsync(x => x.Id == id);

        public virtual Task<List<TEntity>> GetAllAsync() =>
            DefaultQuery.ToListAsync();

        public virtual Task<TEntity> GetByIdAsync(int id) =>
            DefaultQuery.FirstOrDefaultAsync(x => x.Id == id);

        public Task<List<TEntity>> GetByIdsAsync(IEnumerable<int> ids) =>
            DefaultQuery.Where(x => ids.Contains(x.Id)).ToListAsync();

        public virtual void Remove(TEntity entity) =>
            Set.Remove(entity);
    }
}
