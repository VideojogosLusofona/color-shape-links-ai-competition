<!--
Board Game AI 2019/2020 (c) by Nuno Fachada

Board Game AI 2019/2020 is licensed under a Creative Commons
Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
-->

# Board Game AI

An assignment for the Artificial Intelligence course unit of the [Bachelor in
Videogames][licvideo] at [Universidade Lusófona de Humanidades e
Tecnologias][ULHT], Lisbon, Portugal.

## Introduction

The goal of this assignment is to implement a competitive AI for the board game
included in this repository, the rules of which are similar to the [Simplexity]
board game. However, this board game can played with arbitrary number of rows
and columns, as well as accepting different number of pieces in a row for
achieving victory. For the purpose of this assignment, we will refer to this
board game as *ColorShapeLinks*.

## The included Unity project

TODO Describe the included Unity project

## Assignment tasks

This project has two tasks, namely, 1) the [implementation of a competitive
AI](#ai-implementation) for the *ColorShapeLinks* board game, and, 2) [writing
of a report](#report). These tasks are discussed in further detail in the next
sections.

### AI implementation

Students should implement a minimum of two classes. For example, if your AI is
called *G03VerySmart*, these classes are as follows:

1. **`G03VerySmartAI`**, which extends [`AIPlayer`]. This class should be added
   as a component of the `SessionConfiguration` game object, and allows your AI
   to be found by the game.
2. **`G03VerySmartAIThinker`**, which implements the [`IThinker`] interface.
   This is were you should actually implement the AI.

These classes should be in their own folder, `G03VerySmart`, which in turn
should be placed at [`Assets/Scripts/AI/AIs/`]. This folder contains some
examples of dumb AIs to demonstrate how your project should be organized.

#### The class that extends `AIPlayer`

This class allows an AI to be found by the game. For that purpose, it must
be added as a component of the `SessionConfiguration` game object, and
implement the following read-only properties:

1. `PlayerName`, which should return the _string_ with the AI's name.
2. `Thinker`, which should return an instance of the class that implements
   [`IThinker`].

The instance of the class that implements [`IThinker`] should be created in the
`Setup()` method, which needs to be overridden when extending [`AIPlayer`].
The [example AIs][`Assets/Scripts/AI/AIs/`] demonstrate how this should be
done.

For AIs that really want to search as long as possible, the time limit is
available in the `AITimeLimit` property, and can be passed to the thinker
during its instantiation in the `Setup()` method.

#### The class that implements `IThinker`

This class is where the AI should be implemented. The [`IThinker`] interface
defines the `Think()` method, which accepts the [game board][`Board`] and a
[cancellation token][`CancellationToken`], returning a [`FutureMove`]. Simply
put, the method accepts the game board, the AI decides the best move to
perform, and returns that move, which will eventually be executed by the game
engine.

##### Cancellation requests

The main thread may ask the AI to stop *thinking*, for example if the thinking
time limit has expired. Thus, while *thinking*, the AI should frequently test
if a cancellation request was made to the
[cancellation token][`CancellationToken`]. If so, it should return immediately
with no move performed, as exemplified in the following code:

```cs
if (ct.IsCancellationRequested) return FutureMove.NoMove;
```

##### Thinker code limitations

The `Think()` method is called in a separate thread from the main Unity thread.
As such, most Unity classes and objects cannot be used by the thinker,
especially those used in Unity's game loop, e.g. [`Time`] and [`Random`][urnd].
The [`Mathf`] functions can be used without problems, since they do not depend
on anything other than the input they are given.

Timing and random number generation functionality, useful or even mandatory for
the thinker, can be obtained with native C# types such as [`DateTime`],
[`TimeSpan`] and [`Random`].

##### Game board functionality inside the thinker

The thinker can freely modify the [game board][`Board`], since this is a copy
and not the original game board being used in the main Unity thread. More
specifically, the thinker can try moves with the `DoMove()` method, and cancel
them with the `UndoMove()` method. The board keeps track of the move history,
so the thinker can perform any sequence of moves, and roll them back
afterwards.

The `CheckWinner()` method is useful to determine if there is a winner. If
there is one, the solution is placed in the method's optional parameter.

For building heuristics, the public read-only variable `winCorridors` might be
important. This variable is a collection containing all corridors (sequences of
positions) where promising or winning piece sequences may exist.

### Report

TODO Report

## Evaluation criteria

TODO Evaluation criteria

## Assignment submission

The assignment should be submitted by groups of 2 or 3 students, via Moodle,
before January 7, 23:00, and must include the following items:

* The class that extends [`AIPlayer`]
* The class that implements [`IThinker`]
* *(Optional)* Other classes required by the two mandatory classes.
* The Markdown-formatted report in the `README.md` file.
* *(Optional)* Images used in the report.

## Academic honesty

Academic dishonesty is unacceptable and will not be tolerated. Cheating,
forgery, plagiarism and collusion in dishonest acts undermine the University's
educational mission and the students' personal and intellectual growth.
[Lusófona][ULHT] students are expected to bear individual responsibility for
their work, to learn the rules and definitions that underlie the practice of
academic integrity, and to uphold its ideals. Ignorance of the rules is not an
acceptable excuse for disobeying them. Any student who attempts to compromise
or devalue the academic process will be sanctioned according to
[Lusófona][ULHT] regulations.

_Text adapted from
[Baruch College](https://www.baruch.cuny.edu/academic/academic_honesty.html)_.

## Licenses

This assignment (the text and non-code files) are made available under the
[Mozilla Public License 2.0][MPLv2]. The code is made available under the
[Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International
License][CC BY-NC-SA 4.0].

## Metadata

* Author: [Nuno Fachada]
* Degree:  [Bachelor in Videogames][licvideo]
* Institution: [Universidade Lusófona de Humanidades e Tecnologias][ULHT]

[MPLv2]:https://opensource.org/licenses/MPL-2.0
[CC BY-NC-SA 4.0]:https://creativecommons.org/licenses/by-nc-sa/4.0/
[licvideo]:https://www.ulusofona.pt/en/undergraduate/videogames
[Nuno Fachada]:https://github.com/fakenmc
[ULHT]:https://www.ulusofona.pt/
[Simplexity]:https://boardgamegeek.com/boardgame/55810/simplexity
[`AIPlayer`]:Assets/Scripts/AI/AIPlayer.cs
[`IThinker`]:Assets/Scripts/AI/IThinker.cs
[`Board`]:Assets/Scripts/BoardGame/Board.cs
[`FutureMove`]:Assets/Scripts/FutureMove.cs
[`Assets/Scripts/AI/AIs/`]:Assets/Scripts/AI/AIs/
[`CancellationToken`]:https://docs.microsoft.com/dotnet/api/system.threading.cancellationtoken
[`DateTime`]:https://docs.microsoft.com/dotnet/api/system.datetime
[`TimeSpan`]:https://docs.microsoft.com/dotnet/api/system.timespan
[`Random`]:https://docs.microsoft.com/dotnet/api/system.random
[urnd]:https://docs.unity3d.com/ScriptReference/Random.html
[`Time`]:https://docs.unity3d.com/ScriptReference/Time.html
[`Mathf`]:https://docs.unity3d.com/ScriptReference/Mathf.html
