/// @file
/// @brief This file contains the ::AbstractThinker class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Threading;

namespace ColorShapeLinks.Common.AI
{
    /// <summary>
    /// The actual AI code of specific AIs should be placed in classes which
    /// extend this class.
    /// </summary>
    public abstract class AbstractThinker : IThinker
    {
        private int rows;
        private int cols;
        private int winSequence;
        private int roundPiecesPerPlayer;
        private int squarePiecesPerPlayer;
        private int timeLimitMillis;

        protected int Rows =>  rows;
        protected int Cols =>  cols;
        protected int WinSequence => winSequence;
        protected int RoundsPerPlayer => roundPiecesPerPlayer;
        protected int SquaresPerPlayer => squarePiecesPerPlayer;
        protected int TimeLimitMillis => timeLimitMillis;

        /// <summary>
        /// Setup thinker.
        /// </summary>
        /// <param name="str">
        /// String containing setup parameters, should be parsed by the
        /// concrete IA thinker.
        /// </param>
        /// <remarks>
        /// By default, this method does nothing and its implementation by
        /// the concrete thinkers is entirely optional.
        /// </remarks>
        public virtual void Setup(string str) { }

        /// @copydoc IThinker.Think
        /// <seealso cref="IThinker.Think"/>
        public abstract FutureMove Think(Board board, CancellationToken ct);

        public override string ToString()
        {
            string name = GetType().Name;

            if (name.Length > "aithinker".Length
                && (name.ToLower().EndsWith("aithinker")
                    || name.ToLower().EndsWith("thinkerai")))
            {
                return name.Substring(0, name.Length - "aithinker".Length);
            }

            if (name.Length > "thinker".Length
                && name.ToLower().EndsWith("thinker"))
            {
                return name.Substring(0, name.Length - "thinker".Length);
            }

            return name;
        }
    }
}
