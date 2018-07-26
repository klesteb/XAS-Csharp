using System.Collections.Generic;

namespace XAS.Model.Paging {

    /// <summary>
    /// Public interface to a paged response.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    public interface IPaged<T> {

        long PageNumber { get; }
        long PageSize { get; }
        long TotalResults { get; }
        IEnumerable<T> Data { get; }

    }

}
