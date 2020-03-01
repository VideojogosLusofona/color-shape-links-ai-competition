/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.AI.AbstractThinker
/// class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;

namespace ColorShapeLinks.Common.AI
{
    /// <summary>
    /// The actual AI code of specific AIs should be placed in classes which
    /// extend this class.
    /// </summary>
    public abstract class AbstractThinker : IThinker
    {
        // Number of board rows
        private int rows = 0;

        // Number of board columns
        private int cols = 0;

        // How many pieces in sequence to find a winner
        private int winSequence = 0;

        // Number of initial round pieces per player
        private int roundPiecesPerPlayer = 0;

        // Number of initial square round pieces per player
        private int squarePiecesPerPlayer = 0;

        // Time limit for the AI to play
        private int timeLimitMillis = 0;

        /// <summary>Number of board rows.</summary>
        /// <value>Number of board rows.</value>
        protected int Rows =>  rows;

        /// <summary>Number of board columns.</summary>
        /// <value>Number of board columns.</value>
        protected int Cols =>  cols;

        /// <summary>How many pieces in sequence to find a winner.</summary>
        /// <value>How many pieces in sequence to find a winner.</value>
        protected int WinSequence => winSequence;

        /// <summary>Number of initial round pieces per player.</summary>
        /// <value>Number of initial round pieces per player.</value>
        protected int RoundsPerPlayer => roundPiecesPerPlayer;

        /// <summary>Number of initial square round pieces per player</summary>
        /// <value>Number of initial square round pieces per player</value>
        protected int SquaresPerPlayer => squarePiecesPerPlayer;

        /// <summary>Time limit for the AI to play.</summary>
        /// <value>Time limit for the AI to play.</value>
        protected int TimeLimitMillis => timeLimitMillis;

        /// <summary>
        /// Setup thinker.
        /// </summary>
        /// <param name="str">
        /// String containing setup parameters, should be parsed by the
        /// concrete AI thinker.
        /// </param>
        /// <remarks>
        /// By default, this method does nothing and its implementation by
        /// the concrete thinkers is entirely optional.
        /// </remarks>
        public virtual void Setup(string str) { }

        /// @copydoc IThinker.Think
        /// <seealso cref="IThinker.Think"/>
        public abstract FutureMove Think(Board board, CancellationToken ct);

        /// <summary>
        /// Returns a short string description of the AI.
        /// </summary>
        /// <remarks>
        /// By default, the namespace is removed, as well as the "thinker" or
        /// "aithinker" or "thinkerai" suffixes.
        /// </remarks>
        /// <returns>A short string description of the AI.</returns>
        public override string ToString()
        {
            // Get base name, without namespace
            string name = GetType().Name;

            // Remove "aithinker" or "thinkerai" suffixes, if they exist
            if (name.Length > "aithinker".Length
                && (name.ToLower().EndsWith("aithinker")
                    || name.ToLower().EndsWith("thinkerai")))
            {
                return name.Substring(0, name.Length - "aithinker".Length);
            }

            // Remove "thinker" suffix if it exists
            if (name.Length > "thinker".Length
                && name.ToLower().EndsWith("thinker"))
            {
                return name.Substring(0, name.Length - "thinker".Length);
            }

            // Return unchanged name
            return name;
        }

        /// <summary>
        /// Thinker should raise the thinking info event using this method.
        /// </summary>
        /// <param name="info">
        /// String containing the thinking information.
        /// </param>
        protected void OnThinkingInfo(string info)
        {
            ThinkingInfo?.Invoke(info);
        }

        /// @copydoc IThinker.ThinkingInfo
        /// <seealso cref="IThinker.ThinkingInfo"/>
        public event Action<string> ThinkingInfo;
    }
}
