// System
using System;

// Unity
using UnityEngine.Events;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Detector;

namespace GUPS.AntiCheat.Detector
{
    /// <summary>
    /// A unity event that is used to subscribe to the cheating detection events. It is useful if you do not want to write custom 
    /// observers to subscribe to the detectors and simply attach a callback to the detector event through the inspector.
    /// </summary>
    [Serializable]
    public class CheatingDetectionEvent<T> : UnityEvent<T>
        where T : IDetectorStatus
    {
    }
}
