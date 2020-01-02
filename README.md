<!--
Board Game AI 2019/2020 (c) by Nuno Fachada

Board Game AI 2019/2020 is licensed under a Creative Commons
Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
-->

# ColorShapeLinks AI

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

A Unity project implementing *ColorShapeLinks* is included in this assignment.
The project should be executed within the Unity editor, not as a standalone
build. Project execution can be configured by manipulating the
`SessionConfiguration` game object in the Unity Editor. This is done by: 1)
editing the fields of the [`SessionController`] script; and, 2) adding or
removing AI scripts, i.e., scripts which extend [`AIPlayer`] (see the [AI
implementation](#ai-implementation)) section.

### Fields of the `SessionController` game object

Fields of the [`SessionController`] script are divided in three sections:

1. **Match properties** - Board dimensions, win conditions or initial number of
   pieces per player and last move animation length in seconds.
2. **AI properties** - AI time limit in seconds and minimum AI game move time.
3. **Tournament properties** - Points per win, draw, loss, and information
   screen blocking and duration options.

Tournaments occur automatically if there are more than two AI scripts active in
the `SessionConfiguration` game object. Otherwise a single match is played,
as discussed in the next section.

### Adding and removing AI scripts

Zero or more AI scripts can be added to the `SessionConfiguration` game
object. These scripts extend the [`AIPlayer`] class, as discussed in the
[AI implementation](#ai-implementation) section. The number of active AI
scripts in the `SessionConfiguration` game object determines what type of
session will run:

* Zero active AI scripts: a match between human players will take place.
* One active AI script: a game between the AI and a human player will take
  place.
* Two active AI scripts: a game between the two AIs will take place.
* More than two active AI scripts: a **tournament session** will take place,
  where each AI plays against all other AIs twice, one as the first player
  (white), another as the second player (red).

During and after the tournament session, all match results as well as current
standings / classifications, are presented.

## Assignment tasks

This project has two tasks, namely, 1) the [implementation of a competitive
AI](#ai-implementation) for the *ColorShapeLinks* board game, and, 2) [writing
of a report](#report). These tasks are discussed in further detail in the next
sections.

### AI implementation

Students should implement a minimum of two classes. For example, if your AI is
called *G03VerySmart*, these classes are as follows:

1. **`G03VerySmartAI`**, which extends [`AIPlayer`]. This class should be added
   as a component of the `SessionConfiguration` game object, and allows the AI
   to be found by the game.
2. **`G03VerySmartAIThinker`**, which implements the [`IThinker`] interface.
   This is were the AI should actually be implemented.

These classes should be in their own folder, `G03VerySmart`, which in turn
should be placed at [`Assets/Scripts/AI/AIs/`]. This folder contains some
examples of dumb AIs to demonstrate how the code should be organized.

Students should not modify existing code. If a bug is found in the included
Unity project, a fix should be submitted through a
[pull request](https://github.com/VideojogosLusofona/ia_2019_board_game_ai/compare),
so it becomes available to everyone.

#### The class that extends `AIPlayer`

This class allows an AI to be found by the game, and for the AI to be
optionally configured in the Unity editor. For that purpose, it must
be added as a component of the `SessionConfiguration` game object, and
implement the following read-only properties:

1. `PlayerName`, which should return the _string_ with the AI's name.
2. `Thinker`, which should return an instance of the class that implements
   [`IThinker`].

The instance of the class that implements [`IThinker`] should be created in the
`Setup()` method, which needs to be overridden when extending [`AIPlayer`].
The [example AIs][`Assets/Scripts/AI/AIs/`] demonstrate how this should be
done.

For configuring the AI in the Unity editor, the class that extends [`AIPlayer`]
can have editable fields (e.g. maximum search depth). Following best practices,
these fields should be private and have the [\[`SerializeField`\]] attribute.

For AIs that really want to search as long and deep as possible, the time limit
is available in the `AITimeLimit` property, and can be passed to the thinker
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

The report should be in the form of a properly formatted Markdown file named
`README.md`, and contain the following information:

* Author names (first and last), and respective student numbers.
* Information of who did what in the assignment.
* Description of the developed solution, namely the implemented algorithm and
  the chosen static evaluation function (heuristic).
  * Diagrams or schemes that aid, enhance and/or simplify the description will
    have positive influence in the final grade.
* References, including books, papers, websites, discussions with colleagues,
  reutilized open source code (e.g. from StackOverflow).

## Evaluation criteria

The assignment will be evaluated with a grade from 0 to 20, according to the
following criteria:

* Up to 11: a basic functional Minimax/Negamax search, with a well thought-out
  heuristic for a game of *ColorShapeLinks* with standard [Simplexity] rules
  (7x7 board, 4 in a row win condition), and capable of stop searching when the
  cancellation token is activated.
* Up to 13: the search is optimized with alpha-beta pruning.
* Up to 15: the search can be used for games of *ColorShapeLinks* with any
  board board size and win condition.
* Over 15: the search is optimized with one or more of the following
  approaches:
  * Move ordering (moves with best potential are searched first).
  * Iterative deepening, which returns best solution found before the AI time
    limit expires.
  * Other approaches not discussed in class: Negascout, transposition tables,
    etc.

Within these criteria, the following items will also be considered:

* Code: quality, readability, comments and organization.
* Report: quality of writing, organization of ideas, references, auxiliary
  diagrams and schemes.

All AIs will face each other in two tournaments:

1. The first tournament will be played using standard [Simplexity] rules
   (7x7 board, 4 in a row win condition) and with a time limit of 0.2 seconds.
   * The top 3 AIs will receive a bonus of 1.5, 1 and 0.5 points in their
     grade.
   * AIs not competent enough to enter the tournament (i.e., they crash or
     freeze the Unity project, or do not respond to cancellation requests),
     will be penalized with 1 point in the final grade.
2. The second tournament will be played using an arbitrary board size, win
   condition and time limit. These will only be revealed immediately before
   the tournament starts.
   * AIs capable of simply playing in this tournament will receive 1 bonus
     point in their grade.
   * The top 3 AIs will receive a bonus of 1.5, 1 and 0.5 points in their
     grade.

The bonuses and penalties are cumulative, but the final grade cannot be lower
than 0 or higher than 20.

## Assignment submission

The assignment should be submitted by groups of 2 or 3 students, via Moodle,
before January 7, 23:00, and must include the following items:

* The class that extends [`AIPlayer`]
* The class that implements [`IThinker`]
* *(Optional)* Other classes required by the two mandatory classes.
* The Markdown-formatted report in the `README.md` file.
* *(Optional)* Images used in the report.

A discussion will be performed on January 8, during class, as well as the two
tournaments referred in the previous section.

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
[`SessionController`]:Assets/Scripts/SessionController.cs
[`Assets/Scripts/AI/AIs/`]:Assets/Scripts/AI/AIs/
[`CancellationToken`]:https://docs.microsoft.com/dotnet/api/system.threading.cancellationtoken
[`DateTime`]:https://docs.microsoft.com/dotnet/api/system.datetime
[`TimeSpan`]:https://docs.microsoft.com/dotnet/api/system.timespan
[`Random`]:https://docs.microsoft.com/dotnet/api/system.random
[urnd]:https://docs.unity3d.com/ScriptReference/Random.html
[`Time`]:https://docs.unity3d.com/ScriptReference/Time.html
[`Mathf`]:https://docs.unity3d.com/ScriptReference/Mathf.html
[\[`SerializeField`\]]:https://docs.unity3d.com/ScriptReference/SerializeField.html
