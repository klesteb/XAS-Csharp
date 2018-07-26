using System;

namespace XAS.Model.Paging {

    public abstract class PagedCriteria: IPagedCriteria {

        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public String[] SortBy { get; set; }
        public String SortDir { get; set; }

    }

}
