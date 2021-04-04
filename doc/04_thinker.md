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
know a bit more about the @ref rules "source code rules and restrictions"
and about the @ref abstractthinker "AbstractThinker base class". Setting up a
proper @ref devenv "development environment" and being able to adequately
@ref testing "test" the developed code is also essential. This guide
closes with a tutorial on how to implement a @ref minimax "basic Minimax AI"
with a very simple heuristic.

## Rules and restrictions for the AI source code {#rules}

The source code of AI thinkers must follow these rules and restrictions:

- Can only use cross-platform [.NET Standard 2.0] API calls in C#.
- Can use additional (open-source) libraries which abide by these same rules.
- Must run in the same process that invokes it.
- Can be multithreaded and use [`unsafe`] contexts.
- Cannot *think* in its opponent time (e.g., by using a background thread).
- Must acknowledge [cancellation requests][`CancellationToken`], in which case
  it should terminate quickly and in an orderly fashion.
- Can only probe the environment for the number of processor cores. Cannot
  search or use any other information, besides what is already available in the
  [`AbstractThinker`] class or passed to the [`Think()`] method, e.g., such as
  using reflection to probe the capabilities of its opponents.
- Cannot use and/or communicate with external data sources.
- Cannot use more than 2GB of memory during the course of a match.
- Must have a reasonable size in disk, including libraries. For example,
  source code, project files and compiled binaries should not exceed 1 MB.

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
@ref ColorShapeLinks.TextBased.Lib.IThinkerListener "thinker listeners" (which
by default print to the console).

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

### Overriding the Setup() method {#setup}

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

The [CheckWinner()] method is useful to determine if there's a winner. If
there is one, the solution is placed in the method's optional parameter.

For building heuristics, the public read-only variable [winCorridors] will
probably be useful. This variable is a collection containing all corridors
(sequences of positions) where promising or winning piece sequences may exist.

The AI thinker will lose the match in the following situations:

