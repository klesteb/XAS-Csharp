# ***XAS.Core***

This is the core modules that the rest of the system is built on. It provides
the following facilities.

### Alerting

A general purpose alerting platform. This used to send "alerts" to outside 
entities.

### Configuration

A general purpose configuration system. Uses the tried true method of sections
with key/value pairs. This also builds a base enviornment that the rest of the
system uses to find "stuff".

### Data Structures

Usefull predefined data structures.

### Exceptions

Usefull predefined exceptions. Also contains methods to display exceptions in
a pleasing manner.

### Extensions

Some usefull String and DateTime extensions.

### Locking

A factory and supporting classes for locking to access to common system 
resources such as directories in the file system.

### Logging

A factory and supporting classes to provide a logging framework. This is
based on the log4net library. This includes prepackaged logging configurations
for "console", "file", "event" and "json".

### MIME

Provides MIME support.

### Security

Provides modules for security. Such as authenticating against AD and 
dealing with the UAC bullshit and some other usefull modules.

### Spooling

Uses the tried and true methods for maintaining spool files in spool
directories. Nothing fancy here, just the way they have been doing it on
UNIX since the mid '70s.

# ***The XAS Environment***

Microsoft has an interesting notion on where applications should live and
where their resources are located. And the .NET environment does a really good
job of enforcing that notion. 

Unfortuantly, that notion falls woefully short of a system that has multiple 
applications, services and shared resources. For that situation, we have to 
look outside of the Windows world and see what has been done before. 

We have to go no further then UNIX to see a different perspective. The UNIX 
filesystem is designed for software systems to share resources. This has been 
a long journey for the UNIX fathfull. It started in the early '70s, had many 
ups, downs, infightings and eventually stablized in the modern UNIX offerings.

The XAS directory structure mimics the UNIX filesystem. This layout can be 
reconfigured with environment variables. It is layed out as follows:

Directory | Environment Variable | Purpose
--------- | -------------------- | -------
\ | XAS_ROOT | Root directory (C:\xas\)
\bin | XAS_BIN | Command line applications
\etc | XAS_ETC | Configuration files
\lib | XAS_LIBS | Shared libraries (including 3rd party)
\sbin | XAS_SBIN | Services
\tmp | XAS_TMP | Temporary work space
\var |  | Variable data 
\var\lib | XAS_LIB | Variable data, such as databases
\var\run | XAS_RUN | Run time data, such as pid files
\var\log | XAS_LOG | Log files
\var\locks | XAS_LOCKS | Shared locks
\var\spool | XAS_SPOOL | Spooler data
\var\spool\alerts |  | Outbound alerts
\var\spool\logs |  | Outbound JSON formatted log entries

The root directory can reside on any available disk device, it defaults to
C:\xas. The whole XAS operations environment resides within this directory 
structure. This works well, as most of the supporting libraries are not self 
signed (including Microsofts Entity Framework) and don't reside in the GAC.
Except log4net. This needs to be in the GAC and luckily it is signed. 

In my opinion, this is the basic environment needed to run in a modern
DevOps oriented world.

