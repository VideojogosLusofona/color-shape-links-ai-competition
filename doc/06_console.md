# Console guide {#console-guide}

@brief A guide on how to run an AI thinker using the console app

[TOC]

@remark All instructions in these guides are cross-platform and work in Linux, Windows, and macOS, requiring only that [.NET Core 3.1][.NET Core] is
installed. On Windows, when not using Git Bash, replace slashes `/` with
backslashes `\` when referencing local paths.

## Running the app

In order to run the console app, `cd` into its folder (i.e.,
`ConsoleApp/ColorShapeLinks/TextBased/App`) and run the following command:

```
$ dotnet run
```

The console app can also be invoked from other folders. For example, consider
the following folder structure (as discussed
[in the previous section](@ref devenv)):

```
└──color-shape-links-ai-dev/
   ├──color-shape-links-ai-competition/
   └──my-ai-solution/
```

If we're developing our thinker in the `my-ai-solution` folder, we could invoke
the console app using the [`-p` option of the `dotnet run` command][dotnet-run],
as follows:

```
$ dotnet run -p ../color-shape-links-ai-competition/ConsoleApp/ColorShapeLinks/TextBased/App
```

In either case, since no options were passed to the app, it will terminate with
an error message, although showing the main options. By default the app is built
and executed in _debug_ mode. To build and run it in _release_ mode, use the [following command instead][dotnet-run]:

```
$ dotnet run -c Release
```

Command-line arguments are passed to app after two dashes, `--`, since these
separate options to the `dotnet` command from the options to the app being
executed. For example, the `info` option shows important environment info
for running ColorShapeLinks matches, such as known assemblies (units of
compiled C# code) and AI thinkers. Running the console app with this option
can be accomplished as follows:

```
$ dotnet run -c Release -- info
```

Alternatively, the app can be [built][dotnet-build] and executed separately:

```
$ dotnet build -c Release
$ ./bin/Release/netcoreapp3.1/ColorShapeLinks.TextBased.App
```

In this case, command-line options are passed directly:

```
$ ./bin/Release/netcoreapp3.1/ColorShapeLinks.TextBased.App info
```

For the remainder of this section we'll consider that the `dotnet run` command
in executed within the `ConsoleApp/ColorShapeLinks/TextBased/App` folder and
using the default _debug_ mode (i.e. no need for the `-p` and `-c` options,
respectively).

### App options

The console application has the following main options (or verbs):

| Option/Verb | Description                                              |
| ----------- | -------------------------------------------------------- |
| `session`   | Run a complete session (tournament) between AIs.         |
| `match`     | Run a single match between two thinkers.                 |
| `info`      | Show known assemblies, thinkers and listeners, and exit. |
| `help`      | Display more information on a specific command.          |

There are two sub-options available for **all** verbs:

* `-a`, `--assemblies`: Load .NET Standard 2.0 assemblies containing thinkers
  and/or listeners (space separated).
* `-d`, `--debug`: Enable debug mode, which shows exception stack traces in
  case an error occurs.

The former option is essential for loading custom AI thinkers. To check if the
AI thinkers (or custom @ref listeners "listeners") are in fact loaded, the
option can be used together with the `info` verb, e.g.:

```
$ dotnet run -- info --assemblies /full/path/to/assembly.dll
```

The `match` and `session` verbs run one or more matches, respectively, and
share the following sub-options:

* `-r`, `--rows`: Number of rows in game board (default is 6).
* `-c`, `--cols`: Number of columns in game board (default is 7).
* `-w`, `--win-sequence`: How many pieces in sequence to win (default is 4).
* `-o`, `--round-pieces`: Number of initial round pieces per player
  (default is 10).
* `-s`, `--square-pieces`: Number of initial square pieces per player
  (default is 11).
* `-t`, `--time-limit`: Time limit (in milliseconds) for making move (default
  is 3600000, i.e., one hour).
* `-m`, `--min-time`: Minimum apparent time (in milliseconds) between moves
  (default is 0).
* `--thinker-listeners`: Thinker event listeners, space separated (default is
  ColorShapeLinks.TextBased.App.SimpleRenderingListener).
* `--match-listeners`: Match event listeners, space separated (default is
  ColorShapeLinks.TextBased.App.SimpleRenderingListener).

These options define the board configuration for matches, the maximum and
minimum apparent play times, as well as alternative thinker and match
@ref listeners "listeners".

### The match verb

The `match` verb runs a single match of ColorShapeLinks. The following command
lists all `match` verb options:

```
$ dotnet run -- help match
```

Of these, the following are `match`-specific:

* `-W`, `--white`: Fully qualified name of player 1 thinker class (default is
  ColorShapeLinks.TextBased.App.HumanThinker).
* `-R`, `--red`: Fully qualified name of player 2 thinker class (default is
  ColorShapeLinks.TextBased.App.HumanThinker).
* `--white-params`: Parameters for setting up player 1 thinker instance (no
  parameters are passed by default).
* `--red-params`: Parameters for setting up player 2 thinker instance (no
  parameters are passed by default).

As such, invoking a match without any parameters will result in a match between
two human players, which selected by default, i.e.:

```
$ dotnet run -- match
```

The following command runs a match between a human player, playing as white,
and a random move AI player:

```
$ dotnet run -- match -R ColorShapeLinks.Common.AI.Examples.RandomAIThinker
```

If a third-party AI thinker named `MyAISolution.MyAI.MyThinker` (in file
`MyThinker.cs`) is placed in the `ConsoleApp/ColorShapeLinks/TextBased/App`
folder, the following command runs a match between `MyThinker`, playing as
white, and a random move player, playing as red, with a time limit of one
second:

```
dotnet run -- match -W MyAISolution.MyAI.MyThinker -R ColorShapeLinks.Common.AI.Examples.RandomAIThinker -t 1000
```

If the AIs are playing too fast for human observation, a minimum apparent play
time can be set, for example two seconds:

```
dotnet run -- match -W MyAISolution.MyAI.MyThinker -R ColorShapeLinks.Common.AI.Examples.RandomAIThinker -t 1000 -m 2000
```

In case the third-party AI thinker is developed outside the ColorShapeLinks
framework, which is [the preferred way to do development anyway](@ref devenv),
its assembly can be specified with the `--assemblies` (or `-a`) option:

```
dotnet run -- match -W MyAISolution.MyAI.MyThinker -R ColorShapeLinks.Common.AI.Examples.RandomAIThinker -t 1000 -m 2000 --assemblies /full/path/to/my/thinker/assembly.dll
```

If an AI thinker requires additional configuration parameters, these can be
specified with the `--white-params`/`--red-params` options. For example,
perhaps the `MyThinker` AI could accept a search depth parameter. Such a
parameter could be passed as follows (note the thinkers themselves are
responsible for parsing the string containing these parameters):

```
dotnet run -- match -W MyAISolution.MyAI.MyThinker -R ColorShapeLinks.Common.AI.Examples.RandomAIThinker --white-params "depth=4"
```

Different board configurations can also be specified. The following command
starts a match between two human players with a 10x10 board, a winning sequence
of 7 pieces, with each player starting with 24 round pieces and 25 square
pieces:

```
dotnet run -- match -r 10 -c 10 -w 7 -o 24 -s 25
```

### The session verb

The `session` verb runs a complete session/tournament between the AI thinkers
specified in a configuration file. The following command list all `session`
verb options:

```
$ dotnet run -- help session
```

Of these, the following are `session` specific:

* `--points-per-win`: Points per win (default is 3).
* `--points-per-loss`: Points per loss (default is 0).
* `--points-per-draw`: Points per draw (default is 1).
* `-g`, `--config`: Mandatory option which specifies the session configuration
  file.
* `--session-listeners`: Session event listeners, space separated (default is
  ColorShapeLinks.TextBased.App.SimpleRenderingListener).

The most important of these options is `-g`, or `--config`, which accepts a
session configuration file. Each line in this file should specify the
participating thinker fully qualified name, followed by its specific
parameters, if any, as shown in the following example:

```
# Lines starting with # are ignored
# Blank lines are also ignored
ColorShapeLinks.Common.AI.Examples.RandomAIThinker
MyAISolution.MyAI.MyThinker depth=5
Awesome.AwesomeAIThinker depth=7,turbo=boost,win=always
```

If this file is named "test-competition.txt", a complete session/tournament
between the specified AI thinkers can be started as follows:

```
$ dotnet run -- session -g test-competition.txt
```

If any of the AI thinkers are in external assemblies, these also need to be
specified, for example:

```
$ dotnet run -- session -g test-competition.txt -a /full/path/to/my/thinker/assembly.dll /full/path/to/awesome/assembly.dll
```

### Exit codes and capturing results

The console app returns the following exits codes to the operating system,
based on the [ExitStatus](@ref ColorShapeLinks.TextBased.Lib.ExitStatus)
enumeration:

| Code | Description                                            |
| ---- | ------------------------------------------------------ |
| 0    | A `match` was played and ended in a draw.              |
| 1    | A `match` was played, white wins.                      |
| 2    | A `match` was played, red wins.                        |
| 3    | A `session` was executed successfully.                 |
| 4    | An `info` request was executed successfully.           |
| 5    | An exception occurred while executing the console app. |

This exit code allows, for instance, to plug-in the console app to machine
learning frameworks. For example, capturing the exit code with a Python script
could be done as follows:

```py
import subprocess
from pathlib import Path

