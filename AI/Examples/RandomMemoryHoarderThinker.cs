/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.Common.AI.Examples.BadMoveAIThinker class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;

namespace ColorShapeLinks.Common.AI.Examples
{
    /// <summary>
    /// This thinker plays randomly and hoards memory for testing purposes.
    /// </summary>
    public class RandomMemoryHoarderThinker : AbstractThinker
    {
        // Reference to the random thinker
        private AbstractThinker randomThinker;

        // Auxiliary random number generator
        private Random rnd;

        /// <summary>
        /// Array where to place stuff that will occupy memory. Leave it
        /// public so compiler doesn't optimize it out.
        /// </summary>
        public ulong[] memory;

        /// <summary>
        /// Setup the random memory hoarder.
        /// </summary>
        /// <param name="str">
        /// A string representation of the memory size in bytes.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when an invalid parameter is given in <paramref name="str"/>.
        /// </exception>
        /// <seealso cref="AbstractThinker.Setup"/>
        public override void Setup(string str)
        {
            int arraySize;
            ulong memSize;
            rnd = new Random();

            // Determine size of memory in bytes to hoard
            if (!ulong.TryParse(str, out memSize))
            {
                throw new ArgumentException(String.Format(
                    "Invalid parameter '{0}' for setting up {1}",
                    str, nameof(RandomMemoryHoarderThinker)));
            }

            // Proceed with memory occupation
            arraySize = (int)(memSize / sizeof(ulong));
            memory = new ulong[arraySize];
            for (int i = 0; i < arraySize; i++) memory[i] = (ulong)rnd.Next();

            // Create an auxiliary random player to perform the actual moves
            randomThinker = new RandomAIThinker();
            randomThinker.Setup("123");
        }

        /// @copydoc IThinker.Think
        /// <seealso cref="IThinker.Think"/>
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // Do some stuff with memory so that compiler doesn't optimize it
            // out
            int rndIdx = rnd.Next(1, memory.Length - 1);
            memory[rndIdx - 1] =
                memory[rndIdx] ^ memory[rndIdx + 1] << rnd.Next(1, 4);

            // Return the bad move
            return randomThinker.Think(board, ct);
        }
    }
}
