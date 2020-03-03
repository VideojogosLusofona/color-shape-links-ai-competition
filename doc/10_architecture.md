# Framework architecture {#architecture}

@brief A guide on how to test an AI thinker in the console

[TOC]

## Overview

The ColorShapeLinks development framework is organized as follows (projects
shown in bold):

* [ConsoleApp] - Console application solution folder.
  * [ConsoleApp.sln] - Solution file.
  * [ColorShapeLinks] - Folder containing all projects required by the
    console app.
    * [**Common**][Common] - Common code shared with the Unity implementation
      (.NET Standard 2.0 class library).
      * [AI] - AI-specific common code.
        * [Examples] - Example AI thinkers.
      * [Session] - Common match and session handling code.
    * [TextBased] - Console app folder.
      * [**App**][App] - The console app (.NET Core 3.1 console app).
      * [**Lib**][Lib] - UI-independent library code for running matches and
        sessions (.NET Standard 2.0 class library).
* [**UnityApp**][UnityApp] - Unity application solution and project folder.

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
