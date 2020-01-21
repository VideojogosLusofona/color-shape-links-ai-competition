<!--
ColorShapeLinks AI 2020 (c) by Nuno Fachada

ColorShapeLinks AI 2020 is licensed under a Creative Commons
Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
-->

# ColorShapeLinks AI competition

*A competition proposal for the [IEEE CoG 2020] conference*

## Description

ColorShapeLinks is an AI competition for the [Simplexity] board game with
arbitrary game dimensions. The first player to place *n* pieces of the same
type in a row wins. In this regard, the base game, with a 6 x 7 board and
_n_ = 4, is similar to [Connect Four]. However, the type of piece is defined
not only by color, but also by shape. Shape can be round or square. Round
or white pieces offer the win to player 1, while square or red pieces
do the same for player 2. Contrary to color, players start the game with
pieces of both shapes. This means that a less observant player
can lose in its turn, especially since shape has priority over color as a
winning condition. Given this fact, as well as the arbitrary game dimensions,
the challenges for the AI, namely at the level of the heuristic evaluation
function, are multifold.

## The competition

### Tracks

The competition runs on two distinct tracks:

1. The **Base Track** competition will be played using standard [Simplexity]
   rules (6x7 board, 4 pieces in a row for victory) and with a time limit of
   0.2 seconds.
2. The **Unknown Track** competition will be played under conditions that will
   only be revealed after the competition deadline. These conditions will be
   derived from the first [EuroMillions] draw that takes place after the
   deadline, most likely at May 19, 2020, as follows:
   * NumberOfRows = Lowest [EuroMillions] ball number higher than 6
     (ascending order).
   * NumberOfCols = Next [EuroMillions] ball (ascending order).
   * WiningSequenceLength = Ceil(NumberOfRows / 2.0).
   * InitialNumberOfRoundPiecesPerPlayer = Floor(NumberOfRows * NumberOfCols / 4.0)
   * InitialNumberOfSquarePiecesPerPlayer = Ceil(NumberOfRows * NumberOfCols / 4.0)
   * TimeLimit (milliseconds) = 25 * Max(NumberOfRows, NumberOfCols)

### Classification

All AIs will play against each other two times, so each AI has the opportunity
to play first. Players will be awarded 3 points per win, 1 point per draw and
0 points per loss. The classification for each track will be based on the total
number of points obtained per AI, sorted in descending order.

Tie-breaks are performed only when there are two or more AIs with the same
points in first place. In such cases, ties are solved according to the
following criteria (from most to least important):

1. Greatest number of points obtained in the matches between AIs with the same
   points.
2. Perform a tie-break tournament between the AIs still tied with the time
   limit doubled. Repeat the process at most 10 times until the tie is broken.
3. Perform a tie-break tournament between the AIs still tied with the time
   limit halved. Repeat the process at most 10 times until the tie is broken.
4. If the tie persists, the AIs are considered officially tied and *ex aequo*
   winners of the competition.

## The AI code

Competition code must be implemented in C# and be compatible with the
cross-platform [.NET Standard 2.0].

At least one class is required for the AI to work. This class should implement
the [`IThinker`] interface, which defines the `Think()` method, which in turn
accepts the [game board][`Board`] and a
[cancellation token][`CancellationToken`], returning a [`FutureMove`]. Simply
put, the method accepts the game board, the AI decides the best move to
perform, and returns that move, which will eventually be executed by the game
engine.

The `Think()` method is called in a separate thread. As such, it should not try
to access shared data and restrict itself to the [.NET Standard 2.0] API.
Third-party libraries can be used if cross-platform and compatible with
[.NET Standard 2.0]. The AI code and libraries used must be open-source.

The main thread may ask the AI to stop *thinking*, for example if the thinking
time limit has expired. Thus, while *thinking*, the AI should frequently test
if a cancellation request was made to the
[cancellation token][`CancellationToken`]. If so, it should return immediately
with no move performed, as exemplified in the following code:

```cs
if (ct.IsCancellationRequested) return FutureMove.NoMove;
```

The thinker can freely modify the [game board][`Board`], since this is a copy
and not the original game board being used in the main thread. More
specifically, the thinker can try moves with the `DoMove()` method, and cancel
them with the `UndoMove()` method. The board keeps track of the move history,
so the thinker can perform any sequence of moves, and roll them back
afterwards.

