﻿using BoardGameVoter.Data.Shared;
using BoardGameVoter.Models.Shared;
using BoardGameVoter.Services;
using Microsoft.EntityFrameworkCore;

namespace BoardGameVoter.Repositorys.Shared
{
    public abstract class RepositoryBase<TEntity> : RepositoryBase<TEntity, TEntity, RepositoryLoadOptions>
        where TEntity : EntityBase
    {
        protected RepositoryBase(IBGVServiceProvider bGVServiceProvider)
            : base(bGVServiceProvider)
        {
        }
    }

    public abstract class RepositoryBase<TEntity, TLoadOptions> : RepositoryBase<TEntity, TEntity, TLoadOptions>
        where TEntity : EntityBase
        where TLoadOptions : RepositoryLoadOptions, new()
    {
        protected RepositoryBase(IBGVServiceProvider bGVServiceProvider)
            : base(bGVServiceProvider)
        {
        }

        protected RepositoryBase(IBGVServiceProvider bGVServiceProvider, TLoadOptions loadWithOptions)
            : base(bGVServiceProvider, loadWithOptions)
        {
        }
    }

    public abstract class RepositoryBase<TEntity, TContextEntity, TLoadOptions> : IRepositoryBase<TEntity, TLoadOptions>
        where TEntity : EntityBase
        where TContextEntity : EntityBase
        where TLoadOptions : RepositoryLoadOptions, new()
    {

        private DBContextBase<TContextEntity> __DBContext;
        private TLoadOptions __LoadWith;

        public RepositoryBase(IBGVServiceProvider bGVServiceProvider)
        {
            __DBContext = (DBContextBase<TContextEntity>?)bGVServiceProvider.DBContextService.GetDBContext(typeof(TContextEntity));
        }

        public RepositoryBase(IBGVServiceProvider bGVServiceProvider, TLoadOptions loadWithOptions)
        {
            __DBContext = (DBContextBase<TContextEntity>?)bGVServiceProvider.DBContextService.GetDBContext(typeof(TContextEntity));
            __LoadWith = loadWithOptions;
        }

        public TEntity? Add(TEntity entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity), "Entity can not be null"); }
            try
            {
                if (entity.UID == Guid.Empty)
                {
                    entity.UID = Guid.NewGuid();
                }

                DBContext.Add(entity);
                DBContext.SaveChanges();
                return entity;
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<TEntity>? Add(IEnumerable<TEntity> entityList)
        {
            if (entityList == null) { throw new ArgumentNullException(nameof(entityList), "Entity List can not be null"); }
            try
            {
                foreach (TEntity entity in entityList)
                {
                    if (entity.UID == Guid.Empty)
                    {
                        entity.UID = Guid.NewGuid();
                    }
                }
                DBContext.AddRange(entityList);
                DBContext.SaveChanges();
                return entityList;
            }
            catch
            {
                return null;
            }
        }

        public bool Delete(TEntity entity)
        {
            try
            {
                DBContext.Remove(entity);
                DBContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(IEnumerable<TEntity> entityList)
        {
            try
            {
                DBContext.RemoveRange(entityList);
                DBContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Data.ToList();
        }

        public TEntity? GetByID(int id)
        {
            return Data.Find(id);
        }

        public IEnumerable<TEntity> GetByID(IEnumerable<int> idList)
        {
            return Data.Where(entity => idList.Contains(entity.ID));
        }

        public TEntity? GetByUID(Guid uid)
        {
            return Data.FirstOrDefault(entity => entity.UID.CompareTo(uid) == 0);
        }

        public IEnumerable<TEntity> GetByUID(IEnumerable<Guid> uidList)
        {
            return Data.Where(entity => uidList.Contains(entity.UID));
        }

        public bool Update(TEntity entity)
        {
            try
            {
                DBContext.Update(entity);
                DBContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(IEnumerable<TEntity> entityList)
        {
            try
            {
                foreach (TEntity entity in entityList)
                {
                    DBContext.Update(entity);
                }
                DBContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public DbSet<TEntity> Data { get => __DBContext.Set<TEntity>(); }

        private DBContextBase<TContextEntity> DBContext { get => __DBContext; }

        public TLoadOptions LoadWith
        {
            get
            {
                if (__LoadWith == null)
                {
                    __LoadWith = new TLoadOptions();
                }
                return __LoadWith;
            }
            set
            {
                __LoadWith = value;
            }
        }
    }
}
