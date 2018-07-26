using System;

namespace XAS.Core.Locking {

    public abstract class Locker: ILocker {

        public abstract String Lockname { get; set; }
        public abstract Boolean TryLock();
        public abstract Boolean Lock();
        public abstract Boolean Unlock();

    }

}
