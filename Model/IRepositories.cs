using System;

namespace XAS.Model {

    /// <summary>
    /// Interface for repositories.
    /// </summary>
    /// 
    public interface IRepositories: IDisposable {

        void Save();
        void DoTransaction(Action method);

    }

}
