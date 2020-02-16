<!--
ColorShapeLinks AI 2020 (c) by Nuno Fachada

ColorShapeLinks AI 2020 is licensed under a Creative Commons
Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
-->

# ColorShapeLinks AI competition

This competition has been accepted for the [IEEE CoG 2020] conference!
Important notes:

* The development framework **must** be downloaded with the following
 command (requires [Git] ≥ 2.13 and [Git LFS], other approaches will not work):
  ```text
  git clone --recurse-submodules https://github.com/VideojogosLusofona/color-shape-links-ai-competition.git
  ```

* The API documentation is finalized and available [here][APIDocs].
* This document and a "How to" video are under development and will be
  available in a few days.

## Description

ColorShapeLinks is an AI competition for the [Simplexity] board game with
arbitrary game dimensions. The first player to place *n* pieces of the same
type in a row wins. In this regard, the base game, with a 6 x 7 board and
_n_ = 4, is similar to [Connect Four]. However, pieces are defined
not only by color, but also by shape: round or square. Round
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

### Submitting your AI

_Work in progress_

## The AI code

<!--* [Downloading the development framework](#downloading-the-development-framework)-->
* [Overview](#overview)
* [Rules](#rules)
* [Testing the AI in the console](testing-the-ai-in-the-console)
* [Testing the AI in Unity](testing-the-ai-in-unity)

<!--### Downloading the development framework

To download/clone the development framework, i.e., the code in this repository,
use the following Git command:

```text
git clone --recurse-submodules https://github.com/VideojogosLusofona/color-shape-links-ai-competition.git
```

Note that downloading the ZIP file or performing a regular clone will not work.
Additionally, it requires [Git] 2.13 or later and [Git LFS].
-->

### Overview

Competition code must be implemented in C# and restrict itself to
cross-platform [.NET Standard 2.0] API calls.

At least one class is required for the AI to work. This class should extend
[`AbstractThinker`] and implement the [`Think()`] method. This method accepts
the [game board][`Board`] and a [cancellation token][`CancellationToken`],
returning a [`FutureMove`]. Simply put, the method accepts the game board,
the AI decides the best move to perform, and returns that move, which will
eventually be executed by the match engine.

The [`Think()`] method is called in a separate thread. As such, it should not
try to access shared data. The main thread may ask the AI to stop *thinking*,
for example if the thinking time limit has expired. Thus, while *thinking*,
the AI should frequently test if a cancellation request was made to the
[cancellation token][`CancellationToken`]. If so, it should return immediately
with no move performed, as exemplified in the following code:

```cs
if (ct.IsCancellationRequested) return FutureMove.NoMove;
```

The thinker can freely modify the [game board][`Board`], since this is a copy
and not the original game board being used in the main thread. More
specifically, the thinker can try moves with the [`DoMove()`] method, and
cancel them with the [`UndoMove()`] method. The board keeps track of the move
history, so the thinker can perform any sequence of moves, and roll them back
afterwards.

The [`CheckWinner()`] method is useful to determine if there is a winner. If
there is one, the solution is placed in the method's optional parameter.

For building heuristics, the public read-only variable [`winCorridors`] might
be important. This variable is a collection containing all corridors
(sequences of positions) where promising or winning piece sequences may exist.

### Rules for the AI code

* Can only use cross-platform [.NET Standard 2.0] API calls in C#.
* Can use additional libraries which abide by these same rules.
* Both the AI code and libraries must be made available under a
  [valid open source license][ossl], although AI codes can be open-sourced
  only after the competition deadline.
* Must run in the same process that invokes it.
* Can be multithreaded and use [`unsafe`] contexts.
* Cannot *think* in its opponent time (e.g., by using a background thread).
* Can only probe the environment for the number of processor cores. It cannot
  search or use any other information, besides what is already available in the
  [`AbstractThinker`] class or passed to the [`Think()`] method, e.g., such as
  using reflection to probe the capabilities of its opponents.
* Cannot use more than 2GB of memory during the course of a match.

### Testing the AI in the console

_Work in progress_

### Testing the AI in Unity

A Unity project implementing this board game is included in the repository,
and can be used as a visually friendly way to test the AI.
The project should be executed within the Unity editor, not as a standalone
build. Project execution can be configured by manipulating the
`SessionConfiguration` game object in the Unity Editor. This is done by: 1)
editing the fields of the [`SessionController`] script; and, 2) adding or
removing instances of the [`AIPlayer`] component.

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

#### Adding and removing `AIPlayer` instances

An instance of the [`AIPlayer`] component represents one AI thinker. Zero or
more instances of this component can be added to the `SessionConfiguration`
game object. Instances of this component has the following configurable
fields:

* **Is Active**: specifies if the selected AI thinker is active.
* **Selected AI**: the AI thinker represented by this component instance,
  selected from a list of known AI thinkers (i.e., classes extending
  [`AbstractThinker`]).
* **AI Config**: optional thinker-specific parameters (e.g. maximum search
  depth).

The number of active [`AIPlayer`] component instances attached to the
`SessionConfiguration` game object determines what type of session will run:

* Zero active instances: a match between human players will take place.
* One active instance: a game between the AI and a human player will take
  place.
* Two active instances: a game between the two AIs will take place.
* More than two active instances: a **tournament session** will take place,
  where each AI plays against all other AIs twice, one as the first player
  (white), another as the second player (red).

During and after the tournament session, all match results as well as current
standings / classifications, are presented.

## Licenses

This assignment (the text and non-code files) are made available under the
[Mozilla Public License 2.0][MPLv2]. The code is made available under the
[Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International
License][CC BY-NC-SA 4.0].

## Metadata

* Author: [Nuno Fachada]
* Institution: [Lusófona University][ULHT]

[Git]:https://git-scm.com/downloads
[Git LFS]:https://git-lfs.github.com/
[APIDocs]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/index.html
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
[`AbstractThinker`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_common_1_1_a_i_1_1_abstract_thinker.html
[`Think()`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_common_1_1_a_i_1_1_abstract_thinker.html#ac8039cba1e4ececb04322fb8e7610f0e
[`Board`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_common_1_1_board.html
[`FutureMove`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/struct_color_shape_links_1_1_common_1_1_a_i_1_1_future_move.html
[ossl]:https://opensource.org/licenses
[`unsafe`]:https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/unsafe-code-pointers/
[`SessionController`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_unity_app_1_1_session_controller.html
[`DoMove()`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_common_1_1_board.html#af97ec0281f2420e4594b1000b609ab73
[`UndoMove()`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_common_1_1_board.html#a4f5022f3b6c72a4bba9fad39f631beee
[`CheckWinner()`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_common_1_1_board.html#a7088451ab7b87b7cac15be65ea521306
[`winCorridors`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_common_1_1_board.html#a518b85b41ceb010c4f7104395977ff85
[`AIPlayer`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_unity_app_1_1_a_i_player.html
[`Assets/Scripts/AI/AIs/`]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/Assets/Scripts/AI/AIs
[`CancellationToken`]:https://docs.microsoft.com/dotnet/api/system.threading.cancellationtoken
[`SerializeField`]:https://docs.unity3d.com/ScriptReference/SerializeField.html
