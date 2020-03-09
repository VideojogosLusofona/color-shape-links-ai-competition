# Thinker implementation guide {#thinker-implementation-guide}

@brief A guide on how to implement an AI thinker

[TOC]

## Introduction

This guide describes how to implement an AI thinker for the ColorShapeLinks
competition. This is a simple process, requiring at the bare minimum
the creation of a class which extends [another class][AbstractThinker]
and overrides [one of its methods][Think()]. The @ref quick-start "Quick start"
guide shows how to get started quickly by simply dropping the new AI class in
the [console app][console_folder] or [Unity app][unity_folder] folders.
However, implementing a competitive AI thinker will most likely require to
know a bit more about the @ref rules "source code rules and restriction"
and about the @ref abstractthinker "AbstractThinker base class". Setting up a
proper @ref devenv "development environment" and being able to adequately
@ref testing "test" the developed code is also essential. This guide
closes with a tutorial on how to implement a @ref minimax "basic Minimax AI"
with a very simple heuristic.

## Rules and restrictions for the AI source code {#rules}

The source code of AI thinkers must follow these rules and restrictions:

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
  [AbstractThinker] class or passed to the [Think()] method, e.g., such as
  using reflection to probe the capabilities of its opponents.
- Cannot use more than 2GB of memory during the course of a match.
- Cannot be more than 250kb in size (including libraries, excluding comments).
- Cannot save or load data from disk.

## The AbstractThinker base class {#abstractthinker}

<!--Describe what information is available in this class, beware of testing it in
isolation, what methods and properties can be overriden-->

The first step to implement an AI thinker is to extend the [AbstractThinker]
base class. This class has three overridable methods, but it's only
mandatory to override one of them, as shown in the following table:

| Method       | Mandatory override? | Purpose                           |
| ------------ | ------------------- | --------------------------------- |
| [Setup()]    | No                  | Setup the AI thinker.             |
| [Think()]    | Yes                 | Perform a move.                   |
| [ToString()] | No                  | Return the AI thinker's name.     |

There's also the non-overridable [OnThinkingInfo()] method, which can be
invoked for producing thinking information, mainly for debugging purposes.
In the [Unity] frontend this information is printed on Unity's console, while
in the [console] frontend the information is forwarded to the registered
@ref ColorShapeLinks.TextBased.Lib.IThinkerListener "thinker listeners".

Classes extending [AbstractThinker] also inherit a number of useful read-only
properties, namely board/match configuration properties
(@ref ColorShapeLinks.Common.AI.AbstractThinker.Rows "No. of rows",
@ref ColorShapeLinks.Common.AI.AbstractThinker.Cols "No. of columns",
@ref ColorShapeLinks.Common.AI.AbstractThinker.WinSequence "No. of pieces in sequence to win a game",
@ref ColorShapeLinks.Common.AI.AbstractThinker.RoundsPerPlayer "No. of initial round pieces per player"
and
@ref ColorShapeLinks.Common.AI.AbstractThinker.SquaresPerPlayer "No. of initial square pieces per player")
and the
@ref ColorShapeLinks.Common.AI.AbstractThinker.TimeLimitMillis "time limit for the AI to play".
Concerning the board/match configuration properties, these are also
available in the @ref ColorShapeLinks.Common.Board "board" object given as a
parameter to the [Think()] method. However, the [Setup()] method can only
access them via the inherited properties.

The following subsections address the overriding of each of these three
methods.

### Overriding the Setup() method

If an AI thinker needs to be configured before starting to play, the [Setup()]
method is the place to do it. This method receives a single argument,
a `string`, which can contain thinker-specific parameters, such as maximum
search depth, heuristic to use, and so on. It is the thinker's responsibility
to parse this string. In the [Unity] frontend, the string is specified in the
"Thinker params" field of the @ref ColorShapeLinks.UnityApp.AIPlayer "AIPlayer"
component. When using the [console] frontend, the string is passed via the
`--white/red-params` option for simple matches, or after the thinker's
fully qualified name in the configuration file of a complete session.
The same AI thinker can represent both players in matches and more than
one player in sessions, and can be configured either with the same or with
different parameters. In the latter case, it might be useful to also
override the [ToString()] method for differentiating between particular
configurations of the same thinker during matches and sessions.

Besides the parameters string, the [Setup()] method also has access to
board/match properties inherited from the [base class][AbstractThinker]. In
any case, concrete AI thinkers do not need to provide an implementation of
this method if they are not parameterizable.

### Overriding the Think() method

Mandatory, uses board and cancellation token

### Overriding the ToString() method

By default, the [ToString()] method removes the namespace from the thinkers'
fully qualified name, as well as the "thinker", "aithinker" or "thinkerai"
suffixes. However, this method can be overriden in order to behave
differently. One such case is when thinkers are parameterizable, and
differentiating between specific parametrizations during matches and sessions
becomes important. For example, if an AI thinker is configurable with a
`maxDepth` parameter, the following would keep the base method behavior,
while adding the value of the maximum depth to the thinkers' name:

```cs
// If the FQN of this class is My.SuperAIThinker and maxDepth is 6, this
// method will return "SuperD6".
public override string ToString()
{
    return base.ToString() + "D" + maxDepth;
}
```

In any case, concrete AI thinkers are not required to override this method.

## Setting up the development environment {#devenv}

Quick methods and best practices

## Testing an AI thinker {#testing}

###  In the console app

TODO: The quick way is already explained. Here we discuss proper development,
with separate folder/repo.

TODO: Run match/session with external assembly/thinker.

### Testing the AI thinker in isolation

TODO: Warn about thinker variables not being available when thinker is
instantiated directly.

## Implementing a simple Minimax player {#minimax}

Steps to create a minimax player

[.NET Standard 2.0]:https://docs.microsoft.com/dotnet/standard/net-standard
[ossl]:https://opensource.org/licenses
[`unsafe`]:https://docs.microsoft.com/dotnet/csharp/programming-guide/unsafe-code-pointers/
[`CancellationToken`]:https://docs.microsoft.com/dotnet/api/system.threading.cancellationtoken
[AbstractThinker]:@ref ColorShapeLinks.Common.AI.AbstractThinker
[Think()]:@ref ColorShapeLinks.Common.AI.AbstractThinker.Think
[Setup()]:@ref ColorShapeLinks.Common.AI.AbstractThinker.Setup
[ToString()]:@ref ColorShapeLinks.Common.AI.AbstractThinker.ToString
[OnThinkingInfo()]:@ref ColorShapeLinks.Common.AI.AbstractThinker.OnThinkingInfo
[Unity]:@ref unity-guide
[console]:@ref console-guide
[console_folder]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/ConsoleApp/ColorShapeLinks/TextBased
[unity_folder]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition/tree/master/UnityApp/Assets/Scripts
