# ***The XAS Spooling Service***

Store and forward queuing systems have been around as long as there have been 
operating systems. This spooling service is just an iteration of this fundemental
concept.

Spooling is used to decouple the XAS Environmnet from outside resources. This 
makes the code simpler and less brittle. It also provides a buffer. All the
local code needs to worry about is writing a spool file. Eveything else is 
handled by the spooler.

Creating spool files is built into the system. If you generate an alert, a
spool file is created. If you use JSON logging, a spool file is created, one
per each log entry.

Hey, it works for email, and has beeen used, since email was invented. So it 
can't be all bad.
