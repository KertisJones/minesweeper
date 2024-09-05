// Microsoft
using System;

// Unity
using UnityEngine;
using UnityEditor;

// GUPS
using GUPS.AntiCheat.Core.Monitor;
using GUPS.AntiCheat.Core.Detector;
using GUPS.AntiCheat.Core.Punisher;

namespace GUPS.AntiCheat.Editor
{
    /// <summary>
    /// Custom editor for inspecting and modifying the AntiCheatMonitor component in the Unity editor.
    /// </summary>
    [CustomEditor(typeof(AntiCheatMonitor), editorForChildClasses: true)]
    public class AntiCheatMonitorEditor : UnityEditor.Editor
    {
        // The serialized properties of the AntiCheatMonitor component.
        private SerializedProperty sensitiveLevelProp;

        /// <summary>
        /// Called when the editor is enabled, initializing serialized properties.
        /// </summary>
        protected virtual void OnEnable()
        {
            // Initialize serialized properties.
            this.sensitiveLevelProp = this.serializedObject.FindProperty("sensitiveLevel");
        }

        /// <summary>
        /// Override of the default inspector GUI to provide a custom interface for the AntiCheatMonitor component.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Update the serialized object.
            this.serializedObject.Update();

            // Display an info message.
            EditorGUILayout.HelpBox("The AntiCheat Monitor component is a monitor for detecting and punishing cheaters. It is the core component of the AntiCheat system.", MessageType.Info);
            
            // Display the header.
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("AntiCheat Monitor - Settings", EditorStyles.boldLabel);

            // Display and edit the sensitive level property.
            EditorGUILayout.PropertyField(this.sensitiveLevelProp, new GUIContent("Sensitive Level", "The sensitive level of the monitor. Manage the reaction sensitivity of the monitor to possible detected threats. Ratings of detected threats are scaled by the sensitive level beginning at 0 (NOT_SENSITIVE) up to 4 (VERY_SENSITIVE). The higher the sensitive level, the earlier the monitor will react and punish the cheater."));

            // Display an info message.
            EditorGUILayout.HelpBox("The AntiCheat Monitor calculates a threat level based on detected possible cheating. The threat level increases by a calculation of the sensitivity, detected threat and its possibility of being a false positive.", MessageType.Info);

            // Display the attached monitors.
            var var_Monitors = this.GetMonitors();

            GUILayout.Label(new GUIContent("Monitors:", "There are " + var_Monitors.Length + " Monitors attached on the game object or its children."));

            EditorGUILayout.HelpBox("A monitor is a component that observes and monitors the game or device state for possible cheating.", MessageType.Info);

            for (int i = 0; i < var_Monitors.Length; i++)
            {
                // Show monitor name.
                EditorGUILayout.LabelField(new GUIContent("-> Monitor " + (i + 1) + " - " + var_Monitors[i].Name));
            }

            // Display the attached detectors.
            var var_Detectors = this.GetDetectors();

            GUILayout.Label(new GUIContent("Detectors:", "There are " + var_Detectors.Length + " Detectors attached on the game object or its children."));

            EditorGUILayout.HelpBox("A detector attaches to a monitor. It calculates a threat level based on the observed state and notifies the AntiCheatMonitor about the detected threat.", MessageType.Info);
            
            for (int i = 0; i < var_Detectors.Length; i++)
            {
                // Show provider name.
                EditorGUILayout.LabelField(new GUIContent("-> Detector " + (i + 1) + " - " + var_Detectors[i].Name));

                // Indent.
                EditorGUI.indentLevel++;

                // Deactivate the ui editing.
                EditorGUI.BeginDisabledGroup(true);

                // Display if the detector is active.
                EditorGUILayout.Toggle(new GUIContent("Is Active", "Gets whether the detector is active and watching for possible cheating."), var_Detectors[i].IsActive);

                // Display its threat rating as label.
                EditorGUILayout.TextField(new GUIContent("Threat Rating", "The threat rating of the detector."), var_Detectors[i].ThreatRating.ToString());

                // End deactivation.
                EditorGUI.EndDisabledGroup();

                // Unindent.
                EditorGUI.indentLevel--;
            }

            // Display the attached punishers.
            var var_Punishers = this.GetPunishers();

            GUILayout.Label(new GUIContent("Punishers:", "There are " + var_Punishers.Length + " Punishers attached on the game object or its children."));

            EditorGUILayout.HelpBox("A punisher attaches to the anti cheat monitor. It administers punitive actions based on the observed state and the detected threat.", MessageType.Info);

            for (int i = 0; i < var_Punishers.Length; i++)
            {
                // Show provider name.
                EditorGUILayout.LabelField(new GUIContent("-> Punisher " + (i + 1) + " - " + var_Punishers[i].Name));

                // Indent.
                EditorGUI.indentLevel++;

                // Deactivate the ui editing.
                EditorGUI.BeginDisabledGroup(true);

                // Display if the punisher is active.
                EditorGUILayout.Toggle(new GUIContent("Is Active", "Gets whether the punisher is active."), var_Punishers[i].IsActive);

                // Display its threat rating as label.
                EditorGUILayout.TextField(new GUIContent("Punished Threat Level", "The threat rating of the punisher, when the accumulated threat level reached by the AntiCheat Monitor it will punish the cheater."), var_Punishers[i].ThreatRating.ToString());

                // Display if it should only punish once.
                EditorGUILayout.Toggle(new GUIContent("Punish Once", "If the punisher should only administer punitive actions once or any time the threat level exceeds the threat rating."), var_Punishers[i].PunishOnce);

                // End deactivation.
                EditorGUI.EndDisabledGroup();

                // Unindent.
                EditorGUI.indentLevel--;
            }

            // Apply modified properties.
            this.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Get all monitors of the target object and its children.
        /// </summary>
        /// <returns>An array of monitor.</returns>
        protected IMonitor[] GetMonitors()
        {
            // Get the target object.
            MonoBehaviour var_Target = this.serializedObject.targetObject as MonoBehaviour;

            // Get the monitors.
            var var_Monitors = var_Target.GetComponentsInChildren<IMonitor>();

            // Return the monitors.
            return var_Monitors;
        }

        /// <summary>
        /// Get all detectors of the target object and its children.
        /// </summary>
        /// <returns>An array of detector.</returns>
        protected IDetector[] GetDetectors()
        {
            // Get the target object.
            MonoBehaviour var_Target = this.serializedObject.targetObject as MonoBehaviour;

            // Get the detectors.
            var var_Detectors = var_Target.GetComponentsInChildren<IDetector>();

            // Return the detectors.
            return var_Detectors;
        }

        /// <summary>
        /// Get all punishers of the target object and its children.
        /// </summary>
        /// <returns>An array of punisher.</returns>
        protected IPunisher[] GetPunishers()
        {
            // Get the target object.
            MonoBehaviour var_Target = this.serializedObject.targetObject as MonoBehaviour;

            // Get the punishers.
            var var_Punishers = var_Target.GetComponentsInChildren<IPunisher>();

            // Return the punishers.
            return var_Punishers;
        }
    }
}
