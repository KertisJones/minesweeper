// System
using System;

namespace GUPS.AntiCheat.Core.Hash
{
    /// <summary>
    /// A list of hash algorithms that can be used to hash data.
    /// </summary>
    [Serializable]
    public enum EHashAlgorithm
    {
        /// <summary>
        /// No hash algorithm.
        /// </summary>
        NONE = 0,

        /// <summary>
        /// MD5 hash algorithm. Not recommended for security purposes.
        /// </summary>
        MD5 = 1,

        /// <summary>
        /// SHA1 hash algorithm. Not recommended for security purposes.
        /// </summary>
        SHA1 = 2,

        /// <summary>
        /// SHA256 hash algorithm.
        /// </summary>
        SHA256 = 3,

        /// <summary>
        /// SHA384 hash algorithm.
        /// </summary>
        SHA384 = 4,

        /// <summary>
        /// SHA512 hash algorithm.
        /// </summary>
        SHA512 = 5
    }
}
