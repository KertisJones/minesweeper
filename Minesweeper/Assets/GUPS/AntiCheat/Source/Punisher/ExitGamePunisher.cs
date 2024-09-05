// System
using System;
using System.Reflection;

// Unity
using UnityEngine;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Punisher;

namespace GUPS.AntiCheat.Punisher
{
    /// <summary>
    /// The exit game punisher is a very drastic punishment. It closes the game.
    /// </summary>
    [Serializable]
    [Obfuscation(Exclude = true)]
    public class ExitGamePunisher : MonoBehaviour, IPunisher
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the punisher.
        /// </summary>
        public String Name => "Exit Game Punisher";

        #endregion

        // Platform
        #region Platform

        /// <summary>
        /// Is supported on all platforms.
        /// </summary>
        public bool IsSupported => true;

        /// <summary>
        /// Gets or sets whether the punisher is active and can administer punitive actions (Default: true).
        /// </summary>
        [SerializeField]
        [Header("Punisher - Settings")]
        [Tooltip("Gets or sets whether the punisher is active and can administer punitive actions (Default: true).")]
        private bool isActive = true;

        /// <summary>
        /// Gets or sets whether the punisher is active and can administer punitive actions (Default: true).
        /// </summary>
        public bool IsActive { get => this.isActive; set => this.isActive = value; }

        #endregion

        // Threat Rating
        #region Threat Rating

        /// <summary>
        /// Is a very drastic punishment, so the threat rating is set to a high value (Default: 850).
        /// </summary>
        [SerializeField]
        [Tooltip("Is a very drastic punishment, so the threat rating is set to a high value (Default: 850).")]
        private uint threatRating = 850;

        /// <summary>
        /// Is a very drastic punishment, so the threat rating is set to a high value (Default: 850).
        /// </summary>
        public uint ThreatRating => this.threatRating;

        #endregion

        // Punishment
        #region Punishment

        /// <summary>
        /// Returns if the punisher should only administer punitive actions once or any time the threat level exceeds the threat rating.
        /// </summary>
        public bool PunishOnce => true;

        /// <summary>
        /// Returns if the punisher has administered punitive actions.
        /// </summary>
        public bool HasPunished { get; private set; } = false;

        /// <summary>
        /// As the name says, close the game!
        /// </summary>
        public void Punish()
        {
            // Has punished.
            this.HasPunished = true;

            // Close the game.
            Application.Quit();
        }

        #endregion
    }
}