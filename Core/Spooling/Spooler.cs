﻿using System;
using System.IO;
using System.Collections.Generic;

using XAS.Core.Locking;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

namespace XAS.Core.Spooling {

    /// <summary>
    /// Spool class.
    /// </summary>
    /// <remarks>
    /// This class will create and maintain spool files.
    /// </remarks>
    /// 
    public class Spooler: ISpooler {

        private readonly ILocker locker = null;
        private readonly IConfiguration config = null;

        /// <summary>
        /// Gets/Sets the spool file extension.
        /// </summary>
        /// 
        public String Extension { get; set; }

        /// <summary>
        /// Gets/Sets the sequence file name.
        /// </summary>
        /// 
        public String Seqfile { get; set; }

        /// <summary>
        /// Gets/Sets the spool directory.
        /// </summary>
        /// 
        public String Directory { get; set; }

        /// <summary>
        /// Gets/Sets the number of retries to write the spool file.
        /// </summary>
        /// 
        public Int32 Retries { get; set; }

        /// <summary>
        /// Gets/Sets the timeout to wait when writing a spool files fails.
        /// </summary>
        /// 
        public Int32 Timeout { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Spooler(IConfiguration config, ILocker locker) {

            this.config = config;
            this.locker = locker;

            var key = config.Key;
            var section = config.Section;

            this.Seqfile = ".SEQ";
            this.Extension = ".pkt";
            this.Directory = config.GetValue(section.Environment(), key.VarSpoolDir());
            this.Retries = 10;
            this.Timeout = 30;

        }

        /// <summary>
        /// Read the spool file.
        /// </summary>
        /// <param name="filename">The spool file to read.</param>
        /// <returns>A byte array of the file contents.</returns>
        /// 
        public Byte[] Read(string filename) {

            byte[] packet = null;

            if (locker.Lock()) {

                packet = ReadPacket(filename);
                locker.Unlock();

            }

            return packet;

        }

        /// <summary>
        /// Write a spool file.
        /// </summary>
        /// <param name="packet">A byte array to write out.</param>
        /// 
        public void Write(Byte[] packet) {

            uint seqnum = 0;

            if (locker.Lock()) {

                seqnum = Sequence();
                WritePacket(packet, seqnum);

                locker.Unlock();

            }

        }

        /// <summary>
        /// Scan a spool directory for files.
        /// </summary>
        /// <returns>A FileInfo array.</returns>
        /// 
        public List<String> Scan() {

            var files = new List<String>();
            string pattern = String.Format("*{0}", this.Extension);

            if (locker.Lock()) {
               
                foreach (string file in System.IO.Directory.EnumerateFiles(this.Directory, pattern)) {

                    files.Add(file);

                }

                locker.Unlock();

            }

            return files;

        }

        /// <summary>
        /// Delete a spool file.
        /// </summary>
        /// <param name="filename">The name of the spool file.</param>
        /// <returns>True on success.</returns>
        /// 
        public Boolean Delete(string filename) {

            bool stat = false;

            if (locker.Lock()) {

                if (File.Exists(filename)) {

                    File.Delete(filename);
                    stat = true;

                }

                locker.Unlock();

            }

            return stat;

        }

        /// <summary>
        /// Count the number of files in a spool directory.
        /// </summary>
        /// <returns>The number of files.</returns>
        /// 
        public Int32 Count() {

            Int32 count = 0;
            string pattern = String.Format("*{0}", this.Extension);

            if (locker.Lock()) {

                // theres gotta be a better way!

                foreach (string file in System.IO.Directory.EnumerateFiles(this.Directory, pattern)) {

                    count++;

                }

                locker.Unlock();

            }

            return count;

        }

        /// <summary>
        /// Get the first file in a spool directory.
        /// </summary>
        /// <returns>The FileInfo for that file.</returns>
        /// 
        public String Get() {

            String file = "";
            string pattern = String.Format("*{0}", this.Extension);

            if (locker.Lock()) {

                // theres gotta be a better way!

                foreach (string f in System.IO.Directory.EnumerateFiles(this.Directory, pattern)) {

                    file = f;
                    break;

                }

                locker.Unlock();

            }

            return file;

        }

        # region Private Methods

        private uint Sequence() {

            uint seqnum = 1;
            string filename = Path.Combine(this.Directory, this.Seqfile);

            Retry.WhileException<int>(Retries, Timeout, () => {

                if (File.Exists(filename)) {

                    using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) {

                        BinaryWriter writer = new BinaryWriter(stream);
                        BinaryReader reader = new BinaryReader(stream);

                        seqnum = reader.ReadUInt32();
                        seqnum += 1;

                        stream.Seek(0, SeekOrigin.Begin);
                        writer.Write(seqnum);

                    }

                } else {

                    using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None)) {

                        BinaryWriter writer = new BinaryWriter(stream);

                        stream.Seek(0, SeekOrigin.Begin);
                        writer.Write(seqnum);

                    }

                }

                return 0;

            }, typeof(System.IO.IOException));

            return seqnum;

        }

        private void WritePacket(byte[] packet, uint seqnum) {

            string filename = Path.Combine(this.Directory, seqnum + this.Extension);

            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None)) {

                BinaryWriter writer = new BinaryWriter(stream);

                writer.Write(packet);

            }

        }

        private byte[] ReadPacket(string filename) {

            byte[] packet = null;

            if (File.Exists(filename)) {

                using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None)) {

                    BinaryReader reader = new BinaryReader(stream);

                    // may be a problem for files greater then 600mb.

                    packet = reader.ReadBytes((int)stream.Length);

                }

            }

            return packet;

        }

        #endregion

    }

}

