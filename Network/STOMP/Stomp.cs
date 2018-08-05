using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;
using XAS.Network.Configuration.Extensions;

namespace XAS.Network.STOMP {

    /// <summary>
    /// Utility class to create STOMP frames.
    /// </summary>
    /// 
    public class Stomp {

        private ILogger log = null;
        protected readonly IConfiguration config = null;
        protected readonly IErrorHandler handler = null;
        protected readonly ILoggerFactory logFactory = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An IloggerFactory object.</param>
        /// 
        public Stomp(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;
            this.logFactory = logFactory;

            log = logFactory.Create(typeof(Stomp));

        }

        /// <summary>
        /// Create a CONNECT frame.
        /// </summary>
        /// <param name="login">Optional login name.</param>
        /// <param name="passcode">Optional password for login name.</param>
        /// <param name="virtualhost">Optional virtual host to use.</param>
        /// <param name="heartbeat">Optional wither to setup heartbeating.</param>
        /// <param name="acceptable">Optional acceptable level of STOMP version</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A properly configured frame.</returns>
        /// 
        public Frame Connect(
            String login = "guest",
            String passcode = "guest",
            String virtualhost = "/",
            String heartbeat = "0,0",
            String acceptable = "1.0,1.1,1.2",
            String level = "1.0") {

            Single target = Convert.ToSingle(level);
            Dictionary<string,string> headers = new Dictionary<string,string>();

            headers.Add("login", login);
            headers.Add("passcode", passcode);

            if (target > 1.0) {

                headers.Add("host", virtualhost);
                headers.Add("accept-version", acceptable);
                headers.Add("heart-beat", heartbeat);

            }

            // if greater then 1.1 return a STOMP frame, otherwise a CONNECT frame.

            if (target > 1.1) {

                return new Frame("STOMP", headers, null, level);

            } else {

                return new Frame("CONNECT", headers, null, level);

            }

        }

        /// <summary>
        /// Create a SUBSCRIBE frame.
        /// </summary>
        /// <param name="destination">Destination queue.</param>
        /// <param name="ack">The type of ACK to use.</param>
        /// <param name="id">Optional Id to use with subsequent frames.</param>
        /// <param name="receipt">Optional receipt nemonic.</param>
        /// <param name="prefetch">Optional number of frames to prefetch (server specific extension).</param>
        /// <param name="durable">Optional wither the frame is "durable" (server specific extension).</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A properly configured frame.</returns>
        /// 
        public Frame Subscribe(
            String destination,
            String id = "",
            String receipt = "",
            String ack = "auto",
            UInt32 prefetch = 0,
            Boolean durable = false,
            String level = "1.0") {

            var key = config.Key;
            var section = config.Section;
            Single target = Convert.ToSingle(level);
            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add("ack", ack);
            headers.Add("destination", destination);

            if (prefetch > 0) {

                // rabbitmq extension

                headers.Add("prefetch-count", prefetch.ToString());

            }

            if (durable) {

                // rabbitmq extension

                headers.Add("durable", "true");
                headers.Add("auto-delete", "false");

            }

            if (!String.IsNullOrEmpty(receipt)) {

                headers.Add("receipt", receipt);

            }

            if (!String.IsNullOrEmpty(id)) {

                headers.Add("id", id);

            } else {

                if (target > 1.0) {

                    // v1.1 and greater must have an id header

                    throw new ArgumentException(config.GetValue(section.Messages(), key.StompSubscribeException()));

                }

            }

            return new Frame("SUBSCRIBE", headers, null, level);

        }

        /// <summary>
        /// Create a UNSUBSCRIBE frame.
        /// </summary>
        /// <param name="id">Optional id used with subsequent frames.</param>
        /// <param name="receipt">Optional receipt nemonic.</param>
        /// <param name="destination">Optional destination queue</param>
        /// <param name="durable">Optional wither the frame is durable (server specific extension).</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A properly configured frame.</returns>
        /// 
        public Frame Unsubscribe(
            String id = "",
            String receipt = "",
            String destination = "",
            Boolean durable = false,
            String level = "1.0") {

            var key = config.Key;
            var section = config.Section;
            Single target = Convert.ToSingle(level);
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(receipt)) {

                headers.Add("receipt", receipt);

            }

            if (durable) {

                // rabbitmq extension

                headers.Add("durable", "true");
                headers.Add("auto-delete", "false");

            }

            // v1.0 should have either a destination and/or id header
            // v1.1 and greater may have a destination header

            if (!String.IsNullOrEmpty(destination) && !String.IsNullOrEmpty(id)) {

                headers.Add("destination", destination);
                headers.Add("id", id);

            } else if (!String.IsNullOrEmpty(destination)) {

                headers.Add("destination", destination);

            } else if (!String.IsNullOrEmpty(id)) {

                headers.Add("id", id);

            } else {

                throw new ArgumentException(config.GetValue(section.Messages(), key.StompUnsubscribeException()));

            }

            if (target > 1.0) {

                // v1.1 and greater must have a id header

                if (String.IsNullOrEmpty(id)) {

                    throw new ArgumentException(config.GetValue(section.Messages(), key.StompUnsubscribeException()));

                }

            }

            return new Frame("UNSUBSCRIBE", headers, null, level);

        }

        /// <summary>
        /// Create a BEGIN frame.
        /// </summary>
        /// <param name="transaction">A transaction id.</param>
        /// <param name="receipt">Optional receipt nemonic.</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A properly configured frame.</returns>
        /// 
        public Frame Begin(
            String transaction,
            String receipt = "",
            String level = "1.0") {

            Single target = Convert.ToSingle(level);
            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add("transaction", transaction);

            if (!String.IsNullOrEmpty(receipt)) {

                headers.Add("receipt", receipt);

            }

            return new Frame("BEGIN", headers, null, level);

        }

        /// <summary>
        /// Create a COMMIT frame.
        /// </summary>
        /// <param name="transaction">A transaction id.</param>
        /// <param name="receipt">Optional receipt nemonic.</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A properly configured frame.</returns>
        /// 
        public Frame Commit(
            String transaction,
            String receipt = "",
            String level = "1.0") {

            Single target = Convert.ToSingle(level);
            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add("transaction", transaction);

            if (!String.IsNullOrEmpty(receipt)) {

                headers.Add("receipt", receipt);

            }

            return new Frame("COMMIT", headers, null, level);

        }

        /// <summary>
        /// Create an ABORT frame.
        /// </summary>
        /// <param name="transaction">A transaction id.</param>
        /// <param name="receipt">Optional receipt nemonic.</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A properly configured frame.</returns>
        /// 
        public Frame Abort(
            String transaction,
            String receipt = "",
            String level = "1.0") {

            Single target = Convert.ToSingle(level);
            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add("transaction", transaction);

            if (!String.IsNullOrEmpty(receipt)) {

                headers.Add("receipt", receipt);

            }

            return new Frame("ABORT", headers, null, level);

        }

        /// <summary>
        /// Create an ACK frame.
        /// </summary>
        /// <param name="messageId">The message id.</param>
        /// <param name="transaction">Optional transaction id.</param>
        /// <param name="receipt">Optional receipt nemonic.</param>
        /// <param name="subscription">Optional subscription id.</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A properly configured frame.</returns>
        /// 
        public Frame Ack(
            String messageId,
            String transaction = "",
            String receipt = "",
            String subscription = "",
            String level = "1.0") {

            var key = config.Key;
            var section = config.Section;
            Single target = Convert.ToSingle(level);
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (target < 1.2) {

                headers.Add("message-id", messageId);

            } else {

                headers.Add("id", messageId);

            }

            if (!String.IsNullOrEmpty(receipt)) {

                headers.Add("receipt", receipt);

            }

            if (!String.IsNullOrEmpty(transaction)) {

                headers.Add("transaction", transaction);

            }

            if (!String.IsNullOrEmpty(subscription)) {

                headers.Add("subscription", subscription);

            } else {

                if (target > 1.0) {

                    throw new ArgumentException(config.GetValue(section.Messages(), key.StompAckException()));

                }

            }

            return new Frame("ACK", headers, null, level);

        }

        /// <summary>
        /// Create a NACK frame.
        /// </summary>
        /// <param name="messageId">The message id.</param>
        /// <param name="transaction">Optional transaction id.</param>
        /// <param name="receipt">Optional receipt nemonic.</param>
        /// <param name="subscription">Optional subscription id.</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A properly configured frame.</returns>
        /// 
        public Frame Nack(
            String messageId,
            String transaction = "",
            String receipt = "",
            String subscription = "",
            String level = "1.0") {

            var key = config.Key;
            var section = config.Section;
            Single target = Convert.ToSingle(level);
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (target < 1.1) {

                throw new ArgumentException(config.GetValue(section.Messages(), key.StompNackVersionException()));

            }

            if (!String.IsNullOrEmpty(receipt)) {

                headers.Add("receipt", receipt);

            }

            if (!String.IsNullOrEmpty(transaction)) {

                headers.Add("transaction", transaction);

            }

            if (target < 1.2) {

                headers.Add("message-id", messageId);

            } else {

                headers.Add("id", messageId);

            }

            if (!String.IsNullOrEmpty(subscription)) {

                headers.Add("subscription", subscription);

            } else {

                if (target > 1.0) {

                    throw new ArgumentException(config.GetValue(section.Messages(), key.StompNackSubscrptionExecption()));

                }

            }

            return new Frame("NACK", headers, null, level);

        }

