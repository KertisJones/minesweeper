// System
using System;

namespace GUPS.AntiCheat.Core.Random
{
    /// <summary>
    /// Uses the System.Random class to generate random values.
    /// Is very fast but predictable under high effort.
    /// </summary>
    public class PseudoRandom : IRandomProvider
    {
        private static readonly System.Random generator = new System.Random();

        /// <summary>
        /// Returns a random number between <see cref="Int32.MinValue"/> and <see cref="Int32.MaxValue"/> - 1.
        /// </summary>
        /// <returns>A random integer value.</returns>
        public int RandomInt32()
        {
            // Min is inclusive, Max is exclusive.
            return generator.Next(Int32.MinValue, Int32.MaxValue);
        }

        /// <summary>
        /// Returns a random number between <paramref name="_Min"/> (inclusive) and <paramref name="_Max"/> (exclusive).
        /// </summary>
        /// <param name="_Min">Inclusive minimum value.</param>
        /// <param name="_Max">Exclusive maximum value.</param>
        /// <returns>A random integer value within the specified range.</returns>
        public int RandomInt32(Int32 _Min, Int32 _Max)
        {
            // Min is inclusive, Max is exclusive.
            return generator.Next(_Min, _Max);
        }
    }
}
