using System;
using System.Collections.Generic;

namespace XAS.Core.Security {

    public interface IAuthenticate {

        List<String> Groups { get; set; }

        Boolean IsAuthenticated(String domain, String username, String password);

    }

}
