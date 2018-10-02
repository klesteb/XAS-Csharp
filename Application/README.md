## **Application**

I don't write GUI code, wither Windows or Web based. Most of my applications 
focus on the single command, a command shell or a service. This project 
reflects that basis.  

### **The command line**

There is a common command line syntax. This allows for a familiar exerience
with each command. You can use either an option switch of "/", "-" or "--" to 
preface your options. You can not mix the option switches within one command 
line. Words without a preceding switch are considered parameters. Multiple words 
that make up one parameter should be quoted. Single character options can be 
stacked and should begain with eithter a "/" or "-". The stacked options must 
be unique from long word options. Long word options must be complete, there 
are no abreviations. Options processing stops at a standalone "--" and the 
rest the command line is passed verbatiom. Environment variables within the
command line are parsed and their values subsituted in place.

### **Common options**

Each application, wither it is cli, shell or service based has the same
common set of options. They are as follows:

    -alerts     - toggles the sending of alerts.
    -debug      - toggles debug output to the log.
    -trace      - toggles trace output to the log.
    -version|v  - outputs the programs version.
    -help|h|?   - outputs a simple help message.
    -manual     - outputs the progams manual.
    -facility=  - sets the alert facility.
    -priority=  - sets the alert priority.
    -log-type=  - toggles the log type.
    -log-file=  - names the log file to use.
    -log-conf=  - alternative logging configuration file.

A command line application adds this option:

    -minwin     - minimizes the console window.

A service application adds these options:

    -install|i    - install the service.
    -uninstall|u  - uninstall the service.

A common optional option is:

    -cfg-file=    - use an alternative configuration file.

The demo applications show how to use this common option.

### **Configuration files**

When an application starts up, it can process an optional configuration
file. This file is located at %XAS_ETC%\<application name>.ini. This
uses the familiar windows .ini style of config file. I find this style
is more readable then Microsofts app.config format. When the configuration
is loaded, the following stanza is processed.

    [application]
	alerts = true           - default is true.
    facility = testing      - default is "system".
    priority = low          - default is "low".
    trace = true            - default is false.
	debug = false           - default is false.
    log-type = file         - default is "console".
	log-file = <filename>   - the path to a log file.
	log-conf = <filename>   - the path to log4net configuration file.

This set defaults for the application. These can be overridden on the 
command line. This makes it very easy to modify the behavior of a service.
Something that is not easy to do from the command line. 

The following takes a boolean value:

	alerts
	trace
	debug

Boolean values use the following verbage:

    boolean => true = yes, 1, true
	boolean => false = no, 0, false

Alerting is built into the application. The following is used to
control the serverity of the alert.

    facility - free form text
	priority - free form text

This can be anything that has meaning for your alerting platform. When
an alert is generated, it creates a spool file in %XAS_SPOOL%\alerts.

	log-type - "console", "file", "json" or "event"

A log type of "file" will use a file to record the log into. By default
this is located at %XAS_LOG%\<application name>.log. A log type of
"json" will create a spool file in %XAS_SPOOL%\logs. A log type of "event"
will create an entry in the Windows Event Log using the "Application" area.

    log-conf 

This is used to specify an alterntaive log4net configuration. 

Spooling is used to decouple the local application from a remote resource. 
It is much easier to write a local spool file then to connect to a remote 
resource. Especially when that remote resource is unavailable.


