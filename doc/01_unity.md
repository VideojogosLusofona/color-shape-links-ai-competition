# Unity guide

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