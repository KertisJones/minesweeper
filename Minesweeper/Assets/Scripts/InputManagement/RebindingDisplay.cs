using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class RebindingDisplay : MonoBehaviour
{
    [SerializeField]
    private InputActionReference inputActionReference; //this is on the SO

    [SerializeField]
    private bool excludeMouse = true;
    [Range(0, 10)]
    [SerializeField]
    private int selectedBinding;
    [SerializeField]
    private InputBinding.DisplayStringOptions displayStringOptions;
    [Header("Binding Info - DO NOT EDIT")]
    [SerializeField]
    private InputBinding inputBinding;
    private int bindingIndex;

    private string actionName;

    [Header("UI Fields")]
    [SerializeField]
    private TMP_Text actionText;
    [SerializeField]
    private Button rebindButton;
    [SerializeField]
    private TMP_Text rebindText;
    [SerializeField]
    private Button resetButton;
/*
Move Left: Left | A; Num 4
Move Right: Right | D; Num 6
Soft Drop: Down | S; Num 2
Hard Drop: Space | Enter; Num 8, Num Enter
Rotate Clockwise: Up, X | W, E; Num 1, Num 5, Num 9
Rotate Counter-Clockwise: Left Control, Z | Q; Num 3, Num 7
// Rotate 180: Alt | R
Hold: Shift | C; Num 0

Reveal Tile: LMB | N
Flag Tile: RMB | M
Chord Tile: MMB | ,
Hard Clear: T | F1, Backspace
Cleanse: F | F2
*/
    private void Start() 
    {
        UpdateUI();    
    }
    
    private void OnEnable()
    {
        rebindButton.onClick.AddListener(() => DoRebind());
        resetButton.onClick.AddListener(() => ResetBinding());

        if(inputActionReference != null)
        {            
            GetBindingInfo();
            InputManager.LoadBindingOverride(actionName);
            UpdateUI();
        }

        InputManager.rebindComplete += UpdateUI;
        InputManager.rebindCanceled += UpdateUI;
    }

    private void OnDisable()
    {
        InputManager.rebindComplete -= UpdateUI;
        InputManager.rebindCanceled -= UpdateUI;
    }

    private void OnValidate()
    {
        if (inputActionReference == null)
            return; 

        GetBindingInfo();
        UpdateUI();
    }

    private void GetBindingInfo()
    {
        if (inputActionReference.action != null)
            actionName = inputActionReference.action.name;

        if(inputActionReference.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }
    }

    private void UpdateUI()
    {
        InputAction action = InputManager.GetAction(actionName);
        if (actionText != null)
            actionText.text = actionName;

        if(rebindText != null)
        {
            //rebindText.text = inputActionReference.action.bindings[bindingIndex].ToDisplayString(displayStringOptions);//.effectivePath;
            
            if (Application.isPlaying)
            {
                rebindText.text = action.bindings[bindingIndex].ToDisplayString(displayStringOptions);
                //InputControlPath.ToHumanReadableString(inputActionReference.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);; 
                //InputManager.GetBindingName(actionName, bindingIndex);
            }
            else
                rebindText.text = inputActionReference.action.bindings[bindingIndex].ToDisplayString(displayStringOptions); //inputActionReference.action.GetBindingDisplayString(bindingIndex, displayStringOptions);
        }

        if (rebindButton != null)
        {
            if (InputManager.CheckDuplicateBindings(action, bindingIndex, action.bindings[bindingIndex].isComposite, true, false))
            {
                rebindButton.gameObject.GetComponent<Image>().color = Color.red;
                rebindText.color = Color.white;
            }
            else
            {
                rebindButton.gameObject.GetComponent<Image>().color = Color.white;
                rebindText.color = new Color(0.1960784f, 0.1960784f, 0.1960784f);
            }
        }
    }

    private void DoRebind()
    {
        InputManager.StartRebind(actionName, bindingIndex, rebindText, excludeMouse, true);
    }

    private void ResetBinding()
    {
        InputManager.ResetBinding(actionName, bindingIndex);
        UpdateUI();
    }

    /*[SerializeField] private InputManager playerController = null;
    [SerializeField] private TMP_Text bindingDisplayNameText = null;
    [SerializeField] private GameObject startRebindObject = null;
    [SerializeField] private GameObject waitingForInputObject = null;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private const string RebindsKey = "rebinds";

    private void Start()
    {
        string rebinds = PlayerPrefs.GetString(RebindsKey, string.Empty);

        if (string.IsNullOrEmpty(rebinds)) { return; }

        playerController.controlInput.LoadBindingOverridesFromJson(rebinds);
        inputActionReference.Set(playerController.controlInput.TetrisweepMap.RotateClockwise);

        int bindingIndex = inputActionReference.action.GetBindingIndexForControl(inputActionReference.action.controls[0]);

        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            inputActionReference.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void Save()
    {
        string rebinds = playerController.controlInput.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(RebindsKey, rebinds);
    }

    public void StartRebinding()
    {
        startRebindObject.SetActive(false);
        waitingForInputObject.SetActive(true);

        //playerController.PlayerInput.SwitchCurrentActionMap("Menu");
        inputActionReference.action.Disable();

        rebindingOperation = inputActionReference.action.PerformInteractiveRebinding() //.WithControlsExcluding("Mouse")
            //.WithTargetBinding(selectedBinding)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        int bindingIndex = inputActionReference.action.GetBindingIndexForControl(inputActionReference.action.controls[0]);

        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            inputActionReference.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        
        playerController.SetBindings();

        rebindingOperation.Dispose();

        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);
        
        inputActionReference.action.Enable();
        //playerController.PlayerInput.SwitchCurrentActionMap("Gameplay");
    }*/
}
