<!--
ColorShapeLinks AI 2020 (c) by Nuno Fachada

ColorShapeLinks AI 2020 is licensed under a Creative Commons
Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
-->

# ColorShapeLinks AI competition

![Build](https://github.com/VideojogosLusofona/color-shape-links-ai-competition/actions/workflows/dotnet.yml/badge.svg)
[![LGPL Licence](https://img.shields.io/badge/license-MPLv2-blue.svg)](https://opensource.org/licenses/MPL-2.0)
[![Supported platforms](https://img.shields.io/badge/platform-windows%20%7C%20macos%20%7C%20linux-orange.svg)](https://en.wikipedia.org/wiki/Cross-platform)

_An AI competition for the [IEEE CoG 2021] conference_

[Important dates (updated!)](#important-dates) | [Daily standings][Standings] |
[Implementation guide][APIDocs] |
[Tutorial video](https://www.youtube.com/watch?v=ELrsLzX3qBY)

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
the challenges for the AI are multifold.

AI agents must be implemented in C# ([.NET Standard 2.0]) by extending
[one class][`AbstractThinker`] and overriding [one method][`Think()`].
The development framework includes both console and [Unity] frontends
([.NET Core 3.1] and [Unity 2019.4 LTS], respectively) and can be downloaded
with the following command (requires [Git] and [Git LFS]):

`git clone --recurse-submodules https://github.com/VideojogosLusofona/color-shape-links-ai-competition.git`

If you did a regular `clone` and are missing the submodule folders, these can be
populated and/or updated with:

`git submodule update --init --recursive`

The [implementation guide][APIDocs] contains the required documentation for
developing an AI agent for ColorShapeLinks. Th

## The competition

### Tracks and prize money

The competition runs on two distinct tracks:

1. The **Base Track** competition will be played using standard [Simplexity]
   rules (6x7 board, 4 pieces in a row for victory) and with a time limit of
   0.2 seconds. Only one processor core will be available for the AIs.
2. The **Unknown Track** competition will be played on a multi-core
   processor under conditions that will only be revealed after the competition
   deadline. These conditions will be derived from the [EuroMillions] draw that
   will take place on July 30, 2021, as follows:
   * NumberOfRows = Lowest [EuroMillions] ball number higher than 6
     (ascending order).
   * NumberOfCols = Next [EuroMillions] ball (ascending order).
   * WiningSequenceLength = Ceil(NumberOfRows / 2.0).
   * InitialNumberOfRoundPiecesPerPlayer = Floor(NumberOfRows * NumberOfCols / 4.0)
   * InitialNumberOfSquarePiecesPerPlayer = Ceil(NumberOfRows * NumberOfCols / 4.0)
   * TimeLimit (milliseconds) = 25 * Max(NumberOfRows, NumberOfCols)

The winner of each track will receive a prize money of **$500 (USD)**,
sponsored by the [IEEE CIS Competition Subcommittee].

### Standings

All AI agents will play against each other two times, so each agent has the
opportunity to play first. Players will be awarded 3 points per win, 1 point
per draw and 0 points per loss. The [standings] for each track will be based
on the total number of points obtained per AI, sorted in descending order.

Tie-breaks are performed only when there are two or more AIs with the same
points in first place. In such cases, ties are solved according to the greatest
number of points obtained in the matches between AIs with the same points. If
the tie persists, the AIs are considered officially tied and *ex aequo* winners
of the competition.

Standings for the base track and two other test configurations are [updated
daily][standings]. These offer a good idea of how each AI is faring. However,
there are two issues which may impact final results:

1. The actual competition will run on a slightly more powerful computer,
   with possibly newer software (e.g., a more recent OS kernel or
   [.NET] version).
2. After the [first deadline](#important-dates), submissions may require
   changes in order to [comply with the rules](#rules-for-the-ai-code).

### Submissions

AI agents should be submitted via email to
[colorshapelinks@ulusofona.pt](mailto:colorshapelinks@ulusofona.pt). Only one
agent is allowed per team, but multiple submissions are encouraged (within
reasonable frequency) as the agent is being developed and improved. Upon
submission, the submitted code is:

1. Superficially checked for [rule](#rules-for-the-ai-code) compliance,
   though not otherwise studied or analysed. Thorough tests will be performed
   only after the [first deadline for AI submissions](#important-dates).
2. Added to the automated competitions, the [standings] of which are
   updated daily.

All submitted codes are considered private and will not be shared, discussed or
analyzed by the organization, [except for rule compliance
purposes](#rules-for-the-ai-code).

### Important dates

* **May 28** _(optional)_ - [IEEE CoG competition papers deadline], e.g., for
  participants that wish to submit a paper about their entry for this
  competition.
* **~~July 15~~ July 29** - First deadline for AI submissions. Submissions will be
  thoroughly checked for [rule compliance](#rules-for-the-ai-code) and
  technical issues.
* **~~July 20~~ August 3** - Author notification of any issues encountered with the
  submissions.
* **~~July 25~~ August 9** - Final deadline for AI submissions + short description of the
  methods used for implementing the AI.
* **~~July 30~~ August 10** - [EuroMillions] draw which will determine the conditions for
  the [Unknown track](#tracks).
* **August 17-20** - [IEEE CoG 2021] takes place, final competition
  results are announced.

These dates are
[anywhere in the world](https://www.timeanddate.com/time/zones/aoe).

## Rules

### Rules for the AI code

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

### General competition rules

- Submissions automatically compete in both tracks with the same [setup]
  parameters (if any are specified).
- An individual may only be affiliated with one team, except for teachers or
  supervisors of student teams, which may be affiliated with more than one
  team.
- Attending [IEEE CoG 2021] is not mandatory for participating in the
  competition. However, all participants are invited to submit a
  [competition paper][IEEE CoG competition papers deadline] about their entry
  (in which case, please [cite this publication][paper]).

## Resources and guides

* [Implementation guide][APIDocs]
* [Tutorial video](https://www.youtube.com/watch?v=ELrsLzX3qBY)
* [Standings (updated daily)][standings]
* [ColorShapeLinks: A board game AI competition for educators and students][paper]

## Licenses

The code is made available under the [Mozilla Public License 2.0][MPLv2].
All the text and documentation (i.e., non-code files) are made available under
the [Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International
License][CC BY-NC-SA 4.0].

## Organization

* [Nuno Fachada], [COPELABS], [Lusófona University][ULHT], Portugal
* Contact us by [e-mail](mailto:colorshapelinks@ulusofona.pt) or open
  [an issue](https://github.com/VideojogosLusofona/color-shape-links-ai-competition/issues)

[Git]:https://git-scm.com/downloads
[Git LFS]:https://git-lfs.github.com/
[APIDocs]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/index.html
[MPLv2]:https://opensource.org/licenses/MPL-2.0
[CC BY-NC-SA 4.0]:https://creativecommons.org/licenses/by-nc-sa/4.0/
[licvideo]:https://www.ulusofona.pt/en/undergraduate/videogames
[IEEE CoG 2021]:http://ieee-cog.org/2021/
[Nuno Fachada]:https://github.com/fakenmc
[ULHT]:https://www.ulusofona.pt/
[Simplexity]:https://boardgamegeek.com/boardgame/55810/simplexity
[Connect Four]:https://www.boardgamegeek.com/boardgame/2719/connect-four
[EuroMillions]:https://www.euro-millions.com/
[.NET Standard 2.0]:https://docs.microsoft.com/dotnet/standard/net-standard
[.NET Core 3.1]:https://dotnet.microsoft.com/download/dotnet/3.1
[Unity]:https://unity.com/
[Unity 2019.4 LTS]:https://unity.com/releases/2019-lts
[ossl]:https://opensource.org/licenses
[`unsafe`]:https://docs.microsoft.com/dotnet/csharp/programming-guide/unsafe-code-pointers/
[`CancellationToken`]:https://docs.microsoft.com/dotnet/api/system.threading.cancellationtoken
[standings]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/standings
[COPELABS]:http://copelabs.ulusofona.pt/
[.NET]:https://dotnet.microsoft.com/download
[`AbstractThinker`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_common_1_1_a_i_1_1_abstract_thinker.html
[`Think()`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_common_1_1_a_i_1_1_abstract_thinker.html#ac8039cba1e4ececb04322fb8e7610f0e
[setup]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/thinker-implementation-guide.html#autotoc_md3
[IEEE CoG competition papers deadline]:https://ieee-cog.org/2021/#call_papers
[IEEE CIS Competition Subcommittee]:https://cis.ieee.org/conferences/student-games-based-competition
[paper]:https://www.sciencedirect.com/science/article/pii/S2666920X21000084

