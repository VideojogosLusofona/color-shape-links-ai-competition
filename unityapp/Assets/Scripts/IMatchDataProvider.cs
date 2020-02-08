/// @file
/// @brief This file contains the ::IMatchDataProvider interface.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

/// <summary>
/// Defines a data provider for *ColorShapeLinks* matches.
/// </summary>
public interface IMatchDataProvider
{
    /// <summary>The game board.</summary>
    /// <value>The game board.</value>
    Board Board { get; }

    /// <summary>The current player.</summary>
    /// <value>The current player.</value>
    IPlayer CurrentPlayer { get; }

    /// <summary>
    /// Maximum real time in seconds that AI can take to play.
    /// </summary>
    /// <value>Maximum real time in seconds that AI can take to play.</value>
    float AITimeLimit { get; }

    /// <summary>
    /// Even if the AI plays immediately, this time (in seconds) gives the
    /// illusion that the AI took some minimum time to play.
    /// </summary>
    /// <value>Minimum apparent AI play time.</value>
    float MinAIGameMoveTime { get; }

    /// <summary>Last move animation length in seconds.</summary>
    /// <value>Last move animation length in seconds.</value>
    float LastMoveAnimLength { get; }

    /// <summary>Get player of the specified color.</summary>
    /// <param name="player">Color of the player to get.</param>
    /// <returns>Player of the specified color.</returns>
    IPlayer GetPlayer(PColor player);
}
