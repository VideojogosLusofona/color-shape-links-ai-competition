<!--
Board Game AI 2019/2020 (c) by Nuno Fachada

Board Game AI 2019/2020 is licensed under a Creative Commons
Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
-->

# Board Game AI

An assignment for the Artificial Intelligence course unit

*Under construction*

## Introduction

*Under construction*

## Goals

The main goal of this project is to implement a competitive AI for the
board game included in this repository, the rules of which are similar to the
[Simplexity] board game. However, this board game can played with arbitrary
number of rows and columns, as well as accepting different number of pieces in
a row for achieving victory.

## How to

Students should implement a minimum of two classes. For example, if your AI is
called *G03VerySmart*, these classes are as follows:

1. **`G03VerySmartAI`**, which extends [`AIPlayer`]. This class should be added
   as a component of the `SessionController` game object, and allows your AI
   to be found by the game.
2. **`G03VerySmartAIThinker`**, which implements the [`IThinker`] interface.
   This is were you should actually implement the AI.

These classes should be in their own folder, `G03VerySmart`, which in turn
should be placed at [`Assets/Scripts/AI/AIs/`]. This folder contains some
examples of dumb AIs to demonstrate how your project should be organized.

### The class that extends `AIPlayer`

This class allows your AI to be found by the game. For that purpose, it must
be added as a component of the `SessionController` game object, and implement
the following read-only properties:

1. `PlayerName`, which should return the _string_ with the AI's name.
2. `Thinker`, which should return an instance of [`IThinker`]. This instance
   should be created in `Awake()`.

### The class that implements `IThinker`

This class is where is AI should be implemented. The [`IThinker`] interface
defines the `Think()` method, which accepts the [game board][`Board`] and a
[cancellation token][`CancellationToken`], returning a [`FutureMove`]. Simply
put, the method accepts the game board, the AI decides the best move to
perform, and returns that move, which will eventually be executed by the game
engine.

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