/// @file
/// @brief This file contains the ::RandomAI class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

/// <summary>
/// Configuration class for the <see cref="RandomAIThinker"/>.
/// </summary>
public class RandomAI : AIPlayer
{
    /// <summary>The player's name.</summary>
    /// <value>The string "RandomAI".</value>
    /// <seealso cref="AIPlayer.PlayerName"/>
    public override string PlayerName => "RandomAI";

    /// <summary>The player's thinker.</summary>
    /// <value>An instance of <see cref="RandomAIThinker"/>.</value>
    /// <seealso cref="AIPlayer.Thinker"/>
    public override IThinker Thinker => thinker;

    // Suport variable for RandomAI's thinker instance
    private IThinker thinker;

    /// <summary>
    /// This method will be called before a match starts and is used for
    /// instantiating a new <see cref="RandomAIThinker"/>.
    /// </summary>
    /// <seealso cref="AIPlayer.Setup"/>
    public override void Setup()
    {
        thinker = new RandomAIThinker();
    }
}
