using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace XAS.Model.Paging {

    public interface IPagedList<T> {

        long PageNumber { get; }
        long PageSize { get; }
        long TotalResults { get; }
        long TotalPages { get; }
        String[] SortedBy { get; }
        IDictionary<string, ListSortDirection> SortedDir { get; }
        IEnumerable<T> Data { get; }

    }

}

