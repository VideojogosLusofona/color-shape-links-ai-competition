/// @file
/// @brief This file contains the ::IPlayer interface.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

/// <summary>
/// Defines a player for the *ColorShapeLinks* board game.
/// </summary>
public interface IPlayer
{
    /// <summary>Is the player human?</summary>
    /// <value>`true` if the player is human, `false` otherwise.</value>
    bool IsHuman { get; }

    /// <summary>Name of the player.</summary>
    /// <value>A string containing the name of the player.</value>
    string PlayerName { get; }

    /// <summary>The player's thinker.</summary>
    /// <value>An instance of <see cref="IThinker"/>.</value>
    IThinker Thinker { get; }
}
