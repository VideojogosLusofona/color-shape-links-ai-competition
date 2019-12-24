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
number of rows and columns, as well as accepting different number of pieces for
achieving victory.

## How to

Students should implement a minimum of two classes. If your AI is called
*VerySmart*, the classes are as follows:

* `VerySmartAI`, which extends [`AIPlayer`].
* `VerySmartAIThinker`, which implements [`IThinker`].

These classes should be in their own folder, which in turn should be placed at
[`Assets/Scripts/AI/AIs/`]. This folder contains some examples of dumb AIs to
demonstrate how this works.

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
[`Assets/Scripts/AI/AIs/`]:Assets/Scripts/AI/AIs/