using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace XAS.Model.Paging {
    /// <summary>
    /// A class to process a request with paging.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    public abstract class Criteria<T>: ICriteria<T> {

        /// <summary>
        /// Get/Set the current page.
        /// </summary>
        /// 
        public int? Page { get; set; }

        /// <summary>
        /// Get/Set the page size.
        /// </summary>
        /// 
        public int? PageSize { get; set; }

        /// <summary>
        /// Get/Set the sortby fields.
        /// </summary>
        /// 
        public List<String> SortBy { get; set; }

        /// <summary>
        /// Get/Set the sort direction.
        /// </summary>
        /// 
        public IDictionary<String, ListSortDirection> SortDir { get; set; }

    }

}
