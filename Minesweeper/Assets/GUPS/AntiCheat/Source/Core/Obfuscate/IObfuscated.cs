namespace GUPS.AntiCheat.Core.Obfuscate
{
    /// <summary>
    /// Represents an interface for classes responsible for implementing obfuscation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IObfuscated"/> interface defines the contract for classes that are tasked with applying obfuscation to their own content.
    /// </para>
    /// </remarks>
    public interface IObfuscated
    {
        /// <summary>
        /// Applies obfuscation to the implementing class.
        /// </summary>
        void Obfuscate();
    }
}
