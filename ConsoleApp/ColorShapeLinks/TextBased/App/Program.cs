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
using ColorShapeLinks.Common.Session;
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

        // Known event listeners
        private static IDictionary<string, Type> knownThinkerListeners;
        private static IDictionary<string, Type> knownMatchListeners;
        private static IDictionary<string, Type> knownSessionListeners;

        // Selected event listeners
        private static IList<IThinkerListener> selectedThinkerListeners;
        private static IList<IMatchListener> selectedMatchListeners;
        private static IList<ISessionListener> selectedSessionListeners;

        // Base options
        private static bool debug;

        // Inner struct for one match sessions
        private struct NoSessionConfig : ISessionConfig
        {
            public int PointsPerWin => 0;
            public int PointsPerLoss => 0;
            public int PointsPerDraw => 0;
        }

        // The main method, where the ColorShapeLinks app starts
        // The Options class defines the command line arguments accepted in
        // the args parameter
        private static int Main(string[] args)
        {

            // Assume there will be an error or exception
            exitStatus = ExitStatus.Exception;

            // Console app may throw a number of exceptions, we'll catch them
            // here
            try
            {
                // Parse command line parameters and act accordingly
                exitStatus = Parser.Default
                    .ParseArguments<SessionOptions, MatchOptions, BaseOptions>(
                        args)
                    .MapResult(
                        (SessionOptions o) => RunSession(o, true),
                        (MatchOptions o) => RunSession(o, false),
                        (BaseOptions o) => ShowInfo(o),
                        errs => ExitStatus.Exception);
            }
            catch (Exception e)
            {   // An exception was thrown, deal with it

                // Should we print the full stack trace?
                if (debug || args.Contains("-d") || args.Contains("--debug"))
                {
                    // Yes

                    // Local function to recursively print inner exception
                    // stack traces
                    void RecurseInners(Exception ie)
                    {
                        if (ie.InnerException != null)
                            RecurseInners(ie.InnerException);
                        Console.Error.WriteLine(String.Format(
                            "\n{0} ({1})\n{2}",
                            ie.GetType().Name, ie.Message, ie.StackTrace));
                    }

                    // Recursively print exceptions, starting with the
                    // innermost one
                    RecurseInners(e);
                }
                else
                {
                    // No full stack trace, just the basic error messages

                    // Used for possible inner exceptions
                    Exception ie = e;

                    // Show the exception type and message
                    Console.Error.WriteLine(
                        $"\nERROR ({e.GetType().Name}): {e.Message}");

                    // If there are inner exceptions that caused this exception,
                    // show them also
                    while ((ie = ie.InnerException) != null)
                    {
                        Console.WriteLine(
                            $"\tcaused by a {ie.GetType().Name}: {ie.Message}");
                    }

                    // No, but inform user on how to see the full stack trace
                    Console.Error.WriteLine(
                        "\nUse the `--debug` option to show the complete "
                        + "stack trace.\n");
                }
            }

            // Terminate console app with the specified exit status
            return (int)exitStatus;
        }

        // Run session
        private static ExitStatus RunSession(
            GameOptions options, bool complete)
        {
            // Load third-party assemblies
            LoadAssembliesAndSelectListeners(options);

            // Initialize a session controller
            SessionController sc = new SessionController(
                options,
                options is ISessionConfig
                    ? options as ISessionConfig
                    : new NoSessionConfig(),
                options.ThinkerPrototypes,
                selectedThinkerListeners,
                selectedMatchListeners,
                selectedSessionListeners);

            // Run session
            return sc.Run(complete);
        }

        // Show environment info (loaded assemblies, known thinkers and known
        // listeners)
        private static ExitStatus ShowInfo(BaseOptions options)
        {
            // Load third-party assemblies
            LoadAssembliesAndSelectListeners(options);

            // Show loaded assemblies
            Console.WriteLine("Loaded assemblies:");
            foreach (System.Reflection.Assembly a in
                AppDomain.CurrentDomain.GetAssemblies())
            {
                Console.WriteLine($"\t{a}");
            }

            // Show known thinkers
            Console.WriteLine("Known thinkers:");
            foreach (string thinkerName in ThinkerManager.Instance.ThinkerNames)
            {
                Console.WriteLine($"\t{thinkerName}");
            }

            // Show known thinker listeners
            Console.WriteLine("Known thinker listeners:");
            foreach (string listenerName in knownThinkerListeners.Keys)
            {
                Console.WriteLine($"\t{listenerName}");
            }

            // Show known match listeners
            Console.WriteLine("Known match listeners:");
            foreach (string listenerName in knownMatchListeners.Keys)
            {
                Console.WriteLine($"\t{listenerName}");
            }

            // Show known session listeners
            Console.WriteLine("Known session listeners:");
            foreach (string listenerName in knownSessionListeners.Keys)
            {
                Console.WriteLine($"\t{listenerName}");
            }

            return ExitStatus.Info;
        }


        // Load assemblies and select listeners
        private static void LoadAssembliesAndSelectListeners(
            BaseOptions options)
        {
            // Get debug mode from options
            debug = options.DebugMode;

            // Load assemblies
            foreach (string a in options.Assemblies)
            {
                if (a != null && a.Length > 0)
                    System.Reflection.Assembly.LoadFile(a);
            }

            // Find listeners
            knownThinkerListeners = FindListeners<IThinkerListener>();
            knownMatchListeners = FindListeners<IMatchListener>();
            knownSessionListeners = FindListeners<ISessionListener>();

            // Filter listeners
            selectedThinkerListeners = new List<IThinkerListener>();
            selectedMatchListeners = new List<IMatchListener>();
            selectedSessionListeners = new List<ISessionListener>();

            // Will there be matches?
            if (options is GameOptions)
            {
                // If so, filter thinker and match listeners so they can
                // listen to thinkers and matches
                GameOptions gameOptions = options as GameOptions;

                FilterListeners<IThinkerListener>(
                    gameOptions.ThinkerListeners,
                    knownThinkerListeners,
                    selectedThinkerListeners);

                FilterListeners<IMatchListener>(
                    gameOptions.MatchListeners,
                    knownMatchListeners,
                    selectedMatchListeners);
            }

            // Will there be a session of matches?
            if (options is SessionOptions)
            {
                // If so, filter session listeners so they can listen to the
                // session
                SessionOptions sessionOptions = options as SessionOptions;

                FilterListeners<ISessionListener>(
                    sessionOptions.SessionListeners,
                    knownSessionListeners,
                    selectedSessionListeners);
            }

        }

        // Helper generic method to find known listeners and place them in the
        // specified variable
        private static IDictionary<string, Type> FindListeners<T>()
        {
            // Get specific listeners type
            Type type = typeof(T);

            // Get known listeners of this type
            return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => type.IsAssignableFrom(t)
                        && !t.IsAbstract
                        && t.GetConstructor(Type.EmptyTypes) != null)
                    .ToDictionary(t => t.FullName, t => t);
        }

        // Helper generic method to filter listeners
        private static void FilterListeners<T>(
            IEnumerable<string> specified,
            IDictionary<string, Type> known,
            IList<T> selected)
        {
            // For each of the listeners specified...
            foreach (string l in specified)
            {
                // Check if it's an empty string, in which case, ignore it
                if (l.Trim().Length == 0) continue;

                // Check if it's known (i.e. if it exists in the
                // loaded assemblies)
                if (known.ContainsKey(l))
                {
                    // If so, instantiate it and add it to the listeners list
                    Type t = known[l];
                    selected.Add((T)Activator.CreateInstance(t));
                }
                else
                {
                    // Otherwise, throw an exception
                    throw new ArgumentException(
                        $"Unknown {typeof(T).Name} listener '{l}'");
                }
            }
        }
    }
}
