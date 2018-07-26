using System;

using XAS.Model.Paging;

using DemoMicroServiceServer.Web.Requests;
using DemoMicroServiceServer.Model.Services;

namespace DemoMicroServiceServer.Web.Services {

    public interface IDinosaurService {

        int DeleteDinosaur(int id);
        DinosaurDTO GetDinosaur(int id);
        DinosaurDTO CreateDinosaur(DinosaurFMI binding);
        DinosaurDTO UpdateDinosaur(Int32 id, DinosaurFMI binding);
        IPagedList<DinosaurDTO> GetPage(DinosaurPagedCriteria criteria);

    }

}
