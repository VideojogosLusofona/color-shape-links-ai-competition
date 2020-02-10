/// @file
/// @brief This file contains the ::Program class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using ColorShapeLinks.Common.AI;
using CommandLine;

namespace ColorShapeLinks.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    if (o.ListPlayers)
                    {
                        foreach (string thinkerName in AIManager.Instance.AIs)
                        {
                            Console.WriteLine(thinkerName);
                        }
                    }
                    else
                    {
                        Game game = new Game(o);
                        game.Run();
                    }

                });
        }
    }
}
