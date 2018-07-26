using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace XAS.Model.Paging {

    /// <summary>
    /// Public interface for selection criteria on database searches.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    public interface ICriteria<T> {

        int? Page { get; set; }
        int? PageSize { get; set; }
        List<String> SortBy { get; set; } 
        IDictionary<String, ListSortDirection> SortDir { get; set; }

    }

}
