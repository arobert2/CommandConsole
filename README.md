# CommandConsole #
##### Real name TBA. #####

CommandConsole is a C# powered multithreaded application interface implementing WPF.

###Usage###

CommandConsole uses .Net 4.6.2

To use the console simply compile and run. To get started type listcommands.

###Extending The CommandConsole###

#####How It Works#####

All commands in the CommandConsole are classes derived from either ICommand or IApp, which IApp is derived from ICommand.
ICommand objects run in the thread their parent IApp object is running in while IApp objects run in their own thread.
Commands are passed to a queue specific to each task (TaskSystem.CommandQueue) and Applications monitor thier specific queue
to determine if a new instruction has been passed to it.

#####Creating an ICommand#####

An ICommand a command that runs in the thread of it's parent IApp's thread. ICommands are expected to be quick and set or 
display information from the application.

ICommand objects inherit from the ICommand interface. The following references are expected in an ICommand:

```C#
string Help { get; }
```
The help text required for help information.

```C#
string Keyword { get; }
```
Keyword used to start this ICommand (or IApp).

```C#
CommandType Type { get; }
```
A CommandType is a enum that can be set as Command or App. It is used to determine if this is an Application or a Command.
ICommand objects are always Command.
```C#
CommandType Type => CommandType.Command;
```

```C#
void Execute(object sender, string input)
```
This is the method that will be executed whether this is an ICommand or an IApp. All calls to methods added will come from here.
```C# object sender``` is expected to be the parent application that this ICommand is defined in.
```C# string input``` is expected to be the raw command input from the CommandConsole interface.

#####Creating an IApp#####

IApps inherit from the IApp interface which also inherits from the ICommand interface.
**All required fields of ICommand are required in an IApp** plus more.

```C#
string Name { get; }
```
This is a display name used to help the user interact with the tasks.

```C#
string TaskID { get; set; }
```
This is the unique TaskID set for each task. CommandEngine always has a TaskID of 1.

```C#
CommandLibrary SubCommands { get; set; }
```
CommandLibrary is a container for a Dictionary of ICommands with their Keywords used as a reference.

```C#
void Log();
```
This method is used for logging to a file.

```C#
string Status { get; }
```
This stores the current status of the IApp.

```C#
bool StopApp { get; set; }
```
This is used to stop the application. Support must be built into your Application loop, but once set to true the app will end on the next cycle.

Example
```C#
public void Execute(object sender, string c)
{
	while(!StopApp)
	{
		
	}
}
```

#####Using CommandEngine as a template#####
