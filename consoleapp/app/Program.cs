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
