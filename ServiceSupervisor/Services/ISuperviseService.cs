using System;
using System.Collections.Generic;

using XAS.Model.Paging;
using ServiceSupervisorCommon.DataStructures;

namespace ServiceSupervisor.Services {

    public interface ISuperviseService {

        List<SuperviseDTO> List();
        Boolean Stop(String name);
        Boolean Start(String name);
        Boolean Delete(String name);
        SuperviseDTO Get(String name);
        SuperviseDTO Create(SupervisePost binding);
        SuperviseDTO Update(String name, SuperviseUpdate binding);
        IPagedList<SuperviseDTO> Paged(SupervisedPagedCriteria criteria);

    }

}
