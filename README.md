# XAS

This is a partial implementation of the XAS Perl modules in C#. In someways, Perl is a much easier language to program in. 
You can build wonderfully complex data structures without any thought and morph the object model in new and interesting ways. 
Not so in C#. Where you are much more concerned with variableness, program structure. 

Which I believe is a hindrance to good programing. But this is not what they tech in CS classes. Of course you can always 
memorize the C# language guide and live on Stackexchange, and maybe you will produce good code. Who knows, stranger things 
have happened. Remember, we went to the moon with 64K of memory, now we can barely load an OS and a word processor is 64GB 
of memory. I guess that is progress. 

## Core

The base set of classes. They provide the basic environment, alerting, logging, locking, configuration and spooling needed 
for the rest of the system. The environment consists of a directory strucutre that mimics the classic UNIX file system layout.
This environment can be overridden by environment variables. This code tries to adhere to current thoughts on DevOps. 
Production code needs to run consistently and when it fails, it needs to leave a trail of useful information on why it failed. 

## Application

I only write command line utilties and services. This provides the core classes to do this easily. They provide a consistent 
commandline interface, a simple help system and they return a meaningful exit code so that they are good command line citizens.
A shell style of interface is provided. A service can be ran from the commmand line for easier debugging. A configuration 
file can be used to define default values, extermely useful for services. The configuration file values can be overridden by 
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

## Open Source

This code is not stand alone. It make use of other Open Source projects. A grateful thanks to those authors for providing their modules
for others to use. 

Kevin

