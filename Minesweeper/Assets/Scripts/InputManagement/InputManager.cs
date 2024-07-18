using System.IO.Enumeration;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

    [DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour //: Singleton.Behaviour<InputManager>
{
    public static InputManager Instance { get; private set; }

    #region Events    
    // InputActions
    public InputAction leftPress; // A key, Left Arrow
    public InputAction rightPress; // D key, Right Arrow
    public InputAction rotateClockwisePress;
    public InputAction rotateCounterClockwisePress;
    public InputAction softDropPress;
    public InputAction hardDroptPress;
    public InputAction holdPress;
    public InputAction escapePress; // Escape key
    public InputAction restartPress; // Escape key
    public InputAction hardClearPress;
    public InputAction revealTilePress;
    public InputAction flagTilePress;
    public InputAction chordTilePress;
    public InputAction chordFlagTilePress;
    public InputAction anyKey; // Any key pressed
    public InputAction inputScroll; // Scroll input
    public InputAction cleansePress;
    public InputAction fullScreenTogglePress;


    /*public InputAction inputPressPrimary; // Left Click, (mobile) Tap
    public InputAction inputPressSecondary; // Right Click, (mobile) Hold
    public InputAction primaryTouchContact;
    public InputAction secondaryTouchContact;
    public InputAction anyKey; // Any key pressed
    public InputAction inputScroll; // Scroll input
    public InputAction escapePress; // Escape key
    public InputAction enterPress; // Enter key
    public InputAction spacePress; // Space key
    public InputAction tabPress; // Tab key
    public InputAction backspacePress; // Backspace key
    public InputAction forwardPress; // W key, Up Arrow
    public InputAction backwardPress; // S key, Down Arrow
    public InputAction leftPress; // A key, Left Arrow
    public InputAction rightPress; // D key, Right Arrow*/
    #endregion

    public static ControlInput controlInput;
    public static event Action rebindComplete;
    public static event Action rebindCanceled;
    public static event Action<InputAction, int> rebindStarted;

    private Camera mainCamera;

    protected void Awake() //override
    {
        // If there is an instance, and it's not me, delete myself.    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 

        //base.Awake();
        SetBindings();

        /*// Drag Input
        inputPressPrimary.started += ctx => StartDragPrimary(ctx); //InputDelay(StartDragPrimary, ctx);
        inputPressPrimary.canceled += ctx => EndDragPrimary(ctx);

        controlInput.InputMap.InputSecondaryDrag.started += ctx => StartDragSecondary(ctx); //InputDelay(StartDragSecondary, ctx);
        controlInput.InputMap.InputSecondaryDrag.performed += ctx => PerformDragSecondary(ctx);
        controlInput.InputMap.InputSecondaryDrag.canceled += ctx => EndDragSecondary(ctx);

        // Zoom Input - Touch
        secondaryTouchContact.started += ctx => ZoomTouchStart();
        secondaryTouchContact.canceled += _ => ZoomTouchEnd();
        primaryTouchContact.canceled += _ => ZoomTouchEnd();*/

        /*foreach (var binding in controlInput.asset.FindAction("Chord Tile").bindings) //controlInput.asset.FindAction(actionName);
        {
            Debug.Log("chordTilePress " + binding);
        }

        foreach (var binding in chordFlagTilePress.bindings)
        {
            Debug.Log("chordFlagTilePress " + binding);
        }*/

        //Debug.Log(chordTilePress.bindings);
        //Debug.Log(chordFlagTilePress.bindings);
        /*for (int b1 = 0; b1 < GetAction("Chord Tile").bindings.Count; b1++)
        {
            for (int b2 = 0; b2 < GetAction("Chord Flag Tile").bindings.Count; b2++)
            {
                Debug.Log(chordTilePress.bindings[b1].effectivePath + ", " + chordFlagTilePress.bindings[b2].effectivePath);
                if (chordTilePress.bindings[b1].effectivePath == chordFlagTilePress.bindings[b2].effectivePath)
                {
                   Debug.Log("RESET CHORDS:  " + chordTilePress.bindings[b1].effectivePath + ", " + chordFlagTilePress.bindings[b2].effectivePath);

                    ResetBinding("Chord Tile", b1);
                    ResetBinding("Chord Flag Tile", b2);
                    break;
                }
            }
        }*/

        /*foreach (InputBinding binding in action.actionMap.bindings)
        {            
            if (binding.action == newBinding.action)
            {
                continue;
            }
            else if (binding.effectivePath == newBinding.effectivePath)
            {
                if ((binding.action == "Flag Tile" && newBinding.action == "Chord Tile") || (newBinding.action == "Flag Tile" && binding.action == "Chord Tile")
                    || (binding.action == "Reveal Tile" && newBinding.action == "Chord Tile") || (newBinding.action == "Reveal Tile" && binding.action == "Chord Tile"))
                {
                    continue;
                }
                else if (binding.action == "Chord Flag Tile" || newBinding.action == "Chord Tile" || newBinding.action == "Chord Flag Tile" || binding.action == "Chord Tile")
                {
                    if ((binding.action == "Chord Flag Tile" && newBinding.action == "Chord Tile") || (newBinding.action == "Chord Flag Tile" && binding.action == "Chord Tile"))
                    {
                        //Debug.Log("Duplicate binding found: " + newBinding.effectivePath + " with " + binding.action + " --- "+ binding.groups + ", " + UnityEngine.InputSystem.InputActionRebindingExtensions.GetBindingIndex(action, binding.groups, binding.effectivePath));
                        if (UnityEngine.InputSystem.InputActionRebindingExtensions.GetBindingIndex(action, binding.groups, binding.effectivePath) != -1)
                        {
                            ResetBinding(binding.action, UnityEngine.InputSystem.InputActionRebindingExtensions.GetBindingIndex(action, binding.groups, binding.effectivePath));
                            ResetBinding(newBinding.action, bindingIndex);
                            return false;
                        } //TODO
                        return true;
                    }
                    continue;
                }
                else
                {
                    Debug.Log("Duplicate binding found: " + newBinding.effectivePath + " with " + binding.action);
                    return true;
                }                
            }
        }*/
    }

    public bool CheckForIncompatibleBindings()
    {
        
        return false;
    }

    public void SetBindings()
    {
        controlInput = new ControlInput();
        mainCamera = Camera.main;

        leftPress = controlInput.TetrisweepMap.Left;
        rightPress = controlInput.TetrisweepMap.Right;
        rotateClockwisePress = controlInput.TetrisweepMap.RotateClockwise;
        rotateCounterClockwisePress = controlInput.TetrisweepMap.RotateCounterClockwise;
        softDropPress = controlInput.TetrisweepMap.SoftDrop;
        hardDroptPress = controlInput.TetrisweepMap.HardDrop;
        holdPress = controlInput.TetrisweepMap.Hold;
        escapePress = controlInput.TetrisweepMap.Escape;
        restartPress = controlInput.TetrisweepMap.Restart;
        hardClearPress = controlInput.TetrisweepMap.HardClear;
        revealTilePress = controlInput.TetrisweepMap.RevealTile;
        flagTilePress = controlInput.TetrisweepMap.FlagTile;
        chordTilePress = controlInput.TetrisweepMap.ChordTile;
        chordFlagTilePress = controlInput.TetrisweepMap.ChordFlagTile;
        anyKey = controlInput.TetrisweepMap.AnyKey;
        inputScroll = controlInput.TetrisweepMap.InputScroll;
        cleansePress = controlInput.TetrisweepMap.Cleanse;
        fullScreenTogglePress = controlInput.TetrisweepMap.FullScreenToggle;
    }

    private void OnEnable()
    {
        controlInput.Enable();
    }

    private void OnDisable()
    {
        controlInput.Disable();
    }

    public Vector2 GetMousePosition()
    {
        return controlInput.TetrisweepMap.MousePosition.ReadValue<Vector2>();
    }

        #region Rebind
    public static void LoadBindingOverride(string actionName)
    {
        if (controlInput == null)
            controlInput = new ControlInput();

        InputAction action = controlInput.asset.FindAction(actionName);

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
                action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
        }
    }
    public static InputAction GetAction(string actionName)
    {
        if (controlInput == null)
            controlInput = new ControlInput();

        return controlInput.asset.FindAction(actionName);
    }

    public static string GetBindingName(string actionName, int bindingIndex)
    {
        if (controlInput == null)
            controlInput = new ControlInput();

        InputAction action = controlInput.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }

    public static void StartRebind(string actionName, int bindingIndex, TMP_Text statusText, bool excludeMouse, bool allowDuplicates = false)
    {
        InputAction action = controlInput.asset.FindAction(actionName);
        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Couldn't find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isComposite)
                DoRebind(action, bindingIndex, statusText, true, excludeMouse, allowDuplicates);
        }
        else
            DoRebind(action, bindingIndex, statusText, false, excludeMouse, allowDuplicates);
    }

    private static void DoRebind(InputAction actionToRebind, int bindingIndex, TMP_Text statusText, bool allCompositeParts, bool excludeMouse, bool allowDuplicates)
    {
        if (actionToRebind == null || bindingIndex < 0)
            return;

        statusText.text = $"Press a {actionToRebind.expectedControlType}";

        actionToRebind.Disable();

        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            if (!allowDuplicates || actionToRebind.name == "Chord Tile" || actionToRebind.name == "Chord Flag Tile")
            {
                if (CheckDuplicateBindings(actionToRebind, bindingIndex, allCompositeParts))
                {
                    actionToRebind.RemoveBindingOverride(bindingIndex);
                    DoRebind(actionToRebind, bindingIndex, statusText, allCompositeParts, excludeMouse, allowDuplicates);
                    return;
                }
            } 

            if(allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isComposite)
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse, allowDuplicates);
            }

            SaveBindingOverride(actionToRebind);
            rebindComplete?.Invoke();
        });

        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            rebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        if (excludeMouse)
            rebind.WithControlsExcluding("Mouse");

        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start(); //actually starts the rebinding process
    }

    private static void SaveBindingOverride(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static bool CheckDuplicateBindings(InputAction action, int bindingIndex, bool allCompositeParts = false, bool duplicateBindingsForThisAction = false, bool resetChordBindings = true)
    {        
        InputBinding newBinding = action.bindings[bindingIndex];
        foreach (InputBinding binding in action.actionMap.bindings)
        {            
            if (binding.action == newBinding.action)
            {
                continue;
            }
            else if (binding.effectivePath == newBinding.effectivePath)
            {
                if ((binding.action == "Flag Tile" && newBinding.action == "Chord Tile") || (newBinding.action == "Flag Tile" && binding.action == "Chord Tile")
                    || (binding.action == "Reveal Tile" && newBinding.action == "Chord Tile") || (newBinding.action == "Reveal Tile" && binding.action == "Chord Tile"))
                {
                    continue;
                }
                else if (binding.action == "Chord Flag Tile" || newBinding.action == "Chord Tile" || newBinding.action == "Chord Flag Tile" || binding.action == "Chord Tile")
                {
                    if ((binding.action == "Chord Flag Tile" && newBinding.action == "Chord Tile") || (newBinding.action == "Chord Flag Tile" && binding.action == "Chord Tile"))
                    {
                        Debug.Log("CHORD DUPLICATE FOUND: " + newBinding.effectivePath + " with " + binding.action + " --- "+ binding.groups);

                        if (resetChordBindings)
                        {
                            ResetBinding("Chord Tile", 0);
                            ResetBinding("Chord Tile", 1);
                            ResetBinding("Chord Tile", 2);
                            ResetBinding("Chord Flag Tile", 0);
                            ResetBinding("Chord Flag Tile", 1);
                            /*if (UnityEngine.InputSystem.InputActionRebindingExtensions.GetBindingIndex(action, binding.groups, binding.effectivePath) != -1)
                            {
                                ResetBinding(binding.action, UnityEngine.InputSystem.InputActionRebindingExtensions.GetBindingIndex(action, binding.groups, binding.effectivePath));
                                ResetBinding(newBinding.action, bindingIndex);
                                return false;
                            }*/ //TODO
                        }                        
                        return false;
                    }
                    continue;
                }
                else
                {
                    Debug.Log("Duplicate binding found: " + newBinding.effectivePath + " with " + binding.action);
                    return true;
                }                
            }
        }
        // Check for duplicate composite bindings
        if (allCompositeParts)
        {
            for (int i = 1; i < bindingIndex; ++i)
            {
                if (action.bindings[i].effectivePath == newBinding.effectivePath)
                {
                    Debug.Log("Duplicate binding found: " + newBinding.effectivePath + " at index (other: " + i + ") and (this: " + bindingIndex + ")");
                    return true;
                }
            }
        }
        // Check for duplicate bindings within the same action
        if (duplicateBindingsForThisAction)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (i != bindingIndex)
                {
                    if (action.bindings[i].effectivePath == newBinding.effectivePath)
                    {
                        Debug.Log("Duplicate binding found: " + newBinding.effectivePath + " at index (other: " + i + ") and (this: " + bindingIndex + ")");
                        return true;
                    }
                }                
            }
        }
        return false;
    }
    public static void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = controlInput.asset.FindAction(actionName);

        if(action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Could not find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
                action.RemoveBindingOverride(i);
        }
        else
            action.RemoveBindingOverride(bindingIndex);

        SaveBindingOverride(action);
        rebindComplete?.Invoke();
    }
    #endregion

    #region Helper
    private Vector3 ScreenToWorld(Vector3 point) {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(point);
        worldPos.z = mainCamera.nearClipPlane;
        return worldPos;
     }
    #endregion
}
