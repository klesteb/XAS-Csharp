using System;

namespace ServiceSupervisor.Supervisor {

    /// <summary>
    /// Interface for supervisors.
    /// </summary>
    /// 
    public interface ISupervisor {

        void Start();
        void Stop();
        void Pause();
        void Continue();
        void Shutdown();

    }

}
