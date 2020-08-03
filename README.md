This read me file is currently being used as a milestone tracker. Things marked with "*" are currently implemented features.

# JSharp => A minimalist java IDE written in c# as possible replacement IDE for minimalist coders

*JSharp should have a flexible User Interface like Visual Studio.

*JSharp should have a text editor with Syntax Highlighting

*JSharp should support auto complete for Java

*JSharp should have a plugin 

  A plugin should be able to add menu items to the JSharp window
  
  A plugin shpuld be able to add toolbar objects to the JSharp window
  
  A plugin should be able to handle some JSharp window events
  
  A plugin should be able to add context menu items

JSharp should have documentation for plugin development

*JSharp should be fast and easy to use

*JSharp should be able to compile java files without the need for projects

*JSharp should have a java debugger 
  The java debugger should be able to identify breakpoints -> Yet to implement
  The java debugger should be able to step into code (line by line)
  
*JSharp should be themeable

Resources Needed to run JSharp:

  Microsoft .net framework 4.8
  Java development kit version 1.8.0_251 or higher
  2GB ram or more
  70MB hard disk space (Minimum)

How to Build:
  Build with visual studio. No complicated process is needed to build JSharp.

Frameworks Used:

Avalon Dock => Used for docking 
Avalon Edit => Used for text editor 
IKVM => Used for running java code from c#
MahApps.Metro => Used to create the window

Notes:

JSharp was initially written in .net core but it was rewritten in the normal .net framework to make it work on other platforms using mono

On Plugins:

Plugins were implemented in a very straight forward way. C# reflection is used to extract data from the dll files.

A plugin project should refrence the JSharp project or "JSharp.exe" which needs all the dll files in the directory to run. It is done this way so that the plugin has direct access to the core of JSharp and vice versa.

The plugin should have one major file called "Entry.cs" which inherits IPlugin which is found in the JSharp lib. Implementing the Interface in any way desired should create a functional plugin as long as there are no build-errors or noticable bugs.

On auto-complete:

The base which this was built on is the avalon-edit framework and IKVM. The completion window provided by avalon-edit is modified and used to display data extracted from code written using the Java reflection libruary to suggest java code (a java libruary used to analyse Java code that has already been built).

On the debugger:

The debugger for JSharp is based on the java-dt graphical debugger. Further work has not been done on the debugger yet. The plan is to pick up where the example program left off and continue from there as the debugger is currently very basic

On Syntax highlighting:
 Syntax highlighting was acchieved using avalon-edit. Custom highlighting schemes in xshd (an xml derivative) were created for JSharp. Once the custom schemes were made, classes to use them were implemented. The original Higlighting manager was not used because JSharp has a light and dark mode and highlighting colour schemes need to change as the theme changes

 Graphics/Front-end:
 MahApps.Metro and Avalon dock were used to give JSharp the clean look it has.