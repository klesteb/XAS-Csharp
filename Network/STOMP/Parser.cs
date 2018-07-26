using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace XAS.Network.STOMP {

    /// <summary>
    /// A STOMP frame parser
    /// </summary>
    /// 
    public class Parser {

        private List<byte> buffer = new List<byte>();

        /// <summary>
        /// Gets/Sets the STOMP version level.
        /// </summary>
        ///                       
        public Single Level { get; set; }

        /// <summary>
        /// Gets the internal buffer size.
        /// </summary>
        /// 
        public Int32 BufferSize {
            get { return buffer.Count; }
        }

        /// <summary>
        /// Gets/Sets the internal buffer.
        /// </summary>
        /// 
        public Byte[] Buffer {
            get { return buffer.ToArray(); }
            set { buffer.AddRange(value); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="level">The STOMP version level.</param>
        /// 
        public Parser(String level = "1.0") {

            this.Level = Convert.ToSingle(level);

        }

        /// <summary>
        /// Filter the buffer.
        /// </summary>
        /// <returns>A STOMP frame.</returns>
        /// 
        public Frame Filter() {

            int i = 0;
            int state = 1;
            int length = 0;
            Frame frame = null;
            List<byte> key = new List<byte>();
            List<byte> body = new List<byte>();
            List<byte> value = new List<byte>();
            List<byte> command = new List<byte>();
            Dictionary<string, string> headers = new Dictionary<string, string>();

            while ((buffer.Count > 0) && (i < buffer.Count)) {

                if (state == 1) {

                    state = GetCommand(ref buffer, ref command, i);
                    i++;

                } else if (state == 2) {

                    state = GetKey(ref buffer, ref key, i);
                    i++;

                } else if (state == 3) {

                    state = GetValue(ref buffer, ref value, i);
                    i++;

                } else if (state == 4) {

                    string k = Stomp.ConvertToString(key.ToArray(), this.Level.ToString());
                    string v = Stomp.ConvertToString(value.ToArray(), this.Level.ToString());

                    Stomp.Unescape(ref k);
                    Stomp.Unescape(ref v);

                    headers.Add(k, v);

                    key.Clear();
                    value.Clear();

                    state = 2;

                } else if (state == 5) {

                    state = 6;

                    if (headers.ContainsKey("content-length")) {

                        length = Convert.ToInt32(headers["content-length"]);

                        if (length > buffer.Count) {

                            state = 8;

                        }

                    }

                } else if (state == 6) {

                    state = GetBody(ref buffer, ref body, length, ref i);

                } else if (state == 7) {

                    frame = new Frame();
                    frame.Command = Stomp.ConvertToString(command.ToArray(), this.Level.ToString());
                    frame.Headers = headers;
                    frame.Body = body.ToArray();
                    frame.Level = this.Level;

                    i++;
                    buffer.RemoveRange(0, i);

                    break;

                } else if (state == 8) {

                    break;

                }

            }

            return frame;

        }

        #region Private Methods

        private int GetCommand(ref List<byte> buffer, ref List<byte> command, int i) {

            int state = 1;

            if (buffer[i] != 10) {

                if (buffer[i] != 13) {

                    command.Add(buffer[i]);
                    state = 1;
                }

            } else {

                state = 2;

            }

            return state;

        }

        private int GetKey(ref List<byte> buffer, ref List<byte> key, int i) {

            int state = 2;

            if (buffer[i] != 10) {

                if (buffer[i] != 13) {

                    if (buffer[i] != 58) {

                        key.Add(buffer[i]);

                    } else {

                        state = 3;
                    }

                }

            } else {

                state = 5;

            }

            return state;

        }

        private int GetValue(ref List<byte> buffer, ref List<byte> value, int i) {

            int state = 3;

            if (buffer[i] != 10) {

                if (buffer[i] != 13) {

                    value.Add(buffer[i]);

                }

            } else {

                state = 4;

            }

            return state;

        }

        private int GetBody(ref List<byte> buffer, ref List<byte> body, int length, ref int i) {

            int state = 7;
            int count = 0;

            if (length > 0) {

                count = length + i;

                for (; i < count; i++) {

                    body.Add(buffer[i]);

                }

            } else {

                for (; i < buffer.Count; i++) {

                    if (buffer[i] != 0) {

                        body.Add(buffer[i]);

                    } else {

                        return state;

                    }

                }

                state = 8;

            }

            return state;

        }

        #endregion

    }

}
