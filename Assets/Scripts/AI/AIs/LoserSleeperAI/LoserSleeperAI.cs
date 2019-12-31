/// @file
/// @brief This file contains the ::LoserSleeperAI class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

/// <summary>
/// Configuration class for the <see cref="LoserSleeperAIThinker"/>.
/// </summary>
public class LoserSleeperAI : AIPlayer
{
    public override string PlayerName => "LoserSleeperAI";
    public override IThinker Thinker => thinker;

    private IThinker thinker;
    public override void Setup()
    {
        thinker = new LoserSleeperAIThinker();
    }

}
