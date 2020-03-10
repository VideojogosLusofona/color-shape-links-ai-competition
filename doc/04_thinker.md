# Thinker implementation guide {#thinker-implementation-guide}

@brief A guide on how to implement an AI thinker

[TOC]

## Introduction

This guide describes how to implement an AI thinker for the ColorShapeLinks
competition. This is a simple process, requiring at the bare minimum
the creation of a class which extends [another class][AbstractThinker]
and overrides [one of its methods][Think()]. The @ref quick-start "Quick start"
guide shows how to get started quickly by simply dropping the new AI class in
the [console app][console_folder] or [Unity app][unity_folder] folders.
However, implementing a competitive AI thinker will most likely require to
know a bit more about the @ref rules "source code rules and restriction"
and about the @ref abstractthinker "AbstractThinker base class". Setting up a
proper @ref devenv "development environment" and being able to adequately
@ref testing "test" the developed code is also essential. This guide
closes with a tutorial on how to implement a @ref minimax "basic Minimax AI"
with a very simple heuristic.

## Rules and restrictions for the AI source code {#rules}

The source code of AI thinkers must follow these rules and restrictions:

- Can only use cross-platform [.NET Standard 2.0] API calls in C#.
- Can use additional libraries which abide by these same rules.
- Both the AI code and libraries must be made available under a
  [valid open source license][ossl], although AI codes can be open-sourced
  only after the competition deadline.
- Must run in the same process that invokes it.
- Can be multithreaded and use [`unsafe`] contexts.
- Cannot *think* in its opponent time (e.g., by using a background thread).
- Must acknowledge [cancellation requests][`CancellationToken`], in which case
  it should terminate quickly and in an orderly fashion.
- Can only probe the environment for the number of processor cores. It cannot
  search or use any other information, besides what is already available in the
  [AbstractThinker] class or passed to the [Think()] method, e.g., such as
  using reflection to probe the capabilities of its opponents.
- Cannot use more than 2GB of memory during the course of a match.
- Cannot be more than 250kb in size (including libraries, excluding comments).
- Cannot save or load data from disk.

## The AbstractThinker base class {#abstractthinker}

<!--Describe what information is available in this class, beware of testing it in
isolation, what methods and properties can be overriden-->

The first step to implement an AI thinker is to extend the [AbstractThinker]
base class. This class has three overridable methods, but it's only
mandatory to override one of them, as shown in the following table:

| Method       | Mandatory override? | Purpose                           |
| ------------ | ------------------- | --------------------------------- |
| [Setup()]    | No                  | Setup the AI thinker.             |
| [Think()]    | Yes                 | Select the next move to perform.  |
| [ToString()] | No                  | Return the AI thinker's name.     |

There's also the non-overridable [OnThinkingInfo()] method, which can be
invoked for producing thinking information, mainly for debugging purposes.
In the [Unity] frontend this information is printed on Unity's console, while
in the [console] frontend the information is forwarded to the registered
@ref ColorShapeLinks.TextBased.Lib.IThinkerListener "thinker listeners".

Classes extending [AbstractThinker] also inherit a number of useful read-only
properties, namely board/match configuration properties
(@ref ColorShapeLinks.Common.AI.AbstractThinker.Rows "No. of rows",
@ref ColorShapeLinks.Common.AI.AbstractThinker.Cols "No. of columns",
@ref ColorShapeLinks.Common.AI.AbstractThinker.WinSequence "No. of pieces in sequence to win a game",
@ref ColorShapeLinks.Common.AI.AbstractThinker.RoundsPerPlayer "No. of initial round pieces per player"
and
@ref ColorShapeLinks.Common.AI.AbstractThinker.SquaresPerPlayer "No. of initial square pieces per player")
and the
@ref ColorShapeLinks.Common.AI.AbstractThinker.TimeLimitMillis "time limit for the AI to play".
Concerning the board/match configuration properties, these are also
available in the [board][Board] object given as a parameter to the
[Think()] method. However, the [Setup()] method can only access them
via the inherited properties.

The following subsections address the overriding of each of these three
methods.

### Overriding the Setup() method

If an AI thinker needs to be configured before starting to play, the [Setup()]
method is the place to do it. This method receives a single argument,
a `string`, which can contain thinker-specific parameters, such as maximum
search depth, heuristic to use, and so on. It is the thinker's responsibility
to parse this string. In the [Unity] frontend, the string is specified in the
"Thinker params" field of the @ref ColorShapeLinks.UnityApp.AIPlayer "AIPlayer"
component. When using the [console] frontend, the string is passed via the
`--white/red-params` option for simple matches, or after the thinker's
fully qualified name in the configuration file of a complete session.
Besides the parameters string, the [Setup()] method also has access to
board/match properties inherited from the [base class][AbstractThinker].

The same AI thinker can represent both players in matches, as well as more
than one player in sessions/tournaments. Additionally, separate instances
of the same AI thinker can be configured with different parameters.
In such a case it might be useful to also override the [ToString()]
method for discriminating between the instances configured differently.