* Causes or throws an exception.
* Takes too long to play.
* Returns an invalid move, such as:
  * Column out of bounds (<0 or >=@ref ColorShapeLinks.Common.Board.cols "cols").
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
frontend, which also allows thinkers to be [tested in isolation](#testing).
Thus, this section focuses on setting up a development environment within that
context. Nonetheless, testing in [Unity] can still be done by copying the
thinker code to the [Unity app scripts folder][unity_folder].

The following commands are cross-platform and work in Linux, Windows and macOS,
requiring only [.NET Core] ≥ 3.1 to be installed.
On Windows, when not using Git Bash, replace slashes `/` with backslashes `\`
when referencing local paths. The steps for setting up a development environment
are as follows:

1. Create a development folder and `cd` into it:
   ```
   $ mkdir color-shape-links-ai-dev
   $ cd color-shape-links-ai-dev
   ```
2. Clone the ColorShapeLinks repository (requires [Git] and [Git LFS]):
   ```
   $ git clone --recurse-submodules https://github.com/VideojogosLusofona/color-shape-links-ai-competition.git
   ```
3. Create a development folder and `cd` into it:
   ```
   $ mkdir my-ai-solution
   $ cd my-ai-solution
   ```
   Currently, the folder structure should be as follows:
   ```
   └──color-shape-links-ai-dev/
      ├──color-shape-links-ai-competition/
      └──my-ai-solution/
   ```
4. _Optional:_ Initialize a Git repository, add a `.gitignore` file and make
   the first commit:
   ```
   $ git init
   $ dotnet new gitignore
   $ git add .
   $ git commit -m "First commit"
   ```
5. Create a new .NET Standard 2.0 class library and remove the default class:
   ```
   $ dotnet new classlib -n MyAI -f netstandard2.0
   $ rm MyAI/Class1.cs
   ```
   The folder structure should now be:
   ```
   └──color-shape-links-ai-dev/
      ├──color-shape-links-ai-competition/
      └──my-ai-solution/
         ├──.gitignore
         └──MyAI/
   ```
6. Add the ::ColorShapeLinks.Common project as a dependency:
   ```
   $ dotnet add MyAI reference ../color-shape-links-ai-competition/ConsoleApp/ColorShapeLinks/Common
   ```
7. In the `MyAI/` folder, create a new class which extends [AbstractThinker] and
   implements the [Think()] method:
   ```cs
    using System.Threading;
    using ColorShapeLinks.Common;
    using ColorShapeLinks.Common.AI;

    namespace MyAISolution.MyAI
    {
        public class MyThinker : AbstractThinker
        {
            public override FutureMove Think(Board board, CancellationToken ct)
            {
                // Will always lose by making a "No move"
                return FutureMove.NoMove;
            }
        }
    }
   ```
8. Build the code:
   ```
   $ dotnet build MyAI
   ```
9.  Using the [console] app, check if the `MyAISolution.MyAI.MyThinker` class
   appears in the "Known thinkers:" section (on Windows PowerShell replace
   `$``(pwd)` with `$pwd` or with the full path to the current folder):
   ```
   $ dotnet run -p ../color-shape-links-ai-competition/ConsoleApp/ColorShapeLinks/TextBased/App -- info -a $(pwd)/MyAI/bin/Debug/netstandard2.0/MyAI.dll
   ```
10. Using the [console] app, run a match between `MyThinker` and the
   @ref ColorShapeLinks.Common.AI.Examples.RandomAIThinker "random AI"
   provided with the development framework  (on Windows PowerShell replace
   `$``(pwd)` with `$pwd` or with the full path to the current folder):
   ```
   $ dotnet run -p ../color-shape-links-ai-competition/ConsoleApp/ColorShapeLinks/TextBased/App -- match -a $(pwd)/MyAI/bin/Debug/netstandard2.0/MyAI.dll -W ColorShapeLinks.Common.AI.Examples.RandomAIThinker -R MyAISolution.MyAI.MyThinker
   ```
   _MyThinker_ will lose, since it doesn't make a valid move. The
   @ref minimax "Implementing a simple Minimax player" section describes
   the implementation of a basic AI which can beat the random player (most
   of the time).

The [console guide][console] describes all the available options for the
console app, in particular the ones used in the above steps.

## Testing an AI thinker in isolation {#testing}

During development, it is crucial to be able to test the AI thinker in
isolation, i.e., outside of running matches and sessions. This is easy to
accomplish, requiring the creation of a test project which references
the AI thinker-specific project (which we called `MyAI` in the previous
section). Continuing with the setup created in the previous section, and
assuming we're in the `my-ai-solution` folder, let's create a console project
for testing our AI in isolation:

```
$ dotnet new console -n TestMyAI
```

We also need to add a reference to the `MyAI` project:

```
$ dotnet add TestMyAI reference MyAI
```

At this stage, it's a good idea to create a .NET solution to include both the
`MyAI` and `TestMyAI` projects (which allows, for example, to have both projects
open at the same time in Visual Studio):

```
$ dotnet new sln
$ dotnet sln add TestMyAI
$ dotnet sln add MyAI
```

The folder structure should now be:
```
└──color-shape-links-ai-dev/
   ├──color-shape-links-ai-competition/
   └──my-ai-solution/
      ├──.gitignore
      ├──my-ai-solution.sln
      ├──MyAI/
      └──TestMyAI/
```

We can now edit `TestMyAI/Program.cs`, create an instance of `MyThinker`,
manipulate it and test it as we see fit, and run our test project with:

```
$ dotnet run -p TestMyAI
```

However, there is an important caveat to be aware of:

@attention Properties inherited from [AbstractThinker] will not be
automatically initialized if the concrete thinker is instantiated directly.

Thus, a [ThinkerPrototype] should instead be used to [create] an instance of the
concrete AI thinker. This initializes the base class properties, as it also
invokes the [Setup()] method. The
[ThinkerPrototype constructor][ThinkerPrototypeCtor] requires three parameters:

1. A string containing the thinker's fully qualified name.
2. A string containing the thinker's configuration parameters.
3. An object which implements the [IMatchConfig] interface.

The last parameter is generally a frontend-dependent type. However, the
[Common] assembly contains the [MatchConfig] helper class, a simple
container of match properties which can be used for this purpose. Thus,
instantiating our basic AI thinker in isolation can be done as follows in the
`TestMyAI/Program.cs` file:

```cs
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;
using MyAISolution.MyAI;

namespace MyAISolution.TestMyAI
{
    class Program
    {
        static void Main(string[] args)
        {
            MatchConfig mc = new MatchConfig(); // Use default values
            ThinkerPrototype tp = new ThinkerPrototype(typeof(MyThinker).FullName, "", mc);
            IThinker thinker = tp.Create();
        }
    }
}
```

A more complete example is available [here](https://github.com/VideojogosLusofona/ia_2020_p1_exemplo/blob/main/TestUltron/Program.cs).

## Implementing a simple Minimax player {#minimax}

In this section we discuss the implementation of a basic [Minimax] AI thinker
with a very simple heuristic. A minimax algorithm is a ["recursive algorithm
for choosing the next move in (...) a two-player game"][Minimax].
The most basic version of this algorithm tries out all possible moves,
branching out the game tree down to a maximum depth, since the search space
would otherwise be too large. Most board states searched
by the algorithm, even when it reaches maximum depth, will not be final boards.
As such, we'll need an [heuristic] function to evaluate these non-final boards.
An heuristic is ["an educated guess, an intuitive judgment"][heuristic]
which helps us evaluate the "goodness" of a board state.
The better the heuristic, the better the AI will be able
to evaluate intermediate boards, and the better it'll play.

Let's start with the template presented in the previous section:

```cs
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace MyAISolution.MyAI
{
    public class MyThinker : AbstractThinker
   {
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // Will always lose by making a "No move"
            return FutureMove.NoMove;
        }
    }
}
```

A minimax algorithm works by maximizing the heuristic score of all possible
moves when it's the AI's turn to play, and minimizing it when it's the
opponent's turn. As such, a `Minimax()` function requires:

* The current board state.
* The color of the AI player.
* The color of who's playing in the current turn.
* The current depth.
* The maximum depth.

It will also need the [`CancellationToken`], so it can check for cancellation
requests from the main thread. As such, the code will look something like:

```cs
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace MyAISolution.MyAI
{
    public class MyThinker : AbstractThinker
    {
        // Maximum depth, set it at 3 for now
        private int maxDepth = 3;

        // The Think() method (mandatory override) is invoked by the game engine
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // Invoke minimax, starting with zero depth
            (FutureMove move, float score) decision =
                Minimax(board, ct, board.Turn, board.Turn, 0);

            // Return best move
            return decision.move;
        }

        // Our minimax implementation
        private (FutureMove move, float score) Minimax(
            Board board, CancellationToken ct, PColor player, PColor turn, int depth)
        {
            // Return invalid move, will always lose
            return (FutureMove.NoMove, float.NaN);
        }
    }
}
```

The infrastructure is all set. The following steps have to be implemented in
the `Minimax()` function:

1. If the cancellation token was activated, return immediately with
   a "no move" (score is irrelevant).
2. Otherwise, if the board is in a final state, return the appropriate score
   (move is irrelevant since no moves can be made on a final board):
   * If the winner is the AI, return the highest possible score.
   * If the winner is the opponent, return the lowest possible score.
   * If the match ended in a draw, return a score of zero.
3. Otherwise, if the maximum depth has been reached, return the score provided
   by the heuristic function (move is irrelevant, since the game tree will not
   be branched further below this depth, and as such, there's no move to
   chose from).
4. Otherwise, for each possible move, invoke `Minimax()` recursively,
   selecting the best score and associated move (i.e., maximizing) if it's
   the AI's turn, or selecting the worst score and associated move (i.e.,
   minimizing) if it's the opponent's turn.

Implementing this reasoning in the `Minimax()` function can be done as follows:

```cs
private (FutureMove move, float score) Minimax(
    Board board, CancellationToken ct, PColor player, PColor turn, int depth)
{
    // Move to return and its heuristic value
    (FutureMove move, float score) selectedMove;

    // Current board state
    Winner winner;

    // If a cancellation request was made...
    if (ct.IsCancellationRequested)
    {
        // ...set a "no move" and skip the remaining part of the algorithm
        selectedMove = (FutureMove.NoMove, float.NaN);
    }
    // Otherwise, if it's a final board, return the appropriate evaluation
    else if ((winner = board.CheckWinner()) != Winner.None)
    {
        if (winner.ToPColor() == player)
        {
            // AI player wins, return highest possible score
            selectedMove = (FutureMove.NoMove, float.PositiveInfinity);
        }
        else if (winner.ToPColor() == player.Other())
        {
            // Opponent wins, return lowest possible score
            selectedMove = (FutureMove.NoMove, float.NegativeInfinity);
        }
        else
        {
            // A draw, return zero
            selectedMove = (FutureMove.NoMove, 0f);
        }
    }
    // If we're at maximum depth and don't have a final board, use
    // the heuristic
    else if (depth == maxDepth)
    {
        // Where did this Heuristic() function come from?
        // We'll return to it in a moment
        selectedMove = (FutureMove.NoMove, Heuristic(board, player));
    }
    else // Board not final and depth not at max...
    {
        //...so let's test all possible moves and recursively call Minimax()
        // for each one of them, maximizing or minimizing depending on who's
        // turn it is

        // Initialize the selected move...
        selectedMove = turn == player
            // ...with negative infinity if it's the AI's turn and we're
            // maximizing (so anything except defeat will be better than this)
            ? (FutureMove.NoMove, float.NegativeInfinity)
            // ...or with positive infinity if it's the opponent's turn and we're
            // minimizing (so anything except victory will be worse than this)
            : (FutureMove.NoMove, float.PositiveInfinity);

        // Test each column
        for (int i = 0; i < Cols; i++)
        {
            // Skip full columns
            if (board.IsColumnFull(i)) continue;

            // Test shapes
            for (int j = 0; j < 2; j++)
            {
                // Get current shape
                PShape shape = (PShape)j;

                // Use this variable to keep the current board's score
                float eval;

                // Skip unavailable shapes
                if (board.PieceCount(turn, shape) == 0) continue;

                // Test move, call minimax and undo move
                board.DoMove(shape, i);
                eval = Minimax(board, ct, player, turn.Other(), depth + 1).score;
                board.UndoMove();

                // If we're maximizing, is this the best move so far?
                if (turn == player && eval > selectedMove.score)
                {
                    // If so, keep it
                    selectedMove = (new FutureMove(i, shape), eval);
                }
                // Otherwise, if we're minimizing, is this the worst move so far?
                else if (turn == player.Other() && eval < selectedMove.score)
                {
                    // If so, keep it
                    selectedMove = (new FutureMove(i, shape), eval);
                }
            }
        }
    }

    // Return selected move and its heuristic value
    return selectedMove;
}
```

We're almost there, but there's still a piece missing: the heuristic function.
This is a fundamental part of the solution, and as such, only a very basic
approach is discussed here. Intuitively, pieces near or at the center of the
board potentially contribute to more winning sequences than pieces near
corners or edges. This is not a scientific claim, just a (possibly unfounded)
guess. As such, let's build an heuristic that values pieces placed closer to
the center of the board:

```cs
using System;

