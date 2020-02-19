/// @namespace ColorShapeLinks
/// @brief The ColorShapeLinks game, an unbounded version of the
/// [Simplexity](https://boardgamegeek.com/boardgame/55810/simplexity) board
/// game.
/// @details This code is organized in three main namespaces:
/// * ColorShapeLinks.Common: @copybrief ColorShapeLinks.Common
/// * ColorShapeLinks.TextBased: @copybrief ColorShapeLinks.TextBased
/// * ColorShapeLinks.UnityApp: @copybrief ColorShapeLinks.UnityApp

/// @namespace ColorShapeLinks.Common
/// @brief The common code between all ColorShapeLinks implementations.
/// @details In particular, types in this namespace constitute the fundamental
/// parts of ColorShapeLinks games, for example the Board and its Piece's.

/// @namespace ColorShapeLinks.Common.AI
/// @brief Common AI-related types between all ColorShapeLinks implementations.

/// @namespace ColorShapeLinks.Common.AI.Examples
/// @brief Example AI thinker implementations.

/// @namespace ColorShapeLinks.Common.Session
/// @brief Common match and session setup code for ColorShapeLinks games.

/// @namespace ColorShapeLinks.TextBased
/// @brief A console implementation of the ColorShapeLinks board game.

/// @namespace ColorShapeLinks.TextBased.App
/// @brief The console application frontend which runs matches and sessions of
/// the ColorShapeLinks board game.
/// @details This namespace only contains the frontend types, relying on the
/// engine code (at ColorShapeLinks.TextBased.Lib)

/// @namespace ColorShapeLinks.TextBased.Lib
/// @brief Library "engine" code for console or third-party implementations
/// of ColorShapeLinks.
/// @details This namespace provides a backend, i.e., an engine, for running
/// ColorShapeLinks games. It's a middleware between the essencial board game
/// code (under the ColorShapeLinks.Common namespace) and frontend
/// applications, such as the console implementation in the
/// ColorShapeLinks.TextBased.App namespace.

/// @namespace ColorShapeLinks.UnityApp
/// @brief A Unity implementation of the ColorShapeLinks board game.
/// @details Since Unity is a game engine itself, this implementation directly
/// uses the essencial board game code (under the ColorShapeLinks.Common
/// namespace), thus not requiring the ColorShapeLinks.TextBased.Lib engine
/// code.
