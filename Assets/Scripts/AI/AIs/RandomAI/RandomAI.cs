/// @file
/// @brief This file contains the ::RandomAI class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

public class RandomAI : AIPlayer
{
    public override string PlayerName => "RandomAI";
    public override IThinker Thinker => thinker;

    private IThinker thinker;
    public override void Setup()
    {
        thinker = new RandomAIThinker();
    }
}