// ....

private float Heuristic(Board board, PColor color)
{
    // Distance between two points
    float Dist(float x1, float y1, float x2, float y2)
    {
        return (float)Math.Sqrt(
            Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
    }

    // Determine the center row
    float centerRow = board.rows / 2;
    float centerCol = board.cols / 2;

    // Maximum points a piece can be awarded when it's at the center
    float maxPoints = Dist(centerRow, centerCol, 0, 0);

    // Current heuristic value
    float h = 0;

    // Loop through the board looking for pieces
    for (int i = 0; i < board.rows; i++)
    {
        for (int j = 0; j < board.cols; j++)
        {
            // Get piece in current board position
            Piece? piece = board[i, j];

            // Is there any piece there?
            if (piece.HasValue)
            {
                // If the piece is of our color, increment the
                // heuristic inversely to the distance from the center
                if (piece.Value.color == color)
                    h += maxPoints - Dist(centerRow, centerCol, i, j);
                // Otherwise decrement the heuristic value using the
                // same criteria
                else
                    h -= maxPoints - Dist(centerRow, centerCol, i, j);
                // If the piece is of our shape, increment the
                // heuristic inversely to the distance from the center
                if (piece.Value.shape == color.Shape())
                    h += maxPoints - Dist(centerRow, centerCol, i, j);
                // Otherwise decrement the heuristic value using the
                // same criteria
                else
                    h -= maxPoints - Dist(centerRow, centerCol, i, j);
            }
        }
    }
    // Return the final heuristic score for the given board
    return h;
}
```

We now have a working AI thinker. We can make our thinker more flexible by
allowing the maximum depth to be specified using the `Setup()` parameters
string:

```cs
// The Setup() method, optional override
public override void Setup(string str)
{
    // Try to get the maximum depth from the parameters
    if (!int.TryParse(str, out maxDepth))
    {
        // If not possible, set it to 3 by default
        maxDepth = 3;
    }
}
```

It would also be useful to differentiate between instances of our AI thinker
parameterized with various maximum depths, playing against each other in
matches or tournaments. This can be accomplished by overriding the `ToString()`
method and customizing the AI thinker's name:

```cs
// The ToString() method, optional override
public override string ToString()
{
   return base.ToString() + "D" + maxDepth;
}
```

Although this implementation will win against a random player (unless the
random player is really lucky), and probably some human players as well, it's
in reality a very simple solution. Thus, while it's a good way of getting
started in board game AI, it won't go very far in a competition. The complete
code of this AI thinker is as follows (it's also in the
@ref ColorShapeLinks.Common.AI.Examples.MinimaxAIThinker "MinimaxAIThinker"
class, included with the development framework):

```cs
using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace MyAISolution.MyAI
{
    public class MyThinker : AbstractThinker
    {
        // Maximum depth
        private int maxDepth;

        // The Setup() method, optional override
        public override void Setup(string str)
        {
            // Try to get the maximum depth from the parameters
            if (!int.TryParse(str, out maxDepth))
            {
                // If not possible, set it to 3 by default
                maxDepth = 3;
            }
        }

        // The ToString() method, optional override
        public override string ToString()
        {
            return base.ToString() + "D" + maxDepth;
        }

        // The Think() method (mandatory override) is invoked by the game engine
        public override FutureMove Think(Board board, CancellationToken ct)
        {

            // Invoke minimax, starting with zero depth
            (FutureMove move, float score) decision =
                Minimax(board, ct, board.Turn, board.Turn, 0);

            // Return best move
            return decision.move;
        }

        // Minimax implementation
        private (FutureMove move, float score) Minimax(
            Board board, CancellationToken ct,
            PColor player, PColor turn, int depth)
        {
            // Move to return and its heuristic value
            (FutureMove move, float score) selectedMove;

            // Current board state
            Winner winner;

            // If a cancellation request was made...
            if (ct.IsCancellationRequested)
            {
                // ...set a "no move" and skip the remaining part of the algorithm
                selectedMove = (FutureMove.NoMove, float.NaN);
            }
            // Otherwise, if it's a final board, return the appropriate evaluation
            else if ((winner = board.CheckWinner()) != Winner.None)
            {
                if (winner.ToPColor() == player)
                {
                    // AI player wins, return highest possible score
                    selectedMove = (FutureMove.NoMove, float.PositiveInfinity);
                }
                else if (winner.ToPColor() == player.Other())
                {
                    // Opponent wins, return lowest possible score
                    selectedMove = (FutureMove.NoMove, float.NegativeInfinity);
                }
                else
                {
                    // A draw, return zero
                    selectedMove = (FutureMove.NoMove, 0f);
                }
            }
            // If we're at maximum depth and don't have a final board, use
            // the heuristic
            else if (depth == maxDepth)
            {
                selectedMove = (FutureMove.NoMove, Heuristic(board, player));
            }
            else // Board not final and depth not at max...
            {
                //...so let's test all possible moves and recursively call Minimax()
                // for each one of them, maximizing or minimizing depending on who's
                // turn it is

                // Initialize the selected move...
                selectedMove = turn == player
                    // ...with negative infinity if it's the AI's turn and we're
                    // maximizing (so anything except defeat will be better than this)
                    ? (FutureMove.NoMove, float.NegativeInfinity)
                    // ...or with positive infinity if it's the opponent's turn and we're
                    // minimizing (so anything except victory will be worse than this)
                    : (FutureMove.NoMove, float.PositiveInfinity);

                // Test each column
                for (int i = 0; i < Cols; i++)
                {
                    // Skip full columns
                    if (board.IsColumnFull(i)) continue;

                    // Test shapes
                    for (int j = 0; j < 2; j++)
                    {
                        // Get current shape
                        PShape shape = (PShape)j;

                        // Use this variable to keep the current board's score
                        float eval;

                        // Skip unavailable shapes
                        if (board.PieceCount(turn, shape) == 0) continue;

                        // Test move, call minimax and undo move
                        board.DoMove(shape, i);
                        eval = Minimax(board, ct, player, turn.Other(), depth + 1).score;
                        board.UndoMove();

                        // If we're maximizing, is this the best move so far?
                        if (turn == player
                            && eval >= selectedMove.score)
                        {
                            // If so, keep it
                            selectedMove = (new FutureMove(i, shape), eval);
                        }
                        // Otherwise, if we're minimizing, is this the worst move so far?
                        else if (turn == player.Other()
                            && eval <= selectedMove.score)
                        {
                            // If so, keep it
                            selectedMove = (new FutureMove(i, shape), eval);
                        }
                    }
                }
            }

            // Return movement and its heuristic value
            return selectedMove;
        }

        // Heuristic function
        private float Heuristic(Board board, PColor color)
        {
            // Distance between two points
            float Dist(float x1, float y1, float x2, float y2)
            {
                return (float)Math.Sqrt(
                    Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            }

            // Determine the center row
            float centerRow = board.rows / 2;
            float centerCol = board.cols / 2;

            // Maximum points a piece can be awarded when it's at the center
            float maxPoints = Dist(centerRow, centerCol, 0, 0);

            // Current heuristic value
            float h = 0;

            // Loop through the board looking for pieces
            for (int i = 0; i < board.rows; i++)
            {
                for (int j = 0; j < board.cols; j++)
                {
                    // Get piece in current board position
                    Piece? piece = board[i, j];

                    // Is there any piece there?
                    if (piece.HasValue)
                    {
                        // If the piece is of our color, increment the
                        // heuristic inversely to the distance from the center
                        if (piece.Value.color == color)
                            h += maxPoints - Dist(centerRow, centerCol, i, j);
                        // Otherwise decrement the heuristic value using the
                        // same criteria
                        else
                            h -= maxPoints - Dist(centerRow, centerCol, i, j);
                        // If the piece is of our shape, increment the
                        // heuristic inversely to the distance from the center
                        if (piece.Value.shape == color.Shape())
                            h += maxPoints - Dist(centerRow, centerCol, i, j);
                        // Otherwise decrement the heuristic value using the
                        // same criteria
                        else
                            h -= maxPoints - Dist(centerRow, centerCol, i, j);
                    }
                }
            }
            // Return the final heuristic score for the given board
            return h;
        }
    }
}
```

[Minimax]:https://en.wikipedia.org/wiki/Minimax
[heuristic]:https://en.wikipedia.org/wiki/Heuristic
[Git]:https://git-scm.com/downloads
[Git LFS]:https://git-lfs.github.com/
[.NET Standard 2.0]:https://docs.microsoft.com/dotnet/standard/net-standard
[.NET Core]:https://dotnet.microsoft.com/download
[VS Code]:https://code.visualstudio.com/
[ossl]:https://opensource.org/licenses
[`unsafe`]:https://docs.microsoft.com/dotnet/csharp/programming-guide/unsafe-code-pointers/
[`CancellationToken`]:https://docs.microsoft.com/dotnet/api/system.threading.cancellationtoken
[csconstruct]:https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/constructors#parameterless-constructors
[console_folder]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/ConsoleApp/ColorShapeLinks/TextBased
[unity_folder]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/UnityApp/Assets/Scripts
[Common]:@ref ColorShapeLinks.Common
[AbstractThinker]:@ref ColorShapeLinks.Common.AI.AbstractThinker
[ThinkerPrototype]:@ref ColorShapeLinks.Common.AI.ThinkerPrototype
[ThinkerPrototypeCtor]:@ref ColorShapeLinks.Common.AI.ThinkerPrototype.ThinkerPrototype
[create]:@ref ColorShapeLinks.Common.AI.ThinkerPrototype.Create
[IMatchConfig]:@ref ColorShapeLinks.Common.Session.IMatchConfig
[MatchConfig]:@ref ColorShapeLinks.Common.Session.MatchConfig
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
