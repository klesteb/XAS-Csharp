using System;
using System.Text;
using System.DirectoryServices;
using System.Collections.Generic;


namespace XAS.Core.Security {

    // taken from: https://support.microsoft.com/en-us/help/316748/how-to-authenticate-against-the-active-directory-by-using-forms-authen
    // with modifications

    /// <summary>
    /// Use LDAP to authenticate against AD.
    /// </summary>
    /// 
    public class Authenticate: IAuthenticate {

        /// <summary>
        /// Get the groups associated with the authenticated user.
        /// </summary>
        /// 
        public List<string> Groups { get; set; }

        /// <summary>
        /// Contructor, uses defaults.
        /// </summary>
        /// 
        public Authenticate() {

            this.Groups = new List<String>();

        }

        /// <summary>
        /// Checks to see if username and password are authenticatable.
        /// </summary>
        /// <param name="username">The username to use.</param>
        /// <param name="password">The password to use.</param>
        /// <returns>Return true if authenticated.</returns>
        ///      
        public Boolean IsAuthenticated(String domain, String username, String password) {

            bool stat = false;
            string path = String.Format("LDAP://{0}", domain.ToUpper());
            DirectoryEntry entry = new DirectoryEntry(path, username, password);

            try {

                // Bind to the native AdsObject to force authentication.
                
                Object obj = entry.NativeObject;

                // retrieve group information

                this.Groups.Clear();
                GetGroups(entry, username);

                stat = true;

            } catch (DirectoryServicesCOMException) {

            }

            return stat;

        }

        private void GetGroups(DirectoryEntry entry, string username) {

            string path;
            string filterAttribute;
            DirectorySearcher search = new DirectorySearcher(entry);

            search.Filter = String.Format("(SAMAccountName={0})", username);
            search.PropertiesToLoad.Add("cn");
            SearchResult result = search.FindOne();

            if (result != null) {

                path = result.Path;
                filterAttribute = (String)result.Properties["cn"][0];

                search = new DirectorySearcher(path);
                search.Filter = String.Format("(cn={0})", filterAttribute);
                search.PropertiesToLoad.Add("memberOf");
                StringBuilder groupNames = new StringBuilder();

                result = search.FindOne();

                int propertyCount = result.Properties["memberOf"].Count;
                String dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++) {

                    dn = (String)result.Properties["memberOf"][propertyCounter];

                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);

                    if (equalsIndex != -1) {

                        this.Groups.Add(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));

                    }

                }

            }

        }

    }

}
