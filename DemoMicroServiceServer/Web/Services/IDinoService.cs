using System;
using System.Collections.Generic;

using XAS.Model.Paging;
using DemoModel.Service;
using DemoModelCommon.DataStructures;

namespace DemoMicroServiceServer.Web.Services {

    public interface IDinoService {

        List<DinosaurDTO> List();
        DinosaurDTO Get(Int32 id);
        Boolean Delete(Int32 id);
        DinosaurDTO Create(DinosaurPost binding);
        DinosaurDTO Update(Int32 id, DinosaurUpdate binding);
        IPagedList<DinosaurDTO> Paged(DinosaursPagedCriteria criteria);

    }

}
