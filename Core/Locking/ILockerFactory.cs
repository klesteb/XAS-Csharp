using System;

namespace XAS.Core.Locking {

    /// <summary>
    /// An interface to the Locker factory.
    /// </summary>
    /// 
    public interface ILockerFactory {

        /// <summary>
        /// Create a locker.
        /// </summary>
        /// <param name="type">The LockType of the locker.</param>
        /// <returns>An initialized locker.</returns>
        /// 
        ILocker Create(LockDriver type);

    }

}
