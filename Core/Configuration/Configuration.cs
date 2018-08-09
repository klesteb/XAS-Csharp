using System;
using System.Linq;
using System.Collections.Generic;


namespace XAS.Core.Configuration {

    public class KeyValues {
        public String Key { get; set; }
        public String Value { get; set; }
    }

    public class Configuration: IConfiguration {

        private Object _critical = null;
        private Dictionary<String, List<KeyValues>> contents = null;
        
        /// <summary>
        /// The defined keys within sections.
        /// </summary>
        /// 
        public Key Key { get; set; }

        /// <summary>
        /// The defined sections.
        /// </summary>
        /// 
        public Section Section { get; set; }

        /// <summary>
        /// Contructor.
        /// </summary>
        /// 
        public Configuration(Section section, Key key) {

            _critical = new Object();
            contents = new Dictionary<string, List<KeyValues>>();

            this.Key = key;
            this.Section = section;

        }

        /// <summary>
        /// Create a new section.
        /// </summary>
        /// <param name="section">The sections name.</param>
        /// <returns>true if successful.</returns>
        /// 
        public Boolean CreateSection(String section) {

            bool stat = false;

            var keyValues = new List<KeyValues>();
            stat = AddSection(section, keyValues);

            return stat;

        }
        
        /// <summary>
        /// Add a new section.
        /// </summary>
        /// <param name="section">The name of the section.</param>
        /// <param name="keyValues">A list of KeyValues objects.</param>
        /// <returns>true when successful</returns>
        /// 
        public Boolean AddSection(String section, List<KeyValues> keyValues) {

            bool stat = false;

            lock (_critical) {

                if (!contents.ContainsKey(section)) {

                    contents.Add(section, keyValues);
                    stat = true;

                }

            }

            return stat;

        }

        /// <summary>
        /// Delete an existing section.
        /// </summary>
        /// <param name="section">The name of the section.</param>
        /// <returns>true when sucessful.</returns>
        /// 
        public Boolean DeleteSection(String section) {

            bool stat = false;

            lock (_critical) {

                if (contents.ContainsKey(section)) {

                    contents.Remove(section);
                    stat = true;

                }

            }

            return stat;

        }

        /// <summary>
        /// Merge a list of KeyValues objects into a section,
        /// </summary>
        /// <param name="section">The section name.</param>
        /// <param name="keyValues">A list of KeyValues objects.</param>
        /// <returns>true if successful.</returns>
        /// <remarks>
        /// If the section doesn't exist, this method will create a new section with the associated KeyValues objects.
        /// </remarks>
        /// 
        public Boolean MergeSection(String section, List<KeyValues> keyValues) {

            bool stat = false;

            lock (_critical) {

                if (contents.ContainsKey(section)) {

                    var values = contents[section];
                    var list = MergeLists(keyValues, values);

                    contents[section] = list;
                    stat = true;

                } else {

                    contents.Add(section, keyValues);
                    stat = true;

                }

            }

            return stat;

        }

        /// <summary>
        /// Checks to see if a section exists.
        /// </summary>
        /// <param name="section">The section name.</param>
        /// <returns>true if it exists.</returns>
        /// 
        public Boolean DoesSectionExist(String section) {

            bool stat = false;

            lock (_critical) {

                if (contents.ContainsKey(section)) {

                    stat = true;

                }

            }

            return stat;

        }

        /// <summary>
        /// Get a list of sections.
        /// </summary>
        /// <returns>A list of section names.</returns>
        /// 
        public List<String> GetSections() {

            var sections = new List<String>();

            lock (_critical) {

                foreach (var section in contents) {

                    sections.Add(section.Key);

                }

            }

            return sections;

        }
        
        /// <summary>
        /// Get a list of keys in a section.
        /// </summary>
        /// <param name="section">The sections name.</param>
        /// <returns>A list of section keys.</returns>
        /// 
        public List<String> GetSectionKeys(String section) {

            var keys = new List<String>();

            lock (_critical) {

                if (contents.ContainsKey(section)) {

                    var values = contents[section];
                    foreach (var value in values) {

                        keys.Add(value.Key);

                    }

                }

            }

            return keys;

        }
        
        /// <summary>
        /// Add a key to a section.
        /// </summary>
        /// <param name="section">The sections name.</param>
        /// <param name="key">The key name.</param>
        /// <param name="value">The value for the key.</param>
        /// <returns>true if successful.</returns>
        /// 
        public Boolean AddKey(String section, String key, String value) {

            bool stat = false;

            lock (_critical) {

                if (contents.ContainsKey(section)) {

                    var values = contents[section];
                    var found = values.SingleOrDefault(i => (i.Key == key));

                    if (found == null) {

                        var keyValues = new KeyValues {
                            Key = key,
                            Value = value
                        };

                        values.Add(keyValues);
                        contents[section] = values;
                        stat = true;

                    }

                }

            }

            return stat;

        }

        /// <summary>
        /// Delete a key from a section.
        /// </summary>
        /// <param name="section">The sections name.</param>
        /// <param name="key">The name of the key.</param>
        /// <returns>true if successful.</returns>
        /// 
        public Boolean DeleteKey(String section, String key) {

            bool stat = false;

            lock (_critical) {

                if (contents.ContainsKey(section)) {

                    var values = contents[section];
                    var found = values.SingleOrDefault(i => (i.Key == key));

                    if (found != null) {

                        values.Remove(found);
                        contents[section] = values;
                        stat = true;

                    }

                }

            }

            return stat;

        }

        /// <summary>
        /// Update the value of a key in a section.
        /// </summary>
        /// <param name="section">The sections name.</param>
        /// <param name="key">The keys name.</param>
        /// <param name="value">The new value.</param>
        /// <returns>true if successful.</returns>
        /// 
        public Boolean UpdateKey(String section, String key, String value) {

            bool stat = true;

            lock (_critical) {

                if (contents.ContainsKey(section)) {

                    var values = contents[section];
                    var found = values.SingleOrDefault(i => (i.Key == key));

                    if (found != null) {

                        values.Remove(found);
                        found.Value = value;
                        values.Add(found);

                        contents[section] = values;
                        stat = true;

                    }

                } else {

                    var values = contents[section];
                    var keyValues = new KeyValues {
                        Key = key,
                        Value = value
                    };

                    values.Add(keyValues);
                    contents[section] = values;

                }

            }

            return stat;

        }

        /// <summary>
        /// Get the value of a key in a section.
        /// </summary>
        /// <param name="section">The sections name.</param>
        /// <param name="key">The keys name.</param>
        /// <returns>The value of the key.</returns>
        /// 
        public String GetValue(String section, String key) {

            string value = "";

            lock (_critical) {

                if (contents.ContainsKey(section)) {

                    var values = contents[section];
                    var found = values.SingleOrDefault(i => (i.Key == key));

                    if (found != null) {

                        value = found.Value;

                    }

                }

            }

            return value;

        }

        /// <summary>
        /// Get the value of a key in an section, with a default value.
        /// </summary>
        /// <param name="section">The sections name.</param>
        /// <param name="key">The keys name.</param>
        /// <param name="xdefault">A default value.</param>
        /// <returns>The value of the key.</returns>
        /// <remarks>
        /// The default value is used if the key is not found.
        /// </remarks>
        /// 
        public String GetValue(String section, String key, String xdefault) {

            string value = xdefault;

            lock (_critical) {

                if (contents.ContainsKey(section)) {

                    var values = contents[section];
                    var found = values.SingleOrDefault(i => (i.Key == key));

                    if (found != null) {

                        value = found.Value;

                    }

                }

            }

            return value;

        }

        /// <summary>
        /// Dump the configuration contents to stdout.
        /// </summary>
        /// 
        public void Dump() {

            System.Console.WriteLine("{0}", Utils.Dump(contents));

        }

        #region Private Methods

        private List<KeyValues> MergeLists(List<KeyValues> list1, List<KeyValues> list2) {

            var list = list1.Concat(list2)
                .ToLookup(p => p.Key)
                .Select(g => g.Aggregate((p1, p2) => new KeyValues {
                    Key = p1.Key,
                    Value = p1.Value
                })).ToList();

            return list;

        }

        #endregion

    }

}
