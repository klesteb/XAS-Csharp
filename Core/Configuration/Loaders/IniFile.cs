using System;
using System.IO;
using System.Collections.Generic;

namespace XAS.Core.Configuration.Loaders {

    /// <summary>
    /// A class for handling Windows .ini configuration files.
    /// </summary>
    /// 
    public class IniFile {

        private List<string> contents = null;

        /// <summary>
        /// Used for critical sections.
        /// </summary>
        /// 
        protected object _critical = null;

        /// <summary>
        /// Gets/Sets the filename.
        /// </summary>
        /// 
        public String Filename { get; set; }

        /// <summary>
        /// Gets/Sets the comment character.
        /// </summary>
        /// 
        public String CommentCharacter { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public IniFile(String filename) {

            _critical = new object();
            contents = new List<string>();

            Filename = filename;
            CommentCharacter = ";";
            
        }

        /// <summary>
        /// Retrieve a value from a configuration section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value for the key.</returns>
        /// 
        public String GetValue(String section, String key) {

            int end = 0;
            int start = 0;
            int offset = 0;
            string xvalue = "";
            string xdefault = "";

            lock (_critical) {

                if (FindSection(section, ref start)) {

                    end = NextSection(start + 1);

                    if ((offset = FindKey(start, end, key, ref xvalue)) > 0) {

                        return xvalue;

                    }

                }

            }

            return xdefault;

        }

        /// <summary>
        /// Retrieve a value from a configuration section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <param name="key">The key.</param>
        /// <param name="xdefault">A defaulf value if key doesn't exist.</param>
        /// <returns>The value for the key.</returns>
        /// 
        public String GetValue(String section, String key, String xdefault) {

            int end = 0;
            int start = 0;
            int offset = 0;
            string xvalue = "";

            lock (_critical) {

                if (FindSection(section, ref start)) {

                    end = NextSection(start + 1);

                    if ((offset = FindKey(start, end, key, ref xvalue)) > 0) {

                        return xvalue;

                    }

                }

            }

            return xdefault;

        }

        /// <summary>
        /// Delete a key from a section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <param name="key">The key.</param>
        /// <returns>True if successful.</returns>
        /// 
        public Boolean DeleteKey(String section, String key) {

            int end = 0;
            int start = 0;
            int offset = 0;
            string value = "";
            bool stat = false;

            lock (_critical) {

                if (FindSection(section, ref start)) {

                    end = NextSection(start + 1);

                    if ((offset = FindKey(start, end, key, ref value)) > 0) {

                        contents.RemoveAt(offset);

                        stat = true;

                    }

                }

            }

            return stat;
                        
        }

        /// <summary>
        /// Add a key, value pair to a section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <param name="key">The key.</param>
        /// <param name="xvalue">The value.</param>
        /// <returns>True if successful.</returns>
        /// 
        public Boolean AddKey(String section, String key, String xvalue) {

            int offset = 0;
            bool stat = false;
            string format = "{0}={1}";

            lock (_critical) {

                if (FindSection(section, ref offset)) {

                    offset += 1;
                    contents.Insert(offset, String.Format(format, key, xvalue));

                    stat = true;

                }

            }

            return stat;

        }

        /// <summary>
        /// Set the value of a key within a section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <param name="key">The key.</param>
        /// <param name="xvalue">The value.</param>
        /// <returns>True if successful.</returns>
        /// 
        public Boolean SetKey(String section, String key, String xvalue) {

            int end = 0;
            int start = 0;
            int offset = 0;
            string value = "";
            bool stat = false;
            string format = "{0}={1}";

            lock (_critical) {

                if (FindSection(section, ref start)) {

                    end = NextSection(start + 1);

                    if ((offset = FindKey(start, end, key, ref value)) > 0) {

                        contents.RemoveAt(offset);
                        contents.Insert(offset, String.Format(format, key, xvalue));

                        stat = true;

                    }

                }

            }

            return stat;

        }

        // Section methods

        /// <summary>
        /// Retrieve the sections.
        /// </summary>
        /// <returns>An arrary of Strings.</returns>
        /// 
        public String[] GetSections() {

            string section = "";
            List<string> sections = new List<string>();

            lock (_critical) {

                for (int i = 0; i < contents.Count; i++) {

                    string line = contents[i];

                    if (ParseSectionName(line, ref section)) {

                        sections.Add(section);

                    }

                }

            }

            return sections.ToArray();

        }

        /// <summary>
        /// Add a section.
        /// </summary>
        /// <param name="section">The section name.</param>
        /// 
        public void AddSection(String section) {

            lock (_critical) {

                contents.Add(String.Format("[{0}]", section));

            }

        }

        /// <summary>
        /// Delete a section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <returns>True if successful.</returns>
        /// 
        public Boolean DeleteSection(String section) {

            int end = 0;
            int start = 0;
            bool stat = false;

            lock (_critical) {

                if (FindSection(section, ref start)) {

                    end = NextSection(start + 1);
                    contents.RemoveRange(start, (end - start) + 1);

                    stat = true;

                }

            }
    
            return stat;

        }

        /// <summary>
        /// Check if a section exists.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <returns>True if section exists.</returns>
        /// 
        public Boolean DoesSectionExist(String section) {

            int start = 0;
            bool stat = false;

            lock (_critical) {

                if (FindSection(section, ref start)) {

                    stat = true;

                }

            }

            return stat;

        }

        /// <summary>
        /// Retrieve the keys from a section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <returns>An array of Strings.</returns>
        /// 
        public String[] GetSectionKeys(String section) {

            int end = 0;
            int start = 0;
            string key = "";
            string value = "";
            List<string> keys = new List<string>();

            lock (_critical) {

                if (FindSection(section, ref start)) {

                    end = NextSection(start + 1);

                    for (int i = start; i <= end; i++) {

                        string line = contents[i];

                        if (ParseKeyValue(line, ref key, ref value)) {

                            keys.Add(key);

                        }

                    }

                }

            }

            return keys.ToArray();

        }

        // file manipulations

        /// <summary>
        /// Load the file.
        /// </summary>
        /// 
        public IniFile Load() {

            lock (_critical) {

                string[] buffer = File.ReadAllLines(this.Filename);

                contents.Clear();

                foreach (string line in buffer) {

                    contents.Add(line.Trim());

                }

            }

            return this;

        }

        /// <summary>
        /// Reload the file.
        /// </summary>
        /// 
        public void ReLoad() {

            lock (_critical) {

                string[] buffer = File.ReadAllLines(this.Filename);

                contents.Clear();

                foreach (string line in buffer) {

                    contents.Add(line.Trim());

                }

            }

        }

        /// <summary>
        /// Save the configuration.
        /// </summary>
        /// 
        public void Save() {

            lock (_critical) {

                StreamWriter sw = new StreamWriter(this.Filename);

                for (int i = 0; i <  contents.Count; i++) {

                    string line = contents[i];

                    sw.WriteLine(line);

                }

                sw.Close();
                            
            }

        }

        /// <summary>
        /// Save the configuration to differant file.
        /// </summary>
        /// <param name="fileName">New file name.</param>
        /// 
        public void SaveAs(String fileName) {

            lock (_critical) {

                StreamWriter sw = new StreamWriter(fileName);

                for (int i = 0; i < contents.Count; i++) {

                    string line = contents[i];

                    sw.WriteLine(line);

                }

                sw.Close();

            }

        }

        // comment methods

        /// <summary>
        /// Add a comment to a key within a section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <param name="key">The key.</param>
        /// <param name="comment">The comment.</param>
        /// 
        public void AddKeyComment(String section, String key, String comment) {

            throw new System.NotImplementedException();

        }

        /// <summary>
        /// Set an existing comment of a key within a section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <param name="key">The key.</param>
        /// <param name="comment">The comment.</param>
        /// 
        public void SetKeyComment(String section, String key, String comment) {

            throw new System.NotImplementedException();

        }

        /// <summary>
        /// Delete a comment from a key within a section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <param name="key">The key.</param>
        /// 
        public void DeleteKeyComment(String section, String key) {

            throw new System.NotImplementedException();

        }

        /// <summary>
        /// Add a comment to a section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <param name="comments">The comment.</param>
        /// 
        public void AddSectionComment(String section, String[] comments) {

            throw new System.NotImplementedException();

        }

        /// <summary>
        /// Delete a comment from a section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// 
        public void DeleteSectionComment(String section) {

            throw new System.NotImplementedException();

        }

        /// <summary>
        /// Set an existing comment on section.
        /// </summary>
        /// <param name="section">The section wanted.</param>
        /// <param name="comments">The comment.</param>
        /// 
        public void SetSectionComment(String section, String[] comments) {

            throw new System.NotImplementedException();

        }

        #region Private Methods

        private int FindKey(int start, int end, string key, ref string value) {

            int offset = 0;
            string xkey = "";

            for (int i = start; i <= end; i++) {

                string line = contents[i];

                if (ParseKeyValue(line, ref xkey, ref value)) {

                    if (key == xkey) {

                        offset = i;
                        break;

                    }                    

                }

            }

            return offset;

        }
        
        private bool FindSection(string section, ref int offset) {

            bool stat = false;
            string wanted = null;

            offset = 0;

            for (int i = 0; i < contents.Count; i++) {

                string line = contents[i];

                if (ParseSectionName(line, ref wanted)) {

                    if (wanted == section) {

                        stat = true;
                        offset = i;

                        break;

                    }

                }

            }

            return stat;

        }

        private int NextSection(int start) {

            int count = 0;
            string section = "" ;

            for (int i = start; i < contents.Count; i++) {

                count = i;
                string line = contents[i];

                if (ParseSectionName(line, ref section)) {

                    break;

                }

            }

            return count;

        }

        private bool ParseSectionName(string line, ref string section) {

            // section name = [name]

            int index = 0;
            bool stat = false;

            if (line.StartsWith("[") && ((index = line.IndexOf("]")) > 0)) {

                section = line.Substring(1, index - 1).Trim();
                stat = true;

            }

            return stat;

        }

        private bool ParseKeyValue(string line, ref string key, ref string value) {

            // key value pair = key=value 

            int index = 0;
            int offset = 0;
            bool stat = false;

            if ((index = line.IndexOf("=")) > 0) {

                offset = line.IndexOf(this.CommentCharacter);

                if (offset > 0) {

                    key = line.Substring(0, index).Trim();
                    value = line.Substring(index + 1, offset - (index + 1)).Trim();
                    stat = true;

                } else if (offset < 0) {

                    offset = line.Length;
                    key = line.Substring(0, index).Trim();
                    value = line.Substring(index + 1, offset - (index + 1)).Trim();
                    stat = true;

                }

            }

            return stat;
                    
        }

        #endregion

    }

}
