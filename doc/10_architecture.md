# Framework architecture {#architecture}

@brief Architecture of the ColorShapeLinks development framework

[TOC]

## Overview

The ColorShapeLinks development framework offers two application frontends:
the @ref console-guide "ConsoleApp" and the @ref unity-guide "UnityApp".
However, the framework is internally composed of four .NET projects, namely
@ref ColorShapeLinks.Common "Common", @ref ColorShapeLinks.TextBased.App "App",
@ref ColorShapeLinks.TextBased.Lib "Lib" and
@ref ColorShapeLinks.UnityApp "UnityApp", as shown in Figure 1.

@dot "Figure 1. Internal framework organization. Arrows represent dependencies between projects."
digraph example {
  rankdir="LR";
  node [ shape=tab, fontname=Helvetica, fontsize=10 ];
  edge [ style="dashed" ];
  Common   [ label="Common", URL="@ref ColorShapeLinks.Common", style=filled, fillcolor=Chartreuse];
  CLParser [ label="CommandLineParser", URL="https://github.com/commandlineparser/commandline",
             color="slategray", fontcolor="slategray"];

  subgraph cluster_0 {
    label="ConsoleApp frontend";
    URL="@ref console-guide";
    fontname=Helvetica;
    fontsize=10;
    bgcolor=mintcream;
    shape=tab;
    App      [ label="App", URL="@ref ColorShapeLinks.TextBased.App", fillcolor=Chartreuse, style=filled ];
    Lib      [ label="Lib", URL="@ref ColorShapeLinks.TextBased.Lib", fillcolor=Chartreuse, style=filled ];
  };

  subgraph cluster_1 {
    label="UnityApp frontend";
    URL="@ref unity-guide";
    fontname=Helvetica;
    fontsize=10;
    bgcolor=mintcream;
    shape=tab;
    UnityApp [ label="UnityApp", URL="@ref ColorShapeLinks.UnityApp", style=filled, fillcolor=Chartreuse ];
  };

  App -> Common;
  App -> Lib   ;
  CLParser -> App [ dir="back", color="slategray" ];
  Lib -> Common;
  Common -> UnityApp [ dir="back" ];
}
@enddot

## The Common project

The @ref ColorShapeLinks.Common "Common" project is a .NET Standard 2.0 class
library which constitutes the core of the framework. It defines fundamental
models of the ColorShapeLinks game (i.e., of the unbounded Simplexity
game), such as the @ref ColorShapeLinks.Common.Board "board", its
@ref ColorShapeLinks.Common.Piece "pieces" or performed
@ref ColorShapeLinks.Common.Move "moves", and is a dependency of the remaining
projects. It is further subdivided in the @ref ColorShapeLinks.Common.AI "AI"
and @ref ColorShapeLinks.Common.Session "Session" namespaces. The former
defines AI-related abstractions, such as the
@ref ColorShapeLinks.Common.AI.AbstractThinker "AbstractThinker" class, which
other AI thinkers must extend, as well as a manager for finding and
instantiating concrete AI thinkers. The latter specifies a number of match and
session-related interfaces, as well as concrete
@ref ColorShapeLinks.Common.Session.Match "match" and
@ref ColorShapeLinks.Common.Session.Session "session" models.

