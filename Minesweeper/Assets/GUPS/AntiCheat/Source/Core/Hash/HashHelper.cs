// System
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace GUPS.AntiCheat.Core.Hash
{
    /// <summary>
    /// Provides utility methods for hashing operations, including obtaining hash algorithms, computing hash values, and converting 
    /// hash values to hexadecimal strings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="HashHelper"/> class offers a collection of static methods to facilitate various hashing tasks. It provides 
    /// functionalities such as obtaining hash algorithms based on specified algorithms or names, computing hash values from byte arrays, 
    /// and converting hash values to hexadecimal strings.
    /// </para>
    /// <para>
    /// The class is designed to be used statically and does not require instantiation. It supports multiple hash algorithms, including 
    /// MD5, SHA1, SHA256, SHA384, and SHA512. Users can choose to compute hash values using these algorithms and convert the results 
    /// to hexadecimal strings with optional uppercasing and separators.
    /// </para>
    /// </remarks>
    public static class HashHelper
    {
        /// <summary>
        /// Retrieves the name corresponding to the specified hash algorithm enumeration.
        /// </summary>
        /// <param name="_HashAlgorithm">The enumeration value representing the hash algorithm.</param>
        /// <returns>The name of the hash algorithm.</returns>
        public static String GetName(EHashAlgorithm _HashAlgorithm)
        {
            switch (_HashAlgorithm)
            {
                case EHashAlgorithm.MD5:
                    return "MD5";
                case EHashAlgorithm.SHA1:
                    return "SHA-1";
                case EHashAlgorithm.SHA256:
                    return "SHA-256";
                case EHashAlgorithm.SHA384:
                    return "SHA-384";
                case EHashAlgorithm.SHA512:
                    return "SHA-512";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Retrieves the hash algorithm corresponding to the specified <paramref name="_HashAlgorithm"/>.
        /// </summary>
        /// <param name="_HashAlgorithm">The enumeration value representing the hash algorithm.</param>
        /// <returns>The <see cref="HashAlgorithm"/> instance corresponding to the specified algorithm.</returns>
        public static HashAlgorithm GetHashAlgorithm(EHashAlgorithm _HashAlgorithm)
        {
            switch (_HashAlgorithm)
            {
                case EHashAlgorithm.MD5:
                    return MD5.Create();
                case EHashAlgorithm.SHA1:
                    return SHA1.Create();
                case EHashAlgorithm.SHA256:
                    return SHA256.Create();
                case EHashAlgorithm.SHA384:
                    return SHA384.Create();
                case EHashAlgorithm.SHA512:
                    return SHA512.Create();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Retrieves the hash algorithm corresponding to the specified <paramref name="_HashAlgorithm"/>.
        /// </summary>
        /// <param name="_HashAlgorithm">The name of the hash algorithm.</param>
        /// <returns>The <see cref="HashAlgorithm"/> instance corresponding to the specified algorithm name.</returns>
        public static HashAlgorithm GetHashAlgorithm(String _HashAlgorithm)
        {
            return HashAlgorithm.Create(_HashAlgorithm);
        }

        /// <summary>
        /// Computes the hash value of the specified byte array using the specified <paramref name="_HashAlgorithm"/>.
        /// </summary>
        /// <param name="_HashAlgorithm">The enumeration value representing the hash algorithm.</param>
        /// <param name="_Buffer">The input byte array to compute the hash value.</param>
        /// <returns>The computed hash value as a byte array.</returns>
        public static byte[] ComputeHash(EHashAlgorithm _HashAlgorithm, byte[] _Buffer)
        {
            using (HashAlgorithm var_HashAlgorithm = GetHashAlgorithm(_HashAlgorithm))
            {
                return var_HashAlgorithm.ComputeHash(_Buffer);
            }
        }

        /// <summary>
        /// Computes the hash value of the specified byte array using the specified <paramref name="_HashAlgorithm"/>.
        /// </summary>
        /// <param name="_HashAlgorithm">The name of the hash algorithm.</param>
        /// <param name="_Buffer">The input byte array to compute the hash value.</param>
        /// <returns>The computed hash value as a byte array.</returns>
        public static byte[] ComputeHash(String _HashAlgorithm, byte[] _Buffer)
        {
            using (HashAlgorithm var_HashAlgorithm = GetHashAlgorithm(_HashAlgorithm))
            {
                return var_HashAlgorithm.ComputeHash(_Buffer);
            }
        }

        /// <summary>
        /// Computes the hash value of the specified stream using the specified <paramref name="_HashAlgorithm"/>.
        /// </summary>
        /// <param name="_HashAlgorithm">The enumeration value representing the hash algorithm.</param>
        /// <param name="_Stream">The input stream to compute the hash value.</param>
        /// <returns>The computed hash value as a byte array.</returns>
        public static byte[] ComputeHash(EHashAlgorithm _HashAlgorithm, Stream _Stream)
        {
            using (HashAlgorithm var_HashAlgorithm = GetHashAlgorithm(_HashAlgorithm))
            {
                return var_HashAlgorithm.ComputeHash(_Stream);
            }
        }

        /// <summary>
        /// Computes the hash value of the specified stream using the specified <paramref name="_HashAlgorithm"/>.
        /// </summary>
        /// <param name="_HashAlgorithm">The name of the hash algorithm.</param>
        /// <param name="_Stream">The input stream to compute the hash value.</param>
        /// <returns>The computed hash value as a byte array.</returns>
        public static byte[] ComputeHash(String _HashAlgorithm, Stream _Stream)
        {
            using (HashAlgorithm var_HashAlgorithm = GetHashAlgorithm(_HashAlgorithm))
            {
                return var_HashAlgorithm.ComputeHash(_Stream);
            }
        }

        /// <summary>
        /// Converts the specified byte array to a hexadecimal string representation.
        /// </summary>
        /// <param name="_Buffer">The byte array to convert to a hexadecimal string.</param>
        /// <param name="_UpperCase">Specifies whether the hexadecimal string should be in uppercase.</param>
        /// <param name="_Separator">Specifies whether to include separators between hexadecimal pairs.</param>
        /// <returns>The hexadecimal string representation of the byte array.</returns>
        public static string ToHex(byte[] _Buffer, bool _UpperCase, bool _Separator)
        {
            StringBuilder var_StringBuilder = new StringBuilder(_Buffer.Length * 2);

            for (int i = 0; i < _Buffer.Length; i++)
            {
                var_StringBuilder.Append(_Buffer[i].ToString(_UpperCase ? "X2" : "x2"));
            }

            String var_Result = var_StringBuilder.ToString();

            if (_Separator)
            {
                var_Result = String.Join(":", Regex.Matches(var_Result, ".{2}").Cast<Match>());
            }

            return var_Result;
        }
    }
}
