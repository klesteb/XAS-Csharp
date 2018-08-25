# ***The XAS Spooling Service***

Store and Forward queuing systems have been around as long as there have been 
operating systems. This spooling service is just an iteration of this fundemental
concept.

Spooling is used to decouple the XAS Environmnet from outside resources. This 
makes the code simpler and less brittle. It also provides a buffer. All the
local application or service  needs to worry about is writing a spool file. 
Eveything else is handled by the spooler.

Creating spool files is built into the system. If you generate an alert, a
spool file is created. If you use JSON logging, a spool file is created, one
per each log entry.

### ***How it works***

The basics of the spooler is to process files in directories and send them to 
queues on a message queue server. Pretty basic stuff.

Each directory in the spooler system is associated with a queue on the messsage
queue server. Files placed into these directories are JSON data structures. They
are loaded, and a header is created. The header consists of the local 
system name, a UNIX time stamp and the data type that is contained. Which is 
also seralized into JSON. These data structures are combined into one JSON blob.
The header is used to help the receiver of this data to determine what to do 
with it.

Communications with the message queue server is done with the STOMP protocol. A
STOMP SEND frame is created. It is marked as "presistent" and a "receipt" is 
attached. The JSON blob is the message. The receipt consists of an "alias" 
and the filename. This "receipt" is then encoded into a base64 string. This 
"receipt" is used in the ack. The ack is a STOMP RECEIPT frame. When this is
received, the "receipt" is decoded and parsed to find the filename. At that 
point the file is removed from the directory. 

This methodology was developed because not all messages queue servers 
supported the STOMP transaction feature. Also with the frame marked as 
"presistent", it was determined to be as good as writting the blob to 
the local file system.

A configuration file is used to define the directories and the location of the
message queue server. 

### ***Configuration***

The XAS Configuration sub system consists of sections that contains key/value 
pairs. This can be populated from many sources. In this case it is populated
from the familiar Windows .ini style of configuration file. The configuration 
file %XAS_ETC%\xas-spoolerd.ini is used for this purpose. 

The configuration file contains three stanzas. They are the following:

    [applicaton]
    alerts = true
    facility = systems
    priority = low
    trace = false
    debug = false
    log-type = file
    log-file = %XAS_LOG%\xas-spoolerd.log
    log-conf = %XAS_ETC%\xas-spoolerd.conf

	[message-queue]
    server = %XAS_MQSERVER%
    port = %XAS_MQPORT%
    use-ssl = false
    username = guest
    password = guest
    keepalive = true;
    level = 1.0

	[%XAS_SPOOL%\alerts]
    queue = /queue/alerts
    packet-type = xas-alerts
	alias = unlink

	[%XAS_SPOOL%\logs]
    queue = /queue/logs
    packet-type = xas-logs
	alias = unlink

The %<name>% needs to be hardcoded. It doesn't subsitute the environment variable. 
But these are the defaults. The "applicaiton" stanza is used to set command line
defaults. This is very usefull for services. The "message-queue" stanza defines
which message queue server to use. The rest are for the directories that are being
used by the spooling system. There can be as many as you need.
