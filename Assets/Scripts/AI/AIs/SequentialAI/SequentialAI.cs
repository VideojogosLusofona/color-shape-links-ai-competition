/// @file
/// @brief This file contains the ::SequentialAI class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;

public class SequentialAI : AIPlayer
{
    public override string PlayerName => "SequentialAI";
    public override IThinker Thinker => thinker;

    private IThinker thinker;

    public override void Setup()
    {
        thinker = new SequentialAIThinker();
    }

}
