/// @file
/// @brief This file contains the ::ColorShapeLinks.TextBased.App.Program
/// class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Linq;
using System.Collections.Generic;
using ColorShapeLinks.TextBased.Lib;
using ColorShapeLinks.Common.AI;
using CommandLine;

namespace ColorShapeLinks.TextBased.App
{
    /// <summary>
    /// This class contains the `Main()` method for the ColorShapeLinks
    /// console app.
    /// </summary>
    public class Program
    {
        // Exit status to return to OS
        private static ExitStatus exitStatus;
        // Known match event listeners
        private static IDictionary<string, Type> knownListeners;

        // The main method, where the ColorShapeLinks app starts
        // The Options class defines the command line arguments accepted in
        // the args parameter
        private static int Main(string[] args)
        {
            // Reference to the parsed options
            Options options = null;

            // Assume there will be an error or exception
            exitStatus = ExitStatus.Exception;

            // Console app may throw a number of exceptions, we'll catch them
            // here
            try
            {
                // Parse command line parameters and act accordingly
                Parser.Default
                    .ParseArguments<Options>(args)
                    .WithParsed<Options>(o =>
                    {
                        // Make sure we have a reference to the parsed options
                        // after options parsing
                        options = o;

                        // Load assemblies
                        foreach (string a in o.Assemblies)
                        {
                            System.Reflection.Assembly.LoadFile(a);
                        }

                        // Find match event listeners and place them in the
                        // knownListeners class variable
                        FindListeners();

                        // Show info and exit?
                        if (o.ShowInfoAndExit)
                        {   // Yes, show info and exit
                            ShowInfo();
                            exitStatus = ExitStatus.Info;
                        }
                        else
                        {   // Play a match of ColorShapeLinks

                            // Create a new match with the parsed options
                            MatchController match = new MatchController(o);

                            // Register match listeners
                            RegisterListeners(match, o.Listeners);

                            // Run the match, converting the returned winner
                            // to an exit status
                            exitStatus = match
                                .Run()
                                .ToExitStatus();
                        }
                    });
            }
            catch (Exception e)
            {   // An exception was thrown, deal with it

                // Show the exception type and message
                Console.Error.WriteLine(
                    $"\nERROR ({e.GetType().Name}): {e.Message}");

                // Should we print the full stack trace?
                if (options?.DebugMode ?? true)
                {
                    // Yes
                    Console.Error.WriteLine($"{e.StackTrace}\n");
                }
                else
                {
                    // No, but inform user on how to see the full stack trace
                    Console.Error.WriteLine(
                        "\nUse the `--debug` option to show the complete "
                        + "stack trace.\n");
                }
            }

            // Terminate console app with the specified exit status
            return (int)exitStatus;
        }

        // Show environment info (loaded assemblies, known thinkers and known
        // listeners)
        private static void ShowInfo()
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

        // Find known match event listeners and populate the knownListeners
        // class variable
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

        // Register specified match listeners with the match
        private static void RegisterListeners(
            IMatchSubject match, IEnumerable<string> listeners)
        {
            // Loop through the specified match listeners
            foreach (string listenerName in listeners)
            {
                // Is the listener known?
                if (knownListeners.ContainsKey(listenerName))
                {
                    // If so, create an instance...
                    IMatchListener listener =
                        (IMatchListener)Activator.CreateInstance(
                            knownListeners[listenerName]);
                    // ...and register it with the match
                    listener.ListenTo(match);
                }
                else
                {
                    // Otherwise, throw an exception
                    throw new ArgumentException(
                        $"Unknown listener '{listenerName}'");
                }
            }
        }
    }
}