# Assume the ColorShapeLinks framework is in the source/repos folder in the
# home folder
csl_path = str(Path.home().joinpath("source", "repos",
    "color-shape-links-ai-competition", "ConsoleApp",
    "ColorShapeLinks", "TextBased", "App"))

# Run a match between two of the included thinkers
cp = subprocess.run(["dotnet", "run", "-p", csl_path, "--", "match",
    "-W", "ColorShapeLinks.Common.AI.Examples.RandomAIThinker",
    "-R", "ColorShapeLinks.Common.AI.Examples.MinimaxAIThinker"])

# Show captured exit code
print("Return value is {0}".format(cp.returncode))
```

However, sessions/tournaments are probably more practical for certain machine
learning contexts, for example when using evolutionary algorithms.
While exit codes cannot return `session` results, the
[RankingSessionListener](@ref ColorShapeLinks.TextBased.Lib.RankingSessionListener)
was developed for this purpose. It exports `session` results to a [TSV] file,
allowing for setups in which the machine learning algorithm continuosly defines
the session roster and captures the session's results.

## Event listeners {#listeners}

### Listener architecture

Listeners are responsible for handling:

* @ref ColorShapeLinks.Common.AI.IThinker "Thinker events"
* @ref ColorShapeLinks.TextBased.Lib.IMatchSubject "Match events"
* @ref ColorShapeLinks.TextBased.Lib.ISessionSubject "Session events"

Such listeners are used, for example, for rendering information on the console
in response to these events. However, listeners are not limited to console
rendering, and can be used to process the generated information any way
the developer sees fit.

Listeners can be specified at runtime using the following options, which accept
the listener's fully qualified name:

* `--thinker-listeners`, available to `match` and `session` verbs.
* `--match-listeners`, available to `match` and `session` verbs.
* `--session-listeners`, only available to the `session` verb.

More than one listener can be specified and default listeners can be disabled.
For example, the following command specifies two thinker listeners, while
disabling all match listeners:

```
$ dotnet run -- match --thinker-listeners My.ThinkListnr Other.ThinkListnr --match-listeners ""
```

Known listeners can be listed with the `info` verb:

```
$ dotnet run -- info
```

If listeners are defined in an external assembly, the assembly also needs to be
specified, e.g.:

```
$ dotnet run -- info -a /full/path/to/listener/assembly.dll
```

### The default listener

The
@ref ColorShapeLinks.TextBased.App.SimpleRenderingListener "SimpleRenderingListener"
implements all three listener interfaces, namely
@ref ColorShapeLinks.TextBased.Lib.IThinkerListener "IThinkerListener",
@ref ColorShapeLinks.TextBased.Lib.IMatchListener "IMatchListener" and
@ref ColorShapeLinks.TextBased.Lib.ISessionListener "ISessionListener". As
such, it is able to listen to
@ref ColorShapeLinks.Common.AI.IThinker "thinker",
@ref ColorShapeLinks.TextBased.Lib.IMatchSubject "match" and
@ref ColorShapeLinks.TextBased.Lib.ISessionSubject "session" events, and is
used as the default listener in the configurations described in the following
paragraphs.

When running simple matches (i.e.,
using the `match` verb), the
@ref ColorShapeLinks.TextBased.App.SimpleRenderingListener "SimpleRenderingListener"
is registered as a
@ref ColorShapeLinks.TextBased.Lib.IThinkerListener "thinker listener" and a
@ref ColorShapeLinks.TextBased.Lib.IMatchListener "match listener".
There are no
@ref ColorShapeLinks.TextBased.Lib.ISessionSubject "session events" when
running simple matches.

Similarly,
@ref ColorShapeLinks.TextBased.App.SimpleRenderingListener "SimpleRenderingListener"
is registered as a
@ref ColorShapeLinks.TextBased.Lib.ISessionListener "session listener" when
running complete sessions/tournaments with the `session` verb. By default, no
@ref ColorShapeLinks.TextBased.Lib.IThinkerListener "thinker" and
@ref ColorShapeLinks.TextBased.Lib.IMatchListener "match" listeners are
registered during complete sessions.

The
@ref ColorShapeLinks.TextBased.App.SimpleRenderingListener "SimpleRenderingListener"
works according to its name, outputting
@ref ColorShapeLinks.Common.AI.IThinker "thinker",
@ref ColorShapeLinks.TextBased.Lib.IMatchSubject "match" and
@ref ColorShapeLinks.TextBased.Lib.ISessionSubject "session" events
to the console in the simplest possible fashion.

### Custom listeners

A custom listener can listen to several events of one or more types. Depending
on the types of event to listen, the custom listener must implement one or more
of the following interfaces:

* @ref ColorShapeLinks.TextBased.Lib.IThinkerListener "IThinkerListener" for
  listening to @ref ColorShapeLinks.Common.AI.IThinker "thinker events".
* @ref ColorShapeLinks.TextBased.Lib.IMatchListener "IMatchListener" for
  listening to @ref ColorShapeLinks.TextBased.Lib.IMatchSubject "match events".
* @ref ColorShapeLinks.TextBased.Lib.ISessionListener "ISessionListener" for
  listening to
  @ref ColorShapeLinks.TextBased.Lib.ISessionSubject "session events".

As an example, we'll show how to implement a listener to output information
about all the moves performed in a match to a CSV file which can be opened in
a spreadsheet.

First, create a project folder, `cd` into the folder and create a new class
library project, removing the temporary C# class that is created:

```
$ mkdir CSVListeners
$ cd CSVListeners
$ dotnet new classlib -f netstandard2.0
$ rm Class1.cs
```

The new project needs to reference the ColorShapeLinks.Common and
ColorShapeLinks.TextBased.Lib projects:

```
$ dotnet add reference /path/to/color-shape-links-ai-competition/ConsoleApp/ColorShapeLinks/Common
$ dotnet add reference /path/to/color-shape-links-ai-competition/ConsoleApp/ColorShapeLinks/TextBased/Lib
```

Open your favorite IDE and create a new class named for example
`MatchListener`, which should appear as follows:

```cs
using System;

