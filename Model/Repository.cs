﻿using System;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;

using XAS.Model.Paging;

namespace XAS.Model {

    /// <summary>
    /// A Generic Repository, an abstract class that implements a Repository Pattern.
    /// </summary>
    /// <typeparam name="T">A database entity object.</typeparam>
    /// 
    public abstract class Repository<T>: Respository<T> where T: class, new() {

        /// <summary>
        /// Get/Set the DbContext.
        /// </summary>
        /// 
        protected DbContext Context { get; private set; }

        /// <summary>
        /// Get/Set the DbSet.
        /// </summary>
        /// 
        protected DbSet<T> DbSet { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">A DbContext object</param>
        /// 
        public Repository(DbContext context) {

            this.Context = context;
            this.DbSet = context.Set<T>();

        }

        /// <summary>
        /// Find a record in the database.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>The record.</returns>
        /// 
        public virtual T Find(Expression<Func<T, bool>> predicate) {

            return this.DbSet.Where(predicate).SingleOrDefault();

        }

        /// <summary>
        /// Search for records in the database.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>A list of records.</returns>
        /// 
        public virtual IEnumerable<T> Search(Expression<Func<T, bool>> predicate = null) {

            IQueryable<T> query = this.DbSet;

            if (predicate != null) {

                query = query.Where(predicate);

            }

            return query.ToList();

        }


        /// <summary>
        /// Create a record in the database.
        /// </summary>
        /// <param name="entity"></param>
        /// 
        public virtual void Create(T entity) {

            this.DbSet.Add(entity);

        }

        /// <summary>
        /// Delete a record from the database.
        /// </summary>
        /// <param name="id">The index of the record.</param>
        /// 
        public virtual void Delete(object id) {

            var entity = this.DbSet.Find(id);

            if (this.Context.Entry(entity).State == EntityState.Detached) {

                this.DbSet.Attach(entity);

            }

            this.DbSet.Remove(entity);

        }

        /// <summary>
        /// Update a record in the databse.
        /// </summary>
        /// <param name="entity"></param>
        /// 
        public virtual void Update(T entity) {

            this.DbSet.Attach(entity);
            this.Context.Entry(entity).State = EntityState.Modified;

        }

        /// <summary>
        /// Page thru the results of a database search.
        /// </summary>
        /// <param name="criteria">Paging criteria.</param>
        /// <returns></returns>
        /// 
        public IPaged<T> Page(ICriteria<T> criteria) {

            var query = this.Search();
            var totalRecords = query.Count();

            if (criteria.SortDir.Any()) {

                foreach (var kvp in criteria.SortDir) {

                    switch (kvp.Value) {
                        case ListSortDirection.Ascending:
                            query = query.OrderBy(r => kvp.Key);
                            break;

                        case ListSortDirection.Descending:
                            query = query.OrderBy(r => (kvp.Key + " descending"));
                            break;
                    }

                }

            }

            if (criteria.Page.HasValue) {

                query = query.Skip((criteria.Page.Value - 1) * criteria.PageSize.GetValueOrDefault());

            }

            if (criteria.PageSize.HasValue) {

                query = query.Take(criteria.PageSize.Value);

            }

            return new Paged<T>(
                criteria.Page ?? 0,
                criteria.PageSize ?? totalRecords,
                totalRecords,
                query
            );

        }

        /// <summary>
        /// Populate the database with a list of entities.
        /// </summary>
        /// <param name="entites">The entities to use.</param>
        ///
        public virtual void Populate(IList<T> entites) {

            foreach (var entity in entites) {

                this.DbSet.Add(entity);

            }

        }

        /// <summary>
        /// Count the number of items.
        /// </summary>
        /// <param name="predicate">The filter to use for the count.</param>
        /// <returns>The number of items found.</returns>
        ///
        public virtual Int32 Count(Expression<Func<T, bool>> predicate = null) {

            Int32 count = 0;

            if (predicate == null) {

                count = this.DbSet.Count();

            } else {

                count = this.DbSet.Count(predicate);

            }

            return count;

        }

    }

}
