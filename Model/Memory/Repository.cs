using System;
using System.Linq;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections.Concurrent;

using XAS.Model.Paging;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace XAS.Model.Memory {

    /// <summary>
    /// A Generic in memory database Repository, a class that implements a Repository Pattern.
    /// </summary>
    /// 
    public class Repository<T>: IRepository<T> where T : class, new() {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly ConcurrentBag<T> database = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="handler"></param>
        /// <param name="logFactory"></param>
        /// 
        public Repository(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;
            this.log = logFactory.Create(this.GetType());

            this.database = new ConcurrentBag<T>();

        }

        /// <summary>
        /// Find a record in the database.
        /// </summary>
        /// <param name="predicate">A set of criteria used to find the record.</param>
        /// <returns>The record.</returns>
        /// <remarks>
        /// string name = "me";
        /// var result = Find(x => (x.Name == name));
        /// </remarks>
        /// 
        public T Find(Expression<Func<T, bool>> predicate) {

            var query = database.AsQueryable();
            var result = query.Where(predicate).SingleOrDefault();

            return result;

        }

        /// <summary>
        /// Search for records in the database.
        /// </summary>
        /// <param name="predicate">A set of criteria used to find the records.</param>
        /// <returns>A list of records.</returns>
        /// <remarks>
        /// string name = "me";
        /// var results = Search(x => (x.Name == name));
        /// </remarks>
        /// 
        public IEnumerable<T> Search(Expression<Func<T, bool>> predicate = null) {

            var query = database.AsQueryable();

            if (predicate != null) {

                query = query.Where(predicate);

            }

            return query.ToList();

        }

        /// <summary>
        /// Create a record in the databae.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// 
        public void Create(T record) {

            database.Add(record);

        }

        /// <summary>
        /// Update a record in the databse.
        /// </summary>
        /// <param name="record">The record to update.</param>
        /// 
        public void Update(T record) {

            T data = null;
            var query = database.AsQueryable();

            if (query.Contains(record)) {

                if (database.TryTake(out data)) {

                    database.Add(record);

                }

            }

        }

        /// <summary>
        /// Delete a record from the database.
        /// </summary>
        /// <param name="name">The name to use.</param>
        /// 
        public void Delete(object entity) {

            T data = null;
            var query = database.AsQueryable();

            if (query.Contains(entity)) {

                database.TryTake(out data);

            }

        }

        /// <summary>
        /// Populate the database with a list of entities.
        /// </summary>
        /// <param name="entites">The entities to use.</param>
        ///
        public void Populate(IList<T> entites) {

            foreach (var entity in entites) {

                database.Add(entity);

            }

        }

        /// <summary>
        /// Count the number of items.
        /// </summary>
        /// <param name="predicate">The filter to use for the count.</param>
        /// <returns>The number of items found.</returns>
        ///
        public Int32 Count(Expression<Func<T, bool>> predicate = null) {

            Int32 count = 0;

            if (predicate == null) {

                count = database.Count();

            } else {

                var query = database.AsQueryable();
                count = query.Count(predicate);

            }

            return count;

        }

        /// <summary>
        /// Page thru the results of a database search.
        /// </summary>
        /// <param name="criteria">Paging criteria.</param>
        /// <returns>A paged listing.</returns>
        /// 
        public IPaged<T> Page(ICriteria<T> criteria) {

            var query = database.AsQueryable();
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

    }

}
