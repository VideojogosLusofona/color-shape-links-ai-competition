/// @file
/// @brief This file contains the ::HumanPlayer class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;

/// <summary>
/// Represents a human player, for testing purposes.
/// </summary>
public class HumanPlayer : IPlayer
{
    /// <summary>
    /// Is the player human?
    /// </summary>
    /// <value>Always evaluates to `true`, since this is a human player.</value>
    public bool IsHuman => true;

    /// <summary>
    /// Name of the human player.
    /// </summary>
    /// <value>The string "Human".</value>
    public string PlayerName => "Human";

    /// <summary>
    /// Humans don't have a thinker, so accessing this property will result in
    /// an exception.
    /// </summary>
    /// <value>None.</value>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown by accessing this property.
    /// </exception>
    public IThinker Thinker =>
        throw new InvalidOperationException("Humans don't need an AI thinker");
}