namespace CSVListeners
{
    public class MatchListener
    {
    }
}
```

We don't need the `System` namespace, but we do need
the ColorShapeLinks.TextBased.Lib namespace. Furthermore, our `MatchListener`
must implement `IMatchListener`:

```cs
using ColorShapeLinks.TextBased.Lib;

namespace CSVListeners
{
    public class MatchListener : IMatchListener
    {
        public void ListenTo(IMatchSubject subject)
        {

        }
    }
}
```

Since our goal is to write information about all moves performed in a match to
a CSV file, we need to:

1. Listen for the start of the match, determine the file name and open the file
   for writing.
2. Listen for moves performed, and write information about them to the file.
3. Listen for the end of the match and close the file.

Thus, we need to create methods for each of these actions, and register them
with the appropriate events, namely
@ref ColorShapeLinks.TextBased.Lib.IMatchSubject.MatchStart "MatchStart",
@ref ColorShapeLinks.TextBased.Lib.IMatchSubject.MatchStart "MovePerformed" and
@ref ColorShapeLinks.TextBased.Lib.IMatchSubject.MatchStart "MatchOver". The
skeleton of such code can be as follows:

```cs
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;
using ColorShapeLinks.TextBased.Lib;
using System.Collections.Generic;

namespace CSVListeners
{
    public class MatchListener : IMatchListener
    {
        public void ListenTo(IMatchSubject subject)
        {
            subject.MatchStart += OpenFile;
            subject.MovePerformed += WriteToFile;
            subject.MatchOver += CloseFile;
        }