The @ref ColorShapeLinks.Common "Common" project is hosted
[in its own branch][Common] branch of the Git repository, and is
included as a [submodule](https://git-scm.com/book/en/v2/Git-Tools-Submodules)
in both the [ConsoleApp] and [UnityApp] folders of the `master` branch,
which is why the framework needs to be cloned with the `--recurse-submodules`
Git clone option.

The folder structure of the @ref ColorShapeLinks.Common "Common" project is
organized as follows:

* [**Common**][Common] - Board, pieces, moves, etc.
  * [AI] - AI-related abstractions and management of AI thinkers.
    * [Examples] - Example AI thinkers.
  * [Session] - Match and session classes and related interfaces.

Namespaces follow the folder organization. For example, code in the
[Session] folder is under the ColorShapeLinks.Common.Session namespace.

## The ConsoleApp projects

The @ref console-guide "ConsoleApp" is composed of two projects,
@ref ColorShapeLinks.TextBased.App "App" and
@ref ColorShapeLinks.TextBased.Lib "Lib", both of which depend on the
@ref ColorShapeLinks.Common "Common" class library, as shown in Figure 1. The
@ref ColorShapeLinks.TextBased.App "App" project is a .NET Core console
application with an external dependency on the [CommandLineParser] library
and an internal
dependency on the @ref ColorShapeLinks.TextBased.Lib "Lib" project – itself
a .NET Standard 2.0 class library. The @ref ColorShapeLinks.TextBased.App "App"
project provides the actual console frontend, namely the text user interface
(TUI) with which the user interacts in order to run ColorShapeLinks matches and
sessions.

The @ref ColorShapeLinks.TextBased.Lib "Lib" class library acts as an
UI-independent "game engine", offering
@ref ColorShapeLinks.TextBased.Lib.MatchController "match" and
@ref ColorShapeLinks.TextBased.Lib.SessionController "session controllers", as
well as interfaces for the associated event system, allowing to plug-in
renderers (_views_, in [MVC] parlance) or other event handling code at runtime.
It serves as a middleware between
the @ref ColorShapeLinks.Common "Common" library and frontend applications,
such as the one implemented in the @ref ColorShapeLinks.TextBased.App "App"
project. It is not used by the @ref unity-guide "Unity implementation", since
Unity already provides its own game engine logic, forcing
@ref ColorShapeLinks.UnityApp.MatchController "match" and
@ref ColorShapeLinks.UnityApp.SessionController "session" controllers to
be tightly integrated with its way of doing things. Nonetheless, the
@ref ColorShapeLinks.TextBased.Lib "Lib" class library makes the
creation of new ColorShapeLinks TUIs or GUIs very simple, as long as they're
not based on highly prescriptive frameworks such as Unity.

The @ref console-guide "ConsoleApp" projects are organized as follows:

* [ConsoleApp] - Console application solution folder.
  * [ConsoleApp.sln] - Solution file.
  * [ColorShapeLinks] - Folder containing all projects required by the
    console app.
    * [**Common**][Common] - Reference to the Common project branch.
    * [TextBased] - Console app folder.
      * [**App**][App] - The console app (.NET Core 3.1 console app).
      * [**Lib**][Lib] - UI-independent library code for running matches and
        sessions (.NET Standard 2.0 class library).

As in the case of the @ref ColorShapeLinks.Common "Common" project, namespaces
follow folder organization. For example, code in the
[Lib] folder is under the ColorShapeLinks.TextBased.Lib namespace.

## The UnityApp project

The @ref unity-guide "UnityApp" is a ColorShapeLinks frontend implemented in
the [Unity] game engine. Like the @ref console-guide "ConsoleApp", it is
designed around the [MVC] pattern, making use of the models provided by the
@ref ColorShapeLinks.Common "Common" library. In this case, however, the views
and controllers are tightly integrated with the Unity engine. The
@ref unity-guide "UnityApp" project is organized as follows:

* [**UnityApp**][UnityApp] - Unity application solution and project folder.
  * _Other Unity folders not relevant for this discussion_.
  * [Assets] - Project assets, such as code and textures.
    * _Other asset folders not relevant for this discussion_.
    * [Scripts] - Code assets.
      * [**Common**][Common] -  Reference to the Common project branch.
      * [UnityApp][UnityAppCode] - Unity app source code.

[CommandLineParser]:https://github.com/commandlineparser/commandline
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
[UnityAppCode]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/UnityApp/Assets/Scripts/UnityApp
[Assets]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/UnityApp/Assets
[Scripts]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/UnityApp/Assets/Scripts
[Unity]:https://unity.com/
[MVC]:https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller
