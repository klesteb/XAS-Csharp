using System;

namespace XAS.Model.Paging {

    public interface IPagedCriteria {

        int? Page { get; set; }
        int? PageSize { get; set; }
        String[] SortBy { get; set; }
        String SortDir { get; set; }

    }

}
