using System;
using System.Collections.Generic;

using XAS.Model.Paging;
using ServiceSupervisorCommon.DataStructures;

namespace ServiceSupervisor.Web.Services {

    public interface ISupervised {

        SuperviseDTO Get(String name);
        SuperviseDTO Create(SupervisePost binding);
        SuperviseDTO Update(String name, SuperviseUpdate binding);
        Boolean Delete(String name);
        List<SuperviseDTO> List();
        IPagedList<SuperviseDTO> Paged(Model.Services.Supervised.SupervisedPagedCriteria criteria);

    }

}
