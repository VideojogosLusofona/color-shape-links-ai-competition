<!--
ColorShapeLinks AI 2020 (c) by Nuno Fachada

ColorShapeLinks AI 2020 is licensed under a Creative Commons
Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
-->

# ColorShapeLinks AI competition

This competition has been accepted for the [IEEE CoG 2020] conference and is
officially open!

* What's ready:
  * Introductory documentation (this document), with quick start tutorials for
    testing your AI using the [console](#testing-the-ai-in-the-console) or with
    [Unity](#testing-the-ai-in-unity).
  * The development framework  (see
    [how to download it](#downloading-the-development-framework)) and its
    [API][APIDocs].
  * Running daily competition with [Base Track](#tracks) settings and automated
    upload of [classification and results][standings].
* What's not:
  * How to video.
  * Detailed guide for implementing the [thinker AI][thinker-guide], as well as
    how to and best practices guides for testing it in the
    [console][console-guide] and [Unity][unity-guide].

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
   0.2 seconds. Only one processor core will be available for the AIs.
2. The **Unknown Track** competition will be played on a multi-core
   processor under conditions that will only be revealed after the competition
   deadline. These conditions will be derived from the first [EuroMillions]
   draw that takes place after the deadline, most likely at May 19, 2020, as
   follows:
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
0 points per loss. The [classification][standings] for each track will be based
on the total number of points obtained per AI, sorted in descending order.

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

Classification for the base track is updated daily, and is available
[here][standings].

### Submissions

AIs should be submitted via email to
[colorshapelinks@ulusofona.pt](mailto:colorshapelinks@ulusofona.pt). Only one
AI is allowed per team, but multiple submissions are encouraged as the AI is
being developed and improved. Upon submission, the submitted code is:

1. Checked and tested for [rule](#rules-for-the-ai-code) compliance, though
   not otherwise studied or analysed.
2. Added to the base track competition, the [classification][standings] of
   which is updated daily.

All submitted codes are considered private and privileged and will not be
shared, discussed or analyzed by the organization before the competition
deadline. After this deadline, all the submitted codes will be made public at
this repository. If code authors do not specify a
[valid open source license][ossl] upon submission, the organization will make
the code available under the [Mozilla Public License 2.0][MPLv2].

## The AI code

### Overview

_For a complete AI implementation guide, check out this [link][thinker-guide]._

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

For building heuristics, the public read-only variable [`winCorridors`] will
probably be useful. This variable is a collection containing all corridors
(sequences of positions) where promising or winning piece sequences may exist.

### Rules for the AI code

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
  [`AbstractThinker`] class or passed to the [`Think()`] method, e.g., such as
  using reflection to probe the capabilities of its opponents.
- Cannot use more than 2GB of memory during the course of a match.

### Quickly testing your AI

The following sections describe quick ways of testing your AI in the
[console](#testing-the-ai-in-the-console) or [Unity](#testing-the-ai-in-unity).
For proper development check the complete [console][console-guide] and
[Unity][unity-guide] guides.

All instructions are cross-platform and work in Linux, Windows, and macOS,
requiring only that [.NET Core] and/or [Unity] are installed.

#### Downloading the development framework

The development framework **must** be downloaded with the following
command (requires [Git] ≥ 2.13 and [Git LFS], other approaches will not work):
```text
git clone --recurse-submodules https://github.com/VideojogosLusofona/color-shape-links-ai-competition.git
```

#### Testing the AI in the console

_For a complete console how to, check out this [guide][console-guide]._

1. Place your AI class or classes in `ConsoleApp/ColorShapeLinks/TextBased/App`.
2. Open a terminal or console and `cd` into the
   `ConsoleApp/ColorShapeLinks/TextBased/App` folder.
3. Execute the command `dotnet run -- info` to check if your AI is found by
   the _ColorShapeLinks_ console app.
   * The `dotnet run -- help` lists the high-level command line options
     available.
4. Assuming the fully qualified name (namespace + class name) of your AI class
   is `MyAI.MyThinker`, the AI, playing as the first player (White), can be
   tested against a random AI (playing as Red), with a play limit of one
   second, with
   `dotnet run -- match -W MyAI.MyThinker -R ColorShapeLinks.Common.AI.Examples.RandomAIThinker -t 1000`.
   * The `dotnet run -- help match` presents all the sub-options for the
     `match` option.
5. To run a competition, create a text file named `competition.txt` with the
   following contents:
   ```text
   MyAI.MyThinker
   ColorShapeLinks.Common.AI.Examples.RandomAIThinker
   ColorShapeLinks.Common.AI.Examples.SequentialAIThinker
   ```
   and execute `dotnet run -- session -g competition.txt`.
   * The `dotnet run -- help session` presents all the sub-options for the
     `session` option.

#### Testing the AI in Unity

_For a complete Unity how to, check out this [guide][unity-guide]._

1. Place your AI class or classes in `UnityApp/Assets/Scripts`.
2. Open the `UnityApp` project in Unity 2019.2.x or newer.
3. Open the **MainScene** scene in the `Assets/Scenes` folder.

   ![unity01](https://user-images.githubusercontent.com/3018963/74774639-04580d80-528c-11ea-914a-5dab8f91b390.png)

4. In the Hierarchy tab, select the **SessionConfiguration** game object.

   ![unity02](https://user-images.githubusercontent.com/3018963/74774641-04f0a400-528c-11ea-97ec-e86727de2279.png)

5. In the Inspector tab there will be several **AI Player** components already
   attached to the game object. These are simple dummy players for testing
   purposes. Either:
   1. Change the selected thinker in one of the components to your own, or
   2. Add a new **AI Player** component, and then select your thinker.

   ![unity03](https://user-images.githubusercontent.com/3018963/74774643-05893a80-528c-11ea-9b98-9f8dfbb78a02.png)

6. Press the "Play" button.

   ![unity04](https://user-images.githubusercontent.com/3018963/74774644-05893a80-528c-11ea-8a43-b385316563a2.png)


   * If three or more **AI Player** components are active, a competition
     between all AIs will take place.
   * If two **AI Player** components are active, a single match between them
     will take place.
   * If less than two **AI Player** components are active, the missing
     components will be replaced by human players.

## Resources and guides

* [API documentation][APIDocs]
* [Console guide][console-guide]
* [Unity guide][unity-guide]
* [FAQ][faq]
* [Classification and Results for the Base Track (updated daily)][standings]

## Licenses

The code is made available under the [Mozilla Public License 2.0][MPLv2].
All the text and documentation (i.e., non-code files) are made available under
the [Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International
License][CC BY-NC-SA 4.0].

## Organization

* [Nuno Fachada], [HEI-Lab], [Lusófona University][ULHT], Portugal
* Contact us by [e-mail](mailto:colorshapelinks@ulusofona.pt) or open
  [an issue](https://github.com/VideojogosLusofona/color-shape-links-ai-competition/issues)

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
[thinker-guide]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/md_doc_00_thinker.html
[console-guide]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/md_doc_02_console.html
[unity-guide]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/md_doc_05_unity.html
[faq]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/md_doc_10_faq.html
[standings]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/standings
[HEI-Lab]:http://hei-lab.ulusofona.pt/
[.NET Core]:https://dotnet.microsoft.com/download
