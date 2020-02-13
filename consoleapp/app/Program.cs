/// @file
/// @brief This file contains the ::Program class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Linq;
using System.Collections.Generic;
using ColorShapeLinks.ConsoleAppLib;
using ColorShapeLinks.Common.AI;
using CommandLine;

namespace ColorShapeLinks.ConsoleApp
{
    class Program
    {
        private static ExitStatus exitStatus;
        private static IDictionary<string, Type> knownListeners;

        private static int Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    foreach (string a in o.Assemblies)
                    {
                        System.Reflection.Assembly.LoadFile(a);
                    }

                    FindListeners();

                    if (o.ShowDebugInfoAndExit)
                    {
                        ShowDebugInfo();
                        exitStatus = ExitStatus.DebugInfo;
                    }
                    else
                    {
                        Match game = new Match(o);
                        RegisterListeners(game, o.Listeners);
                        exitStatus = game
                            .Run()
                            .ToExitStatus();
                    }
                });
            return (int)exitStatus;
        }

        private static void ShowDebugInfo()
        {
            // Show loaded assemblies
            Console.WriteLine("Loaded assemblies:");
            foreach (System.Reflection.Assembly a in
                AppDomain.CurrentDomain.GetAssemblies())
            {
                Console.WriteLine($"\t{a}");
            }

            // Show known thinkers
            Console.WriteLine("Known thinkers:");
            foreach (string thinkerName in AIManager.Instance.AIs)
            {
                Console.WriteLine($"\t{thinkerName}");
            }

            // Show known listeners
            Console.WriteLine("Known listeners:");
            foreach (string listenerName in knownListeners.Keys)
            {
                Console.WriteLine($"\t{listenerName}");
            }
        }

        private static void FindListeners()
        {
            Type type = typeof(IMatchListener);
            knownListeners =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => type.IsAssignableFrom(t)
                        && !t.IsAbstract
                        && t.GetConstructor(Type.EmptyTypes) != null)
                    .ToDictionary(t => t.FullName, t => t);
        }

        private static void RegisterListeners(
            IMatchSubject match, IEnumerable<string> listeners)
        {
            foreach (string listenerName in listeners)
            {
                if (knownListeners.ContainsKey(listenerName))
                {
                    IMatchListener listener =
                        (IMatchListener)Activator.CreateInstance(
                            knownListeners[listenerName]);
                    listener.ListenTo(match);
                }
                else
                {
                    throw new ArgumentException(
                        $"Unknown listener '{listenerName}'");
                }
            }
        }

    }
}
