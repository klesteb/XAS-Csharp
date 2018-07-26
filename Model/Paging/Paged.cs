using System.Collections.Generic;

namespace XAS.Model.Paging {

    /// <summary>
    /// A class to display paged data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    public class Paged<T>: IPaged<T> {

        /// <summary>
        /// Get the page size.
        /// </summary>
        /// 
        public long PageSize { get; private set; }

        /// <summary>
        /// Get the page number.
        /// </summary>
        /// 
        public long PageNumber { get; private set; }

        /// <summary>
        /// Get the total rows.
        /// </summary>
        /// 
        public long TotalResults { get; private set; }

        /// <summary>
        /// Get the data to display.
        /// </summary>
        /// 
        public IEnumerable<T> Data { get; private set; }

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="number">The page number.</param>
        /// <param name="size">The page size.</param>
        /// <param name="total">The total possible items.</param>
        /// <param name="data">The data to display.</param>
        /// 
        public Paged(long number, long size, long total, IEnumerable<T> data) {

            PageSize = size;
            PageNumber = number;
            TotalResults = total;
            Data = data;

        }

    }

}
