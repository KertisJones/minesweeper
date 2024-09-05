namespace GUPS.AntiCheat.Core.Integrity
{
    /// <summary>
    /// Represents an interface for classes responsible for ensuring and verifying data integrity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IDataIntegrity"/> interface defines the contract for classes that are responsible for maintaining and validating the integrity of their contained data.
    /// </para>
    /// </remarks>
    public interface IDataIntegrity
    {
        /// <summary>
        /// Gets a value indicating whether the implementing class has data integrity.
        /// </summary>
        bool HasIntegrity { get; }

        /// <summary>
        /// Checks the integrity of the data within the implementing class.
        /// </summary>
        /// <returns>
        /// True if the data integrity check is successful; otherwise, false.
        /// </returns>
        bool CheckIntegrity();
    }
}