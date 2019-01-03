using System;
using System.Text;
using System.Collections.Generic;

namespace XAS.Network.STOMP {

    /// <summary>
    /// Create a STOMP frame.
    /// </summary>
    /// <remarks>
    /// 
    /// A STOMP v1.0 frame consists of the following:
    ///
    ///    COMMAND\n
    ///    HEADERS -> key: value\n
    ///    \n
    ///    BODY\0
    ///     
    /// Where \n is platform specific and all commands and headers are implied ASCII.
    /// 
    /// A STOMP v1.1 frame consists of the following:
    /// 
    ///    COMMAND\n
    ///    HEADERS -> key: value\n
    ///    \n
    ///    BODY\0
    ///     
    /// The command and headers are now UTF8 and the following characters may be found in the 
    /// headers, and need to be "escaped".
    /// 
    ///     \n - octets (92 and 110) translate to \n (octet 10)
    ///     \c - octets (92 and 99) translates to : (octet 58)
    ///     \\ - octext (92 and 92) tranlates to \ (octet 92)
    /// 
    /// A frame type of:
    /// 
    ///     \n\n\n\0
    ///     
    /// Is now considered a "heartbeat" with special heartbeat handling required.
    /// 
    /// A STOMP v1.2 frame consists of the following, where the \r is optional:
    /// 
    ///    COMMAND\r\n
    ///    HEADERS -> key: value\r\n
    ///    \r\n
    ///    BODY\0
    /// 
    /// Of course the "keepalive" frame is now:
    /// 
    ///     \r\n\r\n\r\n\0
    /// 
    /// And the following escape has been added:
    /// 
    ///     \r - (octets 92 and 114) translates to carriage return (octect 13)
    ///
    /// End of frame has always been \0.
    /// 
    /// Padding in headers
    /// 
    /// v1.0 and v1.1 is unclear about spaces between headers and values
    /// nor the case of the header. By convention, the command is always
    /// uppercase.                                                   
    ///                                                                  
    /// v1.2 says there should be no "padding" in headers and values.
    /// It now appears the headers and values should be lowercase.                                     
    /// 
    /// As of 2019-01-03
    /// 
    /// It appears the Rabbit MQ (3.1.5) is sending a single LF(10) for the
    /// keepalive frame. And the github pages show that is now the correct
    /// way of doing a keepalive. I didn't see anything on the stomp
    /// mailing list about this change.
    /// 
    /// </remarks>

    public class Frame {

        /// <summary>
        /// Gets/Sets the internal buffer.
        /// </summary>
        /// 
        public Byte[] Body { get; set; }

        /// <summary>
        /// Gets/Sets the STOMP version level.
        /// </summary>
        /// 
        public Single Level { get; set; }

        /// <summary>
        /// Gets/Sets the STOMP command.
        /// </summary>
        /// 
        public String Command { get; set; }

        /// <summary>
        /// Gets the STOMP eol.
        /// </summary>
        /// 
        public String eol { get; private set; }

        /// <summary>
        /// Gets the STOMP eof.
        /// </summary>
        /// 
        public String eof { get; private set; }

        /// <summary>
        /// Gets/Sets the STOMP headers.
        /// </summary>
        /// 
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Frame(): this("", null, null) { }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="command">The STOMP command.</param>
        /// <param name="headers">STOMP headers.</param>
        /// 
        public Frame(String command, Dictionary<string,string> headers): this(command, headers, null) { }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="command">The STOMP command.</param>
        /// <param name="headers">STOMP headers.</param>
        /// <param name="body">Message body.</param>
        /// <param name="level">STOMP version level.</param>
        /// 
        public Frame(String command, Dictionary<string,string> headers, Byte[] body, String level = "1.0") {

            this.eol = "\n";
            this.eof = "\0";
            this.Level = Convert.ToSingle(level);

            if (this.Level > 1.1) {

                this.eol = "\r\n";
            }

            this.Body = body;
            this.Command = command;
            this.Headers = headers;

        }

        /// <summary>
        /// Convert a STOMP frame to a byte array.
        /// </summary>
        /// <returns>A byte array.</returns>
        /// 
        public Byte[] ToArray() {

            var buffer = new List<byte>();
            string command = this.Command;
            Dictionary<string,string> headers = this.Headers;
            byte[] eol = System.Text.Encoding.ASCII.GetBytes(this.eol);
            byte[] eof = System.Text.Encoding.ASCII.GetBytes(this.eof);

            if (command != "") {

                buffer.AddRange(Stomp.ConvertToBytes(command.ToUpper(), this.Level.ToString()));

            }

            buffer.AddRange(eol);

            if (headers != null) {

                foreach (KeyValuePair<string, string> item in headers) {

                    string header;
                    string key = item.Key.ToLower();
                    string value = item.Value;

                    header = this.CreateHeaderString(key, value);

                    buffer.AddRange(Stomp.ConvertToBytes(header, this.Level.ToString()));
                    buffer.AddRange(eol);

                }

            }

            buffer.AddRange(eol);

            if (this.Body != null) {

                buffer.AddRange(this.Body);

            }

            buffer.AddRange(eof);

            return buffer.ToArray();

        }

        /// <summary>
        /// Convert a STOMP frame to a string.
        /// </summary>
        /// <returns>A string.</returns>
        /// 
        public override String ToString() {

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}{1}", this.Command, this.eol);

            foreach (KeyValuePair<string, string> item in this.Headers) {

                sb.AppendFormat("{0}{1}", this.CreateHeaderString(item.Key, item.Value), this.eol);

            }

            sb.AppendFormat("{0}", this.eol);

            if (this.Body != null) {

                sb.AppendFormat("{0}", Stomp.ConvertToString(this.Body, this.Level.ToString()));

            }

            sb.AppendFormat("{0}", this.eof);

            return sb.ToString();

        }

        #region Private Methods

        private string CreateHeaderString(String key, String value) {

            String header;

            if (this.Level > 1.1) {

                Stomp.Escape(ref key);
                Stomp.Escape(ref value);

                header = String.Format("{0}:{1}", key.ToLower(), value);

            } else {

                header = String.Format("{0}: {1}", key.ToLower(), value);
            }

            return header;

        }

        #endregion

    }

}
