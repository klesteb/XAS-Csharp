using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace XAS.Model.Paging {

    public class PagedList<T>: IPagedList<T> {

        public long PageNumber { get; private set; }
        public long PageSize { get; private set; }
        public long TotalResults { get; private set; }
        public IEnumerable<T> Data { get; private set; }
        public String[] SortedBy { get; private set; }
        public IDictionary<string, ListSortDirection> SortedDir { get; private set; }

        public long TotalPages => TotalResults % PageSize == 0 ? TotalResults / PageSize : (TotalResults / PageSize) + 1;

        public PagedList(long pageNumber, long pageSize, long totalResults, string[] sortedBy, IDictionary<string, ListSortDirection> sortedDir, IEnumerable<T> data) {

            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalResults = totalResults;
            SortedBy = sortedBy;
            SortedDir = sortedDir;
            Data = data;

        }

    }

}
