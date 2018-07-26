using System;


namespace XAS.Core.Locking {

    /// <summary>
    /// A factory class to load lockers.
    /// </summary>
    /// 
    public class Factory: ILockerFactory {

        private readonly Factory<LockDriver, ILocker> factory = null;

        /// <summary>
        /// Default Factory method.
        /// </summary>
        /// <remarks>
        /// Initializing the locker, defaults to the lock name to "lock".
        /// </remarks>
        /// 
        public Factory(String lockName = "locked") {

            this.factory = new Factory<LockDriver, ILocker>();

            factory.Register(LockDriver.Mutex, new Func<ILocker>(() => new Lockers.Mutex(lockName)));
            factory.Register(LockDriver.Default, new Func<ILocker>(() => new Lockers.Default(lockName)));

        }

        /// <summary>
        /// Create a locker.
        /// </summary>
        /// <param name="type">The LockType of the locker.</param>
        /// <returns>An initialized locker.</returns>
        /// 
        public ILocker Create(LockDriver type) {

            return factory.Create(type);

        }

    }

}
