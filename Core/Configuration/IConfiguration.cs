using System;
using System.Collections.Generic;

namespace XAS.Core.Configuration {

    /// <summary>
    /// Public interface to manage a configuration instance.
    /// </summary>
    /// 
    public interface IConfiguration {

        Key Key { get; set; }
        Section Section { get; set; }

        List<String> GetSections();
        Boolean DeleteSection(String section);
        Boolean CreateSection(String section);
        Boolean DoesSectionExist(String section);
        List<String> GetSectionKeys(String section);
        String GetValue(String section, String key);
        Boolean DeleteKey(String section, String key);
        Boolean AddKey(String section, String key, String value);
        Boolean UpdateKey(String section, String key, String value);
        String GetValue(String section, String key, String xdefault);
        Boolean AddSection(String section, List<KeyValues> keyValues);
        Boolean MergeSection(String section, List<KeyValues> keyValues);
        void Dump();

    }

}