The `CheckWinner()` method is useful to determine if there is a winner. If
there is one, the solution is placed in the method's optional parameter.

For building heuristics, the public read-only variable `winCorridors` might be
important. This variable is a collection containing all corridors (sequences of
positions) where promising or winning piece sequences may exist.

The AI code can be tested using the console or the [Unity] game engine, both of
which are discussed in the next sections.

### Testing the AI in the console

***TO DO*** if the proposal is accepted in [IEEE CoG 2020]:
* All the
  [game code](https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/Assets/Scripts/BoardGame)
  is Unity-independent. However, automated testing of AIs via console is
  not yet implemented (and will only be so if the proposal is accepted).

### Testing the AI in Unity

A Unity project implementing this board game is included in the repository,
and can be used as a visually friendly way to test the AI.
The project should be executed within the Unity editor, not as a standalone
build. Project execution can be configured by manipulating the
`SessionConfiguration` game object in the Unity Editor. This is done by: 1)
editing the fields of the [`SessionController`] script; and, 2) adding or
removing AI scripts, i.e., scripts which extend [`AIPlayer`] (see the
[An additional class for Unity](#an-additional-class-for-unity) section).

![game](https://user-images.githubusercontent.com/3018963/72279861-f250d280-362e-11ea-9c8a-9244dad16f11.jpg)

#### Fields of the `SessionController` game object

Fields of the [`SessionController`] script are divided in three sections:

1. **Match properties** - Board dimensions, win conditions, initial number of
   pieces per player and last move animation length in seconds.
2. **AI properties** - AI time limit in seconds and minimum AI game move time.
3. **Tournament properties** - Points per win, draw, loss, and information
   screen blocking and duration options.

Tournaments occur automatically if there are more than two AI scripts active in
the `SessionConfiguration` game object. Otherwise a single match is played,
as discussed in the next section.

#### Adding and removing AI scripts

Zero or more AI scripts can be added to the `SessionConfiguration` game
object. These scripts extend the [`AIPlayer`] class, as discussed in the
following section. The number of active AI
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

#### An additional class for Unity

For the AI to be tested in Unity, an additional class, which extends
[`AIPlayer`], must be implemented. This class allows an AI to be found and
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
these fields should be private and have the [`SerializeField`] attribute.

The time limit is available in the `AITimeLimit` property, and can be passed to
the thinker during its instantiation in the `Setup()` method.

## Licenses

This assignment (the text and non-code files) are made available under the
[Mozilla Public License 2.0][MPLv2]. The code is made available under the
[Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International
License][CC BY-NC-SA 4.0].

## Metadata

* Author: [Nuno Fachada]
* Institution: [Universidade Lusófona de Humanidades e Tecnologias][ULHT]

[MPLv2]:https://opensource.org/licenses/MPL-2.0
[CC BY-NC-SA 4.0]:https://creativecommons.org/licenses/by-nc-sa/4.0/
[licvideo]:https://www.ulusofona.pt/en/undergraduate/videogames
[IEEE CoG 2020]:http://ieee-cog.org/2020/
[Nuno Fachada]:https://github.com/fakenmc
[ULHT]:https://www.ulusofona.pt/
[Simplexity]:https://boardgamegeek.com/boardgame/55810/simplexity
[Connect Four]:https://www.boardgamegeek.com/boardgame/2719/connect-four
[EuroMillions]:https://www.euro-millions.com/
[.NET Standard 2.0]:https://docs.microsoft.com/en-us/dotnet/standard/net-standard
[.NET Core 2.0]:https://dotnet.microsoft.com/download
[Unity]:https://unity.com/
[`AIPlayer`]:Assets/Scripts/AI/AIPlayer.cs
[`IThinker`]:Assets/Scripts/AI/IThinker.cs
[`Board`]:Assets/Scripts/BoardGame/Board.cs
[`FutureMove`]:Assets/Scripts/FutureMove.cs
[`SessionController`]:Assets/Scripts/SessionController.cs
[`Assets/Scripts/AI/AIs/`]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/Assets/Scripts/AI/AIs
[`CancellationToken`]:https://docs.microsoft.com/dotnet/api/system.threading.cancellationtoken
[`SerializeField`]:https://docs.unity3d.com/ScriptReference/SerializeField.html