Note that concrete AI thinkers require a parameterless constructor in
order to be found by the various frontends. Such constructor [exists by
default in C# classes if no other constructors are defined][csconstruct].
However, it is not advisable to use a parameterless constructor to setup
an AI thinker, since the various board/match properties will not be
initialized at that time. This is yet another good reason to perform
all thinker configuration tasks in the [Setup()] method. In any case,
concrete AI thinkers don't need to provide an implementation of this
method if they are not parameterizable.

### Overriding the Think() method

The [Think()] method is where the AI actually does its job and is the
only mandatory override when extending the [AbstractThinker] class.
This method accepts the [game board][Board] and a
[cancellation token][`CancellationToken`], returning a [FutureMove]. In
other words, the [Think()] method accepts the game board, the AI decides
the best move to perform, returning it. The selected move will eventually
be executed by the match engine.

The [Think()] method is called in a separate thread. As such, it should only
access local instance data. The main thread may ask the AI to stop *thinking*,
for example if the thinking time limit has expired. Thus, while *thinking*,
the AI should frequently test if a cancellation request was made to the
[cancellation token][`CancellationToken`]. If so, it should return immediately
with no move performed, as exemplified in the following code:

```cs
if (ct.IsCancellationRequested) return FutureMove.NoMove;
```

The [game board][Board] can be freely modified within the [Think()] method,
since this is a copy and not the original game board being used in the
main thread. More specifically, the thinker can try moves with the
[DoMove()] method, and cancel them with the [UndoMove()] method. The
board keeps track of the move history, so the thinker can perform any
sequence of moves, and roll them back afterwards.

The [CheckWinner()] method is useful to determine if there is a winner. If
there is one, the solution is placed in the method's optional parameter.

For building heuristics, the public read-only variable [winCorridors] will
probably be useful. This variable is a collection containing all corridors
(sequences of positions) where promising or winning piece sequences may exist.

The AI thinker will lose the match in the following situations:

* Causes or throws an exception.
* Takes too long to play.
* Returns an invalid move, such as:
  * Column out of bounds (<0 or ≥@ref ColorShapeLinks.Common.Board.cols "cols").
  * Column is already full.
  * No more pieces with the specified shape are available.

### Overriding the ToString() method

By default, the [ToString()] method removes the namespace from the thinkers'
fully qualified name, as well as the "thinker", "aithinker" or "thinkerai"
suffixes. However, this method can be overriden in order to behave
differently. One such case is when thinkers are parameterizable, and
differentiating between specific parametrizations during matches and sessions
becomes important. For example, if an AI thinker is configurable with a
`maxDepth` parameter, the following would keep the base method behavior,
while adding the value of the maximum depth to the thinkers' name:

```cs
// If the FQN of this class is My.SuperAIThinker and maxDepth is 6, this
// method will return "SuperD6".
public override string ToString()
{
    return base.ToString() + "D" + maxDepth;
}
```

In any case, concrete AI thinkers are not required to override this method.

## Setting up the development environment {#devenv}

The most basic development environment for a ColorShapeLinks AI thinker
consists of cloning the repository and just place a new class directly in
the [console app][console_folder] or [Unity app][unity_folder] project
folders. This is not practical, however, especially when using a version
control system. Furthermore, although AI thinkers can be tested in [Unity],
serious development is much simpler within the context of the [console]
frontend. As such, this section focuses on setting up a development
environment within that context. Nonetheless, testing in [Unity] can
still be done by copying the project folder to the
[Unity app scripts folder][unity_folder].

The following commands are cross-platform and work in Linux, Windows
(PowerShell), and macOS, requiring only [.NET Core] ≥ 3.1 to be installed.
On Windows, replace slashes `/` with backslashes `\` when referencing
local paths. The steps for setting up a development environment are as
follows:

1. Create a development folder and `cd` into it:
   ```
   $ mkdir color-shape-links-ai-dev
   $ cd color-shape-links-ai-dev
   ```
2. Clone the ColorShapeLinks repository (requires [Git] and [Git LFS]):
   ```
   $ git clone --recurse-submodules https://github.com/VideojogosLusofona/color-shape-links-ai-competition.git
   ```
3. Create a new .NET Standard class library, `cd` into the project's folder
   and remove the default class:
   ```
   $ dotnet new classlib -n MyAIThinkerProjectName
   $ cd MyAIThinkerProjectName
   $ rm Class1.cs
   ```
4. Add the ColorShapeLinks.Common project as a dependency:
   ```
   $ dotnet add reference ../color-shape-links-ai-competition/ConsoleApp/ColorShapeLinks/Common
   ```
5. _Optional:_ Initialize a Git repository, add a `.gitignore` file and make
   the first commit:
   ```
   $ git init
   $ dotnet new gitignore
   $ git add .
   $ git commit -m "First commit"
   ```
6. Using a code editor or IDE (e.g. [VS Code]), create a new class which
   extends [AbstractThinker] and implements the [Think()] method:
   ```cs
    using System.Threading;
    using ColorShapeLinks.Common;
    using ColorShapeLinks.Common.AI;

    public class MyAIThinker : AbstractThinker
    {
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // Will always lose by making a "No move"
            return FutureMove.NoMove;
        }
    }
   ```
7. Build the code:
   ```
   $ dotnet build
   ```
8. Using the [console] app, check if the `MyAIThinker` class appears in the
   "Known thinkers:" section (on Windows replace `$``(pwd)` with the full path
   to the current folder):
   ```
   $ dotnet run -p ../color-shape-links-ai-competition/ConsoleApp/ColorShapeLinks/TextBased/App -- info -a $(pwd)/bin/Debug/netstandard2.0/MyAIThinkerProjectName.dll
   ```
9. Using the [console] app, run a match between `MyAIThinker` and the
   @ref ColorShapeLinks.Common.AI.Examples.RandomAIThinker "random AI"
   provided with the development framework  (on Windows replace
   `$``(pwd)` with the full path to the current folder):
   ```
   $ dotnet run -p ../color-shape-links-ai-competition/ConsoleApp/ColorShapeLinks/TextBased/App -- match -a $(pwd)/bin/Debug/netstandard2.0/MyAIThinkerProjectName.dll -W ColorShapeLinks.Common.AI.Examples.RandomAIThinker -R MyAIThinker
   ```
   The `MyAIThinker` will lose, since it doesn't make a valid move. The
   @ref minimax "Implementing a simple Minimax player" section describes
   the implementation of a basic AI which can beat the random player (most
   of the time).

The [console guide][console] describes all the available options for the
console app, in particular the ones used in the above steps.

## Testing an AI thinker in isolation {#testing}

During development, it is crucial to be able to test the AI thinker in
isolation, i.e., outside of running matches and sessions. This is easy to
accomplish, requiring the creation of a test project which references
the ColorShapeLinks.Common project, as well as the AI thinker-specific
project (which we called `MyAIThinkerProjectName` in the previous section).
However, there is an important caveat to be aware of:

> Properties inherited from [AbstractThinker] will not be automatically
> initialized if the concrete thinker is instantiated directly.

There are two ways to solve this problem:

1. Use reflection to initialize the private instance variables in the base
   class which provide the property values.
2. Use a [ThinkerPrototype] to [create] an instance of the concrete AI thinker.

The second option is preferred, since it not only initializes the base class
properties, as it also invokes the [Setup()] method. The only downside is that
the [ThinkerPrototype] constructor requires a parameter which implements the
[IMatchConfig] interface containing the board/match specifications. Thus, an
additional class or struct is required when using this approach.

## Implementing a simple Minimax player {#minimax}

TODO

[Git]:https://git-scm.com/downloads
[Git LFS]:https://git-lfs.github.com/
[.NET Standard 2.0]:https://docs.microsoft.com/dotnet/standard/net-standard
[.NET Core]:https://dotnet.microsoft.com/download
[VS Code]:https://code.visualstudio.com/
[ossl]:https://opensource.org/licenses
[`unsafe`]:https://docs.microsoft.com/dotnet/csharp/programming-guide/unsafe-code-pointers/
[`CancellationToken`]:https://docs.microsoft.com/dotnet/api/system.threading.cancellationtoken
[AbstractThinker]:@ref ColorShapeLinks.Common.AI.AbstractThinker
[ThinkerPrototype]:@ref ColorShapeLinks.Common.AI.ThinkerPrototype
[create]:@ref ColorShapeLinks.Common.AI.ThinkerPrototype.Create
[IMatchConfig]:@ref ColorShapeLinks.Common.Session.IMatchConfig
[Think()]:@ref ColorShapeLinks.Common.AI.AbstractThinker.Think
[Setup()]:@ref ColorShapeLinks.Common.AI.AbstractThinker.Setup
[ToString()]:@ref ColorShapeLinks.Common.AI.AbstractThinker.ToString
[OnThinkingInfo()]:@ref ColorShapeLinks.Common.AI.AbstractThinker.OnThinkingInfo
[Board]:@ref ColorShapeLinks.Common.Board
[FutureMove]:@ref ColorShapeLinks.Common.AI.FutureMove
[DoMove()]:@ref ColorShapeLinks.Common.Board.DoMove
[UndoMove()]:@ref ColorShapeLinks.Common.Board.UndoMove
[CheckWinner()]:@ref ColorShapeLinks.Common.Board.CheckWinner
[winCorridors]:@ref ColorShapeLinks.Common.Board.winCorridors
[Unity]:@ref unity-guide
[console]:@ref console-guide
[csconstruct]:https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/constructors#parameterless-constructors
[console_folder]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/ConsoleApp/ColorShapeLinks/TextBased
[unity_folder]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/UnityApp/Assets/Scripts
