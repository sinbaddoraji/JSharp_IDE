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
  The java debugger should be able to identify breakpoints -> Failed
  The java debugger should be able to step into code (line by line)
  
*JSharp should be themeable

Resources Needed to run JSharp:

  Microsoft .net framework 4.8
  Java development kit version 1.8.0_251 or higher
  2GB ram or more
  70MB hard disk space (Minimum)

How to Build:
  Get JNI4Net dlls from http://jni4net.com/ and refrence them in the project
  Build


Frameworks Used:

Avalon Dock => Used for docking 
Avalon Edit => Used for text editor 
Jni4net => Used for running java code from c#
MahApps.Metro => Used to create the window

Notes:

JSharp was initially written in .net core but it was rewritten in the normal .net framework to make it work on other platforms using mono
