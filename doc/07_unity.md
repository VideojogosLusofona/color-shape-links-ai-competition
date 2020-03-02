# Unity guide {#unity-guide}

@brief A guide on how to test an AI thinker in Unity

[TOC]

## Introduction

A [Unity] project implementing this board game is included in the repository,
and can be used as a visually friendly way to test the AI.

![game](https://user-images.githubusercontent.com/3018963/72279861-f250d280-362e-11ea-9c8a-9244dad16f11.jpg)

## Adding an AI thinker to Unity

In order for Unity to find an AI thinker (i.e., a class implementing
[`AbstractThinker`]), the class (or set of classes), should be placed under
the `UnityApp/Assets/Scripts` folder. For organization purposes, it may be
preferable to place the class (or set of classes) in a sub-folder under the
the `UnityApp/Assets/Scripts` folder. For example, if the AI thinker class is
called `AwesomeAIThinker`, then the `AwesomeAIThinker.cs` file (and additional
files, if any), could be placed in `UnityApp/Assets/Scripts/Awesome`.

## Opening the MainScene

The **MainScene** scene contains the Unity frontend for ColorShapeLinks. The
first time the project is opened in Unity, it is necessary to open the
**MainScene**, which is available in the `Assets/Scenes` folder in the Project
tab.

![unity01](https://user-images.githubusercontent.com/3018963/74774639-04580d80-528c-11ea-914a-5dab8f91b390.png)

## Configuring matches and tournaments

ColorShapeLinks matches and/or tournaments should be executed within the Unity
editor, not as standalone builds. These can be configured by manipulating the
`SessionConfiguration` game object. Select this object in the Hierarchy tab as
follows:

![unity02](https://user-images.githubusercontent.com/3018963/74774641-04f0a400-528c-11ea-97ec-e86727de2279.png)

After the `SessionConfiguration` is selected, matches and/or tournaments can be
configured by:

1. Editing the fields of the [`SessionController`] script component.
2. Adding or removing instances of the [`AIPlayer`] component.

The following image shows the components of the `SessionConfiguration` game
object, namely the [`SessionController`] script and several [`AIPlayer`]
components. As can be observed, each [`AIPlayer`] serves as a proxy for an
AI thinker instance.

![image](https://user-images.githubusercontent.com/3018963/75635976-7cbab900-5c12-11ea-8244-b55fdec2fad5.png)

### Fields of the SessionController game object

Fields of the [`SessionController`] script are divided in three sections:

1. **Match properties** - Board dimensions, win conditions, initial number of
   pieces per player and last move animation length in seconds.
2. **AI properties** - AI time limit in seconds and minimum AI game move time.
3. **Tournament properties** - Points per win, draw, loss, and information
   screen blocking and duration options.

Tournaments occur automatically if there are more than two AI scripts active in
the `SessionConfiguration` game object. Otherwise a single match is played,
as discussed in the next section.

### Adding and removing AIPlayer instances

An instance of the [`AIPlayer`] component represents one AI thinker. Zero or
more instances of this component can be added to the `SessionConfiguration`
game object. Instances of this component have the following configurable
fields:

* **Is Active**: Specifies if the selected AI thinker is active.
* **Selected Thinker**: The AI thinker represented by this component instance,
  selected from a list of known AI thinkers (i.e., classes extending
  [`AbstractThinker`]).
* **Thinker Params**: optional thinker-specific parameters (e.g. maximum search
  depth), which are passed to the thinker's [`Setup()`] method.

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

## Running a match or tournament

To start a match or tournament with the active [`AIPlayer`] instances, press
the "Play" button:

![unity04](https://user-images.githubusercontent.com/3018963/74774644-05893a80-528c-11ea-8a43-b385316563a2.png)

[Unity]:https://unity.com/
[`AbstractThinker`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_common_1_1_a_i_1_1_abstract_thinker.html
[`SessionController`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_unity_app_1_1_session_controller.html
[`AIPlayer`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_unity_app_1_1_a_i_player.html
[`Setup()`]:https://videojogoslusofona.github.io/color-shape-links-ai-competition/docs/html/class_color_shape_links_1_1_common_1_1_a_i_1_1_abstract_thinker.html#aa0f63b1df3274e6ef69f4a060869d7c0
