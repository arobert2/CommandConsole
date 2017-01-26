# CommandConsole #
#### Real name TBA. ####

CommandConsole is a C# powered multithreaded application interface implementing WPF.

###Usage###

CommandConsole uses .Net 4.6.2

To use the console simply compile and run. To get started type listcommands.

###Extending The CommandConsole###

####How It Works####

All commands in the CommandConsole are classes derived from either ICommand or IApp, which IApp is derived from ICommand.
ICommand objects run in the thread their parent IApp object is running in while IApp objects run in their own thread.
Commands are passed to a queue specific to each task ```TaskSystem.CommandQueue``` and Applications check thier specific queue
each loop to determine if a new instruction has been passed to it.

####Creating an ICommand####

An ICommand is a command that runs in the thread of it's parent IApp. ICommands are expected to be quick and used to display information or set IApp internal information.

ICommand objects inherit from the ICommand interface. The following references are expected in an ICommand:

```C#
string Help { get; }
```
The help text. When building off of CommandGenerator

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
```object sender``` is expected to be the parent application that this ICommand is defined in.
```string input``` is expected to be the raw command input from the CommandConsole interface.

####Creating an IApp####

IApps inherit from the IApp interface which also inherits from the ICommand interface.
**All required fields of ICommand are required in an IApp** plus more.

```C#
string Name { get; }
```
This is a display name used to help the user interact with the tasks.

```C#
string TaskID { get; set; }
```
This is the unique TaskID set for each task. It is increased by one everytime an object that implements AppTemplate is created.

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

```C#
IApp Clone();
```
Returns a copy of this IApp except it has an updated TaskID. Used for launching classes.


####Creating an Application from AppTemplate.####
This is the recommended way of building an application.

You can derive from AppTemplate to create a preconfigured IApp. Deriving from AppTemplate will include default commands that
are useable by all IApp derived classes.

The commands that are inherited are:
help -  prints the help text for the current IApp you are interacting with.

listcommands - lists all available commands that the current IApp you are interacting with has in his SubCommands CommandLibrary.

printinfo - prints the TaskID and Name of the current active IApp.

exit - shuts down the current IApp you are interacting with and returns you to the top level CommandEngine.

In order to create your own IApp you'll need to override several required fields and methods defined in AppTemplate.

The following should be overridden:
```C#
public string Name { get; }
public string Help { get; }
public string Keyword { get; }
public void Execute();
public void Log();
public IApp Clone();
```
####Loading My New Commands####

Once you have completed your IApp, you will need to add it to the top level CommandLibrary which is AppTemplate's 
SubCommands CommandLibrary. To do this there is a singleton partial class implementation of the CommandLibrary called LoadApps which 
includes a partial method called LoadLibrary(). Create a new partial class LoadApps and within it create a new partial method void LoadLibrary().
Within LoadLibrary add your command with the following syntax ```AppsToLoad.Library.Add(keyword, new Application());``` where keyword is a string
of the keyword used to call it and new Application() is a new instantiation of your IApp.

Now compile and try calling your new IApp.