        /// <summary>
        /// Create a DISCONNECT frame.
        /// </summary>
        /// <param name="receipt">Optional receipt nemonic.</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A properly configured frame.</returns>
        /// 
        public Frame Disconnect(
            String receipt = "",
            String level = "1.0") {

            Single target = Convert.ToSingle(level);
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (target > 1.0) {

                if (!String.IsNullOrEmpty(receipt)) {

                    headers.Add("receipt", receipt);

                }

            }

            return new Frame("DISCONNECT", headers, null, level);

        }

        /// <summary>
        /// Create a SEND frame.
        /// </summary>
        /// <param name="destination">The desination queue.</param>
        /// <param name="message">The message as a byte array.</param>
        /// <param name="receipt">Optional receipt nemonic.</param>
        /// <param name="transaction">Optional transaction id.</param>
        /// <param name="mimeType">Optional MIME type.</param>
        /// <param name="length">Optional length of message.</param>
        /// <param name="persistent">Optional wither the message is "presistent" (server specific extension).</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A properly configured frame.</returns>
        /// 
        public Frame Send(
            String destination,
            Byte[] message,
            String receipt = "",
            String transaction = "",
            String mimeType = "text/plain",
            UInt32 length = 0,
            Boolean persistent = false,
            String level = "1.0") {

            Single target = Convert.ToSingle(level);
            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add("destination", destination);

            if (! String.IsNullOrEmpty(receipt)) {

                headers.Add("receipt", receipt);

            }

            if (! String.IsNullOrEmpty(transaction)) {

                headers.Add("transaction", transaction);

            }

            if (persistent) {

                // rabbitmq extension, supported by others thou

                headers.Add("persistent", persistent.ToString());

            }

            // always add a content length

            if (length > 0) {

                headers.Add("content-length", length.ToString());

            } else {

                Int32 count = message.Length;
                headers.Add("content-length", count.ToString());

            }

            if (target > 1.0) {

                headers.Add("content-type", mimeType);

            }

            return new Frame("SEND", headers, message, level);

        }

        /// <summary>
        /// Convert a string to a byte array.
        /// </summary>
        /// <param name="data">A string.</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A byte array.</returns>
        /// 
        public static Byte[] ConvertToBytes(String data, String level = "1.0") {

            byte[] buffer;
            Single target = Convert.ToSingle(level);

            if (target > 1.1) {

                buffer = System.Text.Encoding.UTF8.GetBytes(data);

            } else {

                buffer = System.Text.Encoding.ASCII.GetBytes(data);

            }

            return buffer;

        }

        /// <summary>
        /// Convert a byte array to a string.
        /// </summary>
        /// <param name="data">A byte array.</param>
        /// <param name="level">The STOMP version level.</param>
        /// <returns>A string.</returns>
        /// 
        public static String ConvertToString(Byte[] data, String level = "1.0") {

            String buffer;
            Single target = Convert.ToSingle(level);

            if (target > 1.1) {

                buffer = System.Text.Encoding.UTF8.GetString(data);

            } else {

                buffer = System.Text.Encoding.ASCII.GetString(data);

            }

            return buffer;

        }

        /// <summary>
        /// Escape data for transmittal.
        /// </summary>
        /// <param name="data">A refernence to a string.</param>
        /// 
        public static void Unescape(ref String data) {

            data = Regex.Replace(data, @"\\\\r", "\r");
            data = Regex.Replace(data, @"\\\\n", "\n");
            data = Regex.Replace(data, @"\\\\c", @":");
            data = Regex.Replace(data, @"\\\\", @"\");

        }

        /// <summary>
        /// Unescape data from transmittal.
        /// </summary>
        /// <param name="data">A reference to a string.</param>
        /// 
        public static void Escape(ref String data) {

            data = Regex.Replace(data, @"\\", @"\\");
            data = Regex.Replace(data, "\r", @"\\r");
            data = Regex.Replace(data, "\n", @"\\n");
            data = Regex.Replace(data, @":", @"\\c");

        }

    }

}