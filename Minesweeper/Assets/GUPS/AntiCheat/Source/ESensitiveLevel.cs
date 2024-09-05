namespace GUPS.AntiCheat
{
    /// <summary>
    /// The sensitivity level of the monitor. Manages the reaction sensitivity of the monitor to possible detected threats.
    /// </summary>
    public enum ESensitiveLevel : byte
    {
        /// <summary>
        /// Do not react to any possible cheating.
        /// </summary>
        NOT_SENSITIVE = 0,

        /// <summary>
        /// React on a high threat rating.
        /// </summary>
        LESS_SENSITIVE = 1,

        /// <summary>
        /// React on a moderate threat rating.
        /// </summary>
        MODERATE = 2,

        /// <summary>
        /// React on a low threat rating. May cause false positives.
        /// </summary>
        SENSITIVE = 3,

        /// <summary>
        /// React on a any threat rating. May cause false positives.
        /// </summary>
        VERY_SENSITIVE = 4,
    }
}