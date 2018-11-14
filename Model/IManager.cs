using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAS.Model {

    /// <summary>
    /// Interface for a repository manager.
    /// </summary>
    /// 
    public interface IManager {

        IRepositories Repository { get; set; }

    }

}