        private void OpenFile(IMatchConfig config, IList<string> thinkers)
        {

        }

        private void WriteToFile(PColor color, string thinker, FutureMove fm, int moveDuration)
        {

        }

        private void CloseFile(Winner winner, ICollection<Pos> solution, IList<string> thinkers)
        {

        }
    }
}
```

We need to add some code to these methods:

```cs
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;
using ColorShapeLinks.TextBased.Lib;
using System.Collections.Generic;
using System.IO;

namespace CSVListeners
{
    public class MatchListener : IMatchListener
    {
        private StreamWriter file;

        public void ListenTo(IMatchSubject subject)
        {
            subject.MatchStart += OpenFile;
            subject.MovePerformed += WriteToFile;
            subject.MatchOver += CloseFile;
        }

        private void OpenFile(IMatchConfig config, IList<string> thinkers)
        {
            // Use the thinker names as the basis for the filename
            file = new StreamWriter($"{thinkers[0]}vs{thinkers[1]}.csv");

            // Write the header
            file.WriteLine("\"Thinker\",\"Color\",\"Shape\",\"Column\",\"Think time (ms)\"");
        }

        private void WriteToFile(PColor color, string thinker, FutureMove fm, int thinkTime)
        {
            file.WriteLine($"\"{thinker}\",\"{color}\",\"{fm.shape}\",{fm.column},{thinkTime}");
        }

