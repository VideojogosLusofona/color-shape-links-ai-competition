# Quick start {#quick-start}

@brief Quick start guide

[TOC]

@remark All instructions in these guides are cross-platform and work in Linux,
Windows, and macOS, requiring only that [.NET Core] and/or [Unity] are
installed. On Windows, when not using Git Bash, replace slashes `/` with
backslashes `\` when referencing local paths.

@htmlonly
<iframe width="560" height="315" src="https://www.youtube.com/embed/ELrsLzX3qBY" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
@endhtmlonly

## Downloading the development framework

The development framework **must** be downloaded with the following
command (requires [Git] ≥ 2.13 and [Git LFS], other approaches will not work):
```text
git clone --recurse-submodules https://github.com/VideojogosLusofona/color-shape-links-ai-competition.git
```

## Quick AI implementation tips

_The complete AI implementation guide is available [here][thinker-guide]._

At least one class is required for implementing an AI thinker. This class
must extend [AbstractThinker] and implement the [Think()] method. This
method accepts the [game board][Board] and a
[cancellation token][`CancellationToken`], returning a [FutureMove]. Simply
put, the method accepts the game board, the AI decides the best move to
perform, and returns that move, which will eventually be executed by the match
engine.

The [Think()] method is called in a separate thread. As such, it should not
try to access shared data. The main thread may ask the AI to stop *thinking*,
for example if the thinking time limit has expired. Thus, while *thinking*,
the AI should frequently test if a cancellation request was made to the
[cancellation token][`CancellationToken`]. If so, it should return immediately
with no move performed, as exemplified in the following code:

```cs
if (ct.IsCancellationRequested) return FutureMove.NoMove;
```

The thinker can freely modify the [game board][Board], since this is a copy
and not the original game board being used in the main thread. More
specifically, the thinker can try moves with the [DoMove()] method, and
cancel them with the [UndoMove()] method. The board keeps track of the move
history, so the thinker can perform any sequence of moves, and roll them back
afterwards.

The [CheckWinner()] method is useful to determine if there is a winner. If
there is one, the solution is placed in the method's optional parameter.

For building heuristics, the public read-only variable [winCorridors] will
probably be useful. This variable is a collection containing all corridors
(sequences of positions) where promising or winning piece sequences may exist.

## Testing the AI in the console {#testing-the-ai-in-the-console}

_The complete console testing guide is available [here][console-guide]._

1. Place the AI thinker class or classes in
   `ConsoleApp/ColorShapeLinks/TextBased/App`.
2. Open a terminal or console and `cd` into the
   `ConsoleApp/ColorShapeLinks/TextBased/App` folder.
3. Execute the command `dotnet run -- info` to check if the AI thinker is
   found by the ColorShapeLinks console app.
   * **Tip**: The `dotnet run -- help` lists the high-level command line
     options available.
4. Assuming the fully qualified name (namespace + class name) of the AI thinker
   class is `MyAI.MyThinker`, the AI thinker, playing as the first player
   (White), can be tested against a random AI (playing as Red), with a play
   limit of one second, with
   `dotnet run -- match -W MyAI.MyThinker -R ColorShapeLinks.Common.AI.Examples.RandomAIThinker -t 1000`.
   * **Tip**: The `dotnet run -- help match` presents all the sub-options for
     the `match` option.
5. To run a competition, create a text file named `competition.txt` with the
   following contents:
   ```text
   MyAI.MyThinker
   ColorShapeLinks.Common.AI.Examples.RandomAIThinker
   ColorShapeLinks.Common.AI.Examples.SequentialAIThinker
   ```
   and execute `dotnet run -- session -g competition.txt`.
   * **Tip**: The `dotnet run -- help session` presents all the sub-options
     for the `session` option.

## Testing the AI in Unity {#testing-the-ai-in-unity}

_The complete Unity testing guide is available [here][unity-guide]._

1. Place the AI thinker class or classes in `UnityApp/Assets/Scripts`.
2. Open the `UnityApp` project in Unity.
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

[Git]:https://git-scm.com/downloads
[Git LFS]:https://git-lfs.github.com/
[.NET Core]:https://dotnet.microsoft.com/download
[Unity]:https://unity.com/
[`CancellationToken`]:https://docs.microsoft.com/dotnet/api/system.threading.cancellationtoken
[AbstractThinker]:@ref ColorShapeLinks.Common.AI.AbstractThinker
[Think()]:@ref ColorShapeLinks.Common.AI.AbstractThinker.Think
[Board]:@ref ColorShapeLinks.Common.Board
[FutureMove]:@ref ColorShapeLinks.Common.AI.FutureMove
[DoMove()]:@ref ColorShapeLinks.Common.Board.DoMove
[UndoMove()]:@ref ColorShapeLinks.Common.Board.UndoMove
[CheckWinner()]:@ref ColorShapeLinks.Common.Board.CheckWinner
[winCorridors]:@ref ColorShapeLinks.Common.Board.winCorridors
[AIPlayer]:@ref ColorShapeLinks.UnityApp.AIPlayer
[thinker-guide]:@ref thinker-implementation-guide
[unity-guide]:@ref unity-guide
[console-guide]:@ref console-guide
