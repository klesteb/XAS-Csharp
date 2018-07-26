using System;
using System.IO;
using System.Net.Mail;

using XAS.Core.Mime;
using XAS.Core.Logging;
using XAS.Core.Configuration;

namespace XAS.Core.Alerting {

    /// <summary>
    /// A class to send an email.
    /// </summary>
    /// 
    public class Email: IAlerting,IDisposable {

        private readonly ILogger log = null;
        private readonly IMimeTypes mimeTypes = null;
        private readonly IConfiguration config = null;

        private SmtpClient client = null;

        /// <summary>
        /// Gets/Sets the from field for the email.
        /// </summary>
        /// 
        public String From { get; set; }

        /// <summary>
        /// Gets/Sets the to field for the email.
        /// </summary>
        /// 
        public String To { get; set; }

        /// <summary>
        /// Gets/Sets the subject field for the email.
        /// </summary>
        /// 
        public String Subject { get; set; }

        /// <summary>
        /// Gets/Sets the filename of the attachment.
        /// </summary>
        /// 
        public String Attachment { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Email(IConfiguration config, IMimeTypes mimeTypes, ILoggerFactory logFactory) {

            this.config = config;
            this.mimeTypes = mimeTypes;
            
            this.To = "";
            this.From = "";
            this.Subject = "";
            this.Attachment = "";
    
            log = logFactory.Create(typeof(Email));

        }

        /// <summary>
        /// Send a email.
        /// </summary>
        /// <param name="text">The message for the email.</param>
        /// 
        public void Send(String text = "") {

            log.Trace("Entering Send()");

            var key = config.Key;
            var section = config.Section;

            string mxServer = config.GetValue(section.Environment(), key.MXServer());
            Int32 mxPort = Convert.ToInt32(config.GetValue(section.Environment(), key.MXPort()));
            Int32 timeout = Convert.ToInt32(config.GetValue(section.Environment(), key.MXTimeout()));

            client = new SmtpClient(mxServer, mxPort);
            client.Timeout = timeout * 1000;

            // build the message

            MailAddress from = new MailAddress(this.From);
            MailAddress to = new MailAddress(this.To);
            MailMessage message = new MailMessage(from, to);

            message.Body = text;
            message.Subject = this.Subject;

            // attach any attachements

            if (this.Attachment != "") {

                FileInfo file = new FileInfo(this.Attachment);

                if (file.Length > 0) {

                    string mimeType = mimeTypes.Type(file.Extension);
                    Attachment attachment = new Attachment(file.FullName, mimeType);

                    attachment.ContentDisposition.CreationDate = file.CreationTime;
                    attachment.ContentDisposition.ModificationDate = file.LastWriteTime;
                    attachment.ContentDisposition.ReadDate = file.LastAccessTime;

                    message.Attachments.Add(attachment);

                }

            }

            //send the email

            client.Send(message);

            log.Trace("Leaving Send()");

        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Generic Dispose method.
        /// </summary>
        /// <param name="disposing"></param>

        protected virtual void Dispose(bool disposing) {

            if (!disposedValue) {

                if (disposing) {

                    this.client = null;

                }

                disposedValue = true;

            }

        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ScheduledTask() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        /// <summary>
        /// Generic Dispose method.
        /// </summary>

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}
