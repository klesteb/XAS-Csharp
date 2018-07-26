using System;
using System.ComponentModel.DataAnnotations;

namespace XAS.Model.Database {

    /// <summary>
    /// A base for all table definations.
    /// </summary>
    /// <remarks>
    /// This class implements an auto incremented ID primary key and a RowVersion column for optimistic locking.
    /// </remarks>
    ///
    public class Base {

        /// <summary>
        /// Primary key.
        /// </summary>
        ///
        public Int32 Id { get; set; }

        /// <summary>
        /// Used by EF optimistic locking. 
        /// </summary>
        /// 
        public byte[] Revision { get; set; }

    }

}
