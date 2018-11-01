using System;
using System.Linq.Expressions;
using System.Collections.Generic;

using XAS.Model.Paging;

namespace XAS.Model {

    // Repository Pattern
    // taken from: http://www.tugberkugurlu.com/archive/generic-repository-pattern-entity-framework-asp-net-mvc-and-unit-testing-triangle
    // and from: http://www.janholinka.net/Blog/Article/9 
    // with modifications

    /// <summary>
    /// Interface IRepository. Implements the Generic Repository.
    /// </summary>
    /// <remarks>
    /// This interface defines the minimal interface for a repository. 
    /// 
    /// </remarks>
    /// <typeparam name="T">A DbContext object.</typeparam>
    /// 
    public interface IRespository<T> where T: class {

        /// <summary>
        /// A generic search method. A null predicate returns all entities.
        /// </summary>
        /// <param name="predicate">The filter to use with the search.</param>
        /// <returns>A list of entities.</returns>
        /// 
        IEnumerable<T> Search(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// A generic find method.
        /// </summary>
        /// <param name="id">The id of the wanted entity.</param>
        /// <returns>A single entity.</returns>
        /// 
        T Find(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// A generic create method.
        /// </summary>
        /// <param name="entity">Create a new entity.</param>
        /// 
        void Create(T entity);

        /// <summary>
        /// A generic update method.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// 
        void Update(T entity);

        /// <summary>
        /// A generic delete method.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// 
        void Delete(object id);

        /// <summary>
        /// Page thru the database.
        /// </summary>
        /// <param name="criteria">The selection criteria.</param>
        /// <returns>A paged list of entities.</returns>
        /// 
        IPaged<T> Page(ICriteria<T> criteria);

        /// <summary>
        /// A generic populate method. Create multiple entities at once. 
        /// </summary>
        /// <param name="entities">A list of entities.</param>
        /// 
        void Populate(IList<T> entities);

        /// <summary>
        /// A generic count method. 
        /// </summary>
        /// <param name="predicate">The filter to use for the count.</param>
        /// <returns>The number of items found.</returns>
        /// 
        Int32 Count(Expression<Func<T, bool>> predicate = null);

    }

}
