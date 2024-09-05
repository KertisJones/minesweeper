// System
using System;

namespace GUPS.AntiCheat.Core.Random
{
    /// <summary>
    /// Represents a provider for generating random values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IRandomProvider"/> interface defines the contract for a provider that offers methods to generate random integer values.
    /// </para>
    /// </remarks>
    public interface IRandomProvider
    {
        /// <summary>
        /// Returns a random number between <see cref="Int32.MinValue"/> and <see cref="Int32.MaxValue"/> - 1.
        /// </summary>
        /// <returns>A random integer value.</returns>
        Int32 RandomInt32();

        /// <summary>
        /// Returns a random number between <paramref name="_Min"/> (inclusive) and <paramref name="_Max"/> (exclusive).
        /// </summary>
        /// <param name="_Min">Inclusive minimum value.</param>
        /// <param name="_Max">Exclusive maximum value.</param>
        /// <returns>A random integer value within the specified range.</returns>
        Int32 RandomInt32(Int32 _Min, Int32 _Max);
    }

}
