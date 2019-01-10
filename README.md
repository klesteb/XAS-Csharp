# XAS

This is a partial implementation of the XAS Perl modules in C#. In some ways, Perl is a much easier language to program in. 
You can build wonderfully complex data structures without any thought and morph the object model in new and interesting ways. 
Not so in C#. Where you are much more concerned with variableness and program structure. 

Which I believe is a hindrance to good programing. But this is not what they teach in CS classes. Of course you can always 
memorize the C# language guide and live on Stackexchange, and maybe you will produce good code. Who knows, stranger things 
have happened. Remember, we went to the moon with 64K of memory, now we can barely load an OS and a word processor is 64GB 
of memory. I guess that is progress. 

## Core

The base set of classes. They provide the basic environment, alerting, logging, locking, configuration and spooling needed 
for the rest of the system. The environment consists of a directory structure that mimics the classic UNIX file system layout.
This environment can be overridden by environment variables. This code tries to adhere to current thoughts on DevOps. 
Production code needs to run consistently and when it fails, it needs to leave a trail of useful information on why it failed. 

For example. If the program raises an alert. It is written out as a spool file. Why? Wouldn't be easier to just send the alert
directly to its destination. Maybe. What if the network is down, what if the destination is not responding? Now your program
has to deal with that situation and that is not its primary function. Writing a spool file is simple and easy. It also 
decouples your program from the actual delivery of the alert. Now a "spooler" is responsible for the delivery of the alert.

## Application

I only write command line utilities and services. This provides the core classes to do this easily. They provide a consistent 
command line interface, a simple help system and they return a meaningful exit code so that they are good command line citizens.
A shell style of interface is provided. A service can be ran from the command line for easier debugging. A configuration 
file can be used to define default values, extremely useful for services. The configuration file values can be overridden by 
the command line.

## Model

An attempt to make the Entity Framework useful. Much better then hardcoding SQL into your program.

## Network

A set of classes to make network programming easier. It provides a simple async TCP client, a STOMP messaging protocol client, 
a FTP client and a DNS client.

## Rest

Client and server classes for building and interacting with REST based services.

## Demos

There are various demo projects that will hopefully show you how to use this code.

## Installation

There are two setup modules available. You need to install SetupCore first. This provides the dependent libraries and creates 
the directory structure needed to allow the code to run. It also installs the spooler. You will need to manually install the 
spooler into SCM. 

    C:\> C:\XAS\sbin\xas-spoolerd -install

And change the XAS-Spooler services start feature from manaul to automatic, if so desried. You will also have to edit the
configuration file for your environment. 

If you installed the package someplace other then C:\xas\. Then you need to create the system wide environment variable XAS_ROOT to 
point to this directory structure.

SetupDemo will install the demos. Yow can install the services, but they may be best to run from the command line.

## Open Source

This code is not stand alone. It makes use of other Open Source projects. A grateful thanks to those authors for providing their code
for others to use. 

Kevin


