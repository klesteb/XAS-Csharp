using System;
using System.Collections.Generic;

using XAS.Core.Security;

using Nancy.Security;
using Nancy.Authentication.Basic;

namespace XAS.Rest.Server {

    /// <summary>
    /// Validate a user against Active Directory.
    /// </summary>
    /// <remarks>
    /// A rather simple procedure. If the username and password is accepted, load all the
    /// groups that the user is associated with. These can then be used as "Claims".
    /// </remarks>
    /// 
    public class UserValidator: IUserValidator {

        private readonly String domain;
        private readonly IAuthenticate authenticate = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public UserValidator(IAuthenticate authenticate, String domain) {

            this.domain = domain;
            this.authenticate = authenticate;

        }

        /// <summary>
        /// Perform the validation.
        /// </summary>
        /// <param name="username">The username to use.</param>
        /// <param name="password">The password to use.</param>
        /// <returns>A UserIndentiy object or null.</returns>
        /// 
        public IUserIdentity Validate(String username, String password) {

            if (authenticate.IsAuthenticated(domain, username, password)) {

                return new UserIdentity() {
                    UserName = username,
                    Claims = authenticate.Groups
                };

            }

            return null;

        }

    }

    public class UserIdentity: IUserIdentity {
    
        public String UserName { get; set; }    
        public IEnumerable<String> Claims { get;  set; }

    }

}
