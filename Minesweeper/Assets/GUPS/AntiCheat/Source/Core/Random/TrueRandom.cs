// System
using System;

namespace GUPS.AntiCheat.Core.Random
{
    /// <summary>
    /// Uses the System.Security.Cryptography.RNGCryptoServiceProvider class to generate random values.
    /// It is slower than System.Random but unpredictable because of its crypto-strength seed.
    /// </summary>
    public class TrueRandom : IRandomProvider
    {
        private static readonly System.Security.Cryptography.RandomNumberGenerator generator = new System.Security.Cryptography.RNGCryptoServiceProvider();

        /// <summary>
        /// Returns a random number between <see cref="Int32.MinValue"/> and <see cref="Int32.MaxValue"/> - 1.
        /// </summary>
        /// <returns>A random integer value.</returns>
        public int RandomInt32()
        {
            return this.RandomInt32(Int32.MinValue, Int32.MaxValue);
        }

        /// <summary>
        /// Returns a random number between <paramref name="_Min"/> (inclusive) and <paramref name="_Max"/> (exclusive).
        /// </summary>
        /// <param name="_Min">Inclusive minimum value.</param>
        /// <param name="_Max">Exclusive maximum value.</param>
        /// <returns>A random integer value within the specified range.</returns>
        public int RandomInt32(Int32 _Min, Int32 _Max)
        {
            // Used for the random bytes.
            byte[] var_Bytes = new byte[sizeof(Int32)];

            // Get the random bytes.
            generator.GetBytes(var_Bytes);

            // Convert the random bytes to an Int32 value.
            Int32 var_Value = BitConverter.ToInt32(var_Bytes, 0);

            // Make sure is between min/max.
            if(var_Value < _Min)
            {
                var_Value = _Min;
            }
            else if (var_Value > _Max - 1)
            {
                var_Value = _Max - 1;
            }

            return var_Value;
        }
    }
}