        private void CloseFile(Winner winner, ICollection<Pos> solution, IList<string> thinkers)
        {
            file.Close();
        }
    }
}
```

Now build the project:

```
$ dotnet build
```

Open another terminal and `cd` into the
`ConsoleApp/ColorShapeLinks/TextBased/App` folder. Run the console app
specifying our custom match listener and disabling the default thinker
listener, running a match between a sequential AI player and a basic (but not
completely dumb) AI. Note we need to specify the assembly containing our
custom listener:

```
$ dotnet run -- match -W ColorShapeLinks.Common.AI.Examples.SequentialAIThinker -R ColorShapeLinks.Common.AI.Examples.MinimaxAIThinker --thinker-listeners "" --match-listeners CSVListeners.MatchListener -a /path/to/CSVListeners/bin/Debug/netstandard2.0/CSVListeners.dll
```

Nothing appears on screen, since the only enabled listener is our custom one,
which only outputs content to a file. If everything goes well, a file named
`SequentialvsMinimaxD3.csv` will appear in the app folder. Opening or importing
the file with LibreOffice Calc or Microsoft Excel yields something similar to:

![image](https://user-images.githubusercontent.com/3018963/113517844-5afcd580-957a-11eb-9c51-a0661df1123c.png)

The process of writing other types of listener is similar, requiring only a
knowledge of what events are produced by
@ref ColorShapeLinks.Common.AI.IThinker "thinkers",
@ref ColorShapeLinks.TextBased.Lib.IMatchSubject "matches" and
@ref ColorShapeLinks.TextBased.Lib.ISessionSubject "sessions".

[.NET Core]:https://dotnet.microsoft.com/download
[Unity]:https://unity.com/
[dotnet-run]:https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run#options
[dotnet-build]:https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build
[TSV]:https://en.wikipedia.org/wiki/Tab-separated_values