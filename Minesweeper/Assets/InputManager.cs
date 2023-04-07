using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

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
    public InputAction anyKey; // Any key pressed
    public InputAction inputScroll; // Scroll input
    public InputAction cleansePress;
    
    
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

    private ControlInput controlInput;
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
        anyKey = controlInput.TetrisweepMap.AnyKey;
        inputScroll = controlInput.TetrisweepMap.InputScroll;
        cleansePress = controlInput.TetrisweepMap.Cleanse;

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
    }

    private void OnEnable()
    {
        controlInput.Enable();
    }

    private void OnDisable()
    {
        controlInput.Disable();
    }

    #region Helper
    private Vector3 ScreenToWorld(Vector3 point) {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(point);
        worldPos.z = mainCamera.nearClipPlane;
        return worldPos;
     }
    #endregion

    public Vector2 GetMousePosition()
    {
        return controlInput.TetrisweepMap.MousePosition.ReadValue<Vector2>();
    }
}
