# Console guide {#console-guide}

@brief A guide on how to test an AI thinker in the console

[TOC]

## Overview

The ColorShapeLinks console application is organized as follows (.NET projects
shown in bold):

* [ConsoleApp] - Solution folder.
  * [ConsoleApp.sln] - Solution file.
  * [ColorShapeLinks] - Folder containing all projects required by the
    console app.
    * [**Common**][Common] - Common code shared with the Unity implementation (.NET
      Standard 2.0 class library).
      * [AI] - AI-specific common code.
        * [Examples] - Example AI thinkers.
      * [Session] - Common match and session handling code.
    * [TextBased] - Console app folder.
      * [**App**][App] - The console app (.NET Core 3.1 console app).
      * [**Lib**][Lib] - UI-independent library code for running matches and
        sessions (.NET Standard 2.0 class library).

Namespaces follow the folder organization. For example, code in the
[Session] folder is under the ColorShapeLinks.Common.Session namespace.
Dependencies between the projects are as follows (additional relations, in
gray, are shown for context):

@dot
digraph example {
  rankdir="LR";
  node [ shape=tab, fontname=Helvetica, fontsize=10 ];
  edge [ style="dashed" ];
  Common   [ label="Common", URL="@ref ColorShapeLinks.Common"];
  App      [ label="App", URL="@ref ColorShapeLinks.TextBased.App"];
  Lib      [ label="Lib", URL="@ref ColorShapeLinks.TextBased.Lib"];
  UnityApp [ label="UnityApp", URL="@ref ColorShapeLinks.UnityApp",
             color="slategray", fontcolor="slategray"];
  CLParser [ label="CommandLineParser", URL="https://github.com/commandlineparser/commandline",
             color="slategray", fontcolor="slategray"];
  App -> Common;
  App -> Lib   ;
  CLParser -> App [ dir="back", color="slategray" ];
  Lib -> Common;
  Common -> UnityApp [ dir="back", color="slategray" ];
}
@enddot

## The console application

<!-- Summary of most important console app commands and how to run info, matches and
sessions.-->

### Executing the app

In order to run the console app, `cd` into its folder and run the following
command:

```
$ dotnet run
```

Since no options were specified, the app will exit with an error message,
although showing the main options. By default the app is built and executed in
_debug_ mode. To build and run it in _release_ mode, use the following
command instead:

```
$ dotnet run -c Release
```

Command-line arguments are passed to app after two dashes, `--`, since these
separate options to the `dotnet` command from the options to the app being
executed. For example, the `info` option shows important environment info
for running ColorShapeLinks matches, for example known assemblies (units of
compiled C# code) and AI thinkers. Running the console app with this options
can be accomplished as follows:

```
$ dotnet run -c Release -- info
```

Alternatively, the app can be build and executed separately:

```
$ dotnet build -c Release
$ ./bin/Release/netcoreapp3.1/ColorShapeLinks.TextBased.App
```

In this case, command-line options are passed directly:

```
$ ./bin/Release/netcoreapp3.1/ColorShapeLinks.TextBased.App info
```

For the remainder of this document, we will use the first approach, i.e.,
the `dotnet run` command.

### App options

The console applications has the following main options (or verbs):

| Option/Verb | Description                                             |
| ----------- | ------------------------------------------------------- |
| `session`   | Run a complete session (tournament) between AIs         |
| `match`     | Run a single match between two thinkers                 |
| `info`      | Show known assemblies, thinkers and listeners, and exit |
| `help`      | Display more information on a specific command          |

There are two sub-options available for all verbs:

* `-a`, `--assemblies`: Load .NET Standard 2.0 assemblies containing thinkers
  and/or listeners (space separated).
* `-d`, `--debug`: Enable debug mode, which shows exception stack traces in
  case an error occurs.

The former option is essential for loading custom AI thinkers. To check if the
AI thinkers (or custom @ref listeners "listeners") are in fact loaded, the
option can be used together with the `info` verb, e.g.:

```
$ dotnet run -c Release -- info --assemblies /full/path/to/assembly.dll
```

### The match verb

The `match` verb runs a single match of ColorShapeLinks. The following command
list all `match` verb options:

```
$ dotnet run -c Release -- help match
```

TODO: Rest of section

### The session verb

The `session` verb runs a complete session/tournament between the AI thinkers
specified in a configuration file. The following command list all `session`
verb options:

```
$ dotnet run -c Release -- help session
```

TODO: Rest of section

## Developing an AI thinker

TODO: The quick way is already explained. Here we discuss proper development,
with separate folder/repo.

TODO: Run match/session with external assembly/thinker.

## Default and custom listeners {#listeners}

TODO: Discuss custom listeners

## Testing the AI thinker in isolation

TODO: Warn about thinker variables not being available when thinker is
instantiated directly.

[ConsoleApp]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/ConsoleApp
[ConsoleApp.sln]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/blob/master/ConsoleApp/ConsoleApp.sln
[ColorShapeLinks]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/ConsoleApp/ColorShapeLinks
[Common]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/common
[AI]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/common/AI
[Examples]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/common/AI/Examples
[Session]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/common/Session
[TextBased]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/ConsoleApp/ColorShapeLinks/TextBased
[App]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/ConsoleApp/ColorShapeLinks/TextBased/App
[Lib]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/ConsoleApp/ColorShapeLinks/TextBased/Lib
[UnityApp]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/UnityApp
