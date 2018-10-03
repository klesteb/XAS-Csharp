## **XAS.Application**

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

Each application, wither it is a command, shell or service based has the same
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

The following takes a boolean value:

	alerts
	trace
	debug

Boolean values use the following verbage:

    boolean => true = yes, 1, true
	boolean => false = no, 0, false

Alerting is built into the application. The following is used to
control the serverity of the alert.

    facility - free form text, default is "system"
	priority - free form text, default is "low"

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

## **Basic usage**

All applications can be invoked in a similar fashion. For example:

	C:\dev\XAS\DemoApp\bin\Debug>DemoApp.exe -help

	Usage: DemoApp
	   or: DemoApp --help

	  Options:
		-alerts     - toggles the sending of alerts.
		-debug      - toggles debug output to the log.
		-trace      - toggles trace output to the log.
		-version|v  - outputs the programs version.
		-help|h|?   - outputs a simple help message.
		-manual     - outputs the progams manual.
		-facility=  - sets the alerts facility, default "system".
		-priority=  - sets the alerts priority, default "low".
		-log-type=  - toggles the log type, default "console", possible "file", "json" or "event".
		-log-file=  - names the log file to use, default "C:\XAS\var\log\DemoApp.log".
		-log-conf=  - alternative logging configuration file, default "C:\XAS\etc\DemoApp.conf".
		-minwin     - minimizes the console window.
		-cfg-file=  - use an alternative configuration file, default: "C:\XAS\etc\DemoApp.ini".

	Options can begin with a "-","--" or "/". Single character options must
	start with a "-" and can be stacked. Options with a trailing "=" have required
	arguments, options with a trailing ":", have optional arguments. Options that are
	being used to toggle functionality can have a trailing "-" or "+" to force boolean
	behavior. Option processing ends with "-- " and the rest of the commadline is
	passed verbatium.

Doing this with any of the application will produce this simple help screen. Doing this:

	C:\dev\XAS\DemoApp\bin\Debug>DemoApp.exe -manual

	NAME

		DemoApp - A demo application using XAS.App

	SYNPOSIS

		DemoApp [--help] [--manual] [--version]

	DESCRIPTION

		This program is a demo command line application using XAS.App.

	OPTIONS and ARGUMENTS

		Options:
		  -alerts     - toggles the sending of alerts.
		  -debug      - toggles debug output to the log.
		  -trace      - toggles trace output to the log.
		  -version|v  - outputs the programs version.
		  -help|h|?   - outputs a simple help message.
		  -manual     - outputs the progams manual.
		  -facility=  - sets the alerts facility, default "system".
		  -priority=  - sets the alerts priority, default "low".
		  -log-type=  - toggles the log type, default "console", possible "file", "json" or "event".
		  -log-file=  - names the log file to use, default "C:\XAS\var\log\DemoApp.log".
		  -log-conf=  - alternative logging configuration file, default "C:\XAS\etc\DemoApp.conf".
		  -minwin     - minimizes the console window.
		  -cfg-file=  - use an alternative configuration file, default: "C:\XAS\etc\DemoApp.ini".

	CONFIGURATION

		The default configuration file is "C:\XAS\etc\DemoApp.ini", and contains the following stanzas:

			[application]
			alerts = true
			facility = systems
			priority = low
			trace = false
			debug = false
			log-type = file
			log-file = C:\XAS\var\log\DemoApp.log
			log-conf = C:\XAS\etc\DemoApp.conf

		This is the basic options that every program has, they can be overridden on the command line.
		The above are the defaults and this stanza is not really needed. But it does allow you to easily
		configure a service.

	EXIT CODES

		0 - success
		1 - failure

	SEE ALSO

	AUTHOR

		Kevin L. Esteb - kevin@kesteb.us

	COPYRIGHT AND LICENSE

		Copyright (c) 2018 Kevin L. Esteb

Will display simple documentation on how the program works. This is using the familiar
UNIX man page format. Doing this will print out the version of the application.

	C:\dev\XAS\DemoApp\bin\Debug>DemoApp.exe -version
	Version: v1.0.0.0

All of which helps with troubleshooting problems when they arise. 

## **Shells**

This creates a shell style of interface. In that you can type commands and 
display the results. This is not a Turing complete interperator. Invoking a
shell you will get this prompt:

	C:\dev\XAS\DemoShell\bin\Debug>DemoShell.exe
	Help is available with the "help" command.
	>

Typing "help" will display this:

	> help

	Internal Commands:
		clear  - clear the screen.
		cls    - clear the screen.
		exit   - exit the shell.
		help   - display this screen.
		quit   - exit the shell.

	Additional Commands:
		set      - set global settings.
		show     - show global settings.
		schedule - schedule a job to run.

	Additional help is available with <command> --help.

	>

Asking for additional help on the "schedule" command displays this:

	> schedule -help

	Usage: schedule --requestor <username> "<job parameters>"

	  Options:
		-help        - outputs a simple help message..
		-requestor:  - the requestor of the job..
		-date:       - the date to submit the job on, defaults to "today"..
		-time:       - the time to start the job, defaults to "now"..
		-group:      - the group to submit the job too, defaults to "production"..
		-target:     - the target to sumbit the job too, defaults to "production"..

	Options can begin with a "-","--" or "/". Single character options must
	start with a "-" and can be stacked. Options with a trailing "=" have required
	arguments, options with a trailing ":", have optional arguments. Options that are
	being used to toggle functionality can have a trailing "-" or "+" to force boolean
	behavior.

	>

Which should look familiar. The shell also has the feature of loading and 
executing commands from a file. Oldtimers like me, would recognize this 
feature from DEC's PDP11 line of operating systems. Where it was called an
"indirect" command file. You can use this feature as follows:

	C:\dev\XAS\DemoShell\bin\Debug>DemoShell.exe
	Help is available with the "help" command.
	> @test.job
	>

Where test.job looks like this:

	C:\dev\XAS\DemoShell\bin\Debug>type test.job
	set -requestor testing

Which is nothing but a series of commands that the shell will execute one after
the other. This is particularly usefull when setting up a command shell environment. 

## **Services**

A service is a background process that is controlled with the Windows SCM. They
are notoriously difficult to install, configure and debug. We are trying to
simplify this process. To install the service do the following:

   C:\dev\XAS\DemoService\bin\Debug>DemoService.exe -install

To desinstall, do the following:

   C:\dev\XAS\DemoService\bin\Debug>DemoService.exe -uninstall

To run it interactivily, do the following:

	C:\dev\XAS\DemoService\bin\Debug>DemoService.exe
	INFO  - Service has been started
	Running Service: Enter either [Q]uit, [P]ause, [R]esume:

Where you can stop, pause or resume the service. Since this is using
the common command line options you can toggle tracing or debugging and
specify a alternative configuration to use. All of which makes interacting 
with services somewhat less annoying.

## **Being a good command line citizen**

A good command line citzen show do the following:

    * Accept input on stdin
	* Write output to stdout
	* Write error messages to stderr
	* Return a meaningful exit code

A command or shell will do the following:

    * Accept input from stdin
	* You can write to stdout
	* Console logging goes to stderr
	* By default, it will return 0 for success and 1 for failure

A service has no concept of stdin, stdout or stderr, even thou the .NET
runtime does a good job of faking that. 

