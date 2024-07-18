using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ManageFullscreenSwitch : MonoBehaviour
{
    private GameManager gm;
    private int _fullscreenWidth = 0;
    private int _fullscreenHeight = 0;
    private int _fullscreenAspectRatio = 0;

    private int _halfscreenWidth = 0;
    private int _halfscreenHeight = 0;
    private int _halfscreenAspectRatio = 0;

    private bool _fullscreen = false;

    //private Resolution _resolution;
    Vector2 lastScreenSize;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        _fullscreen = Screen.fullScreen;
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        SetFullScreenValues();
    }
    private void Update()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        if (this.lastScreenSize != screenSize)
        {
            this.lastScreenSize = screenSize;
            gm.SetCameraScale();
        }

        if (_fullscreen != Screen.fullScreen)
        {
            if (Screen.fullScreen)
            {
                RestoreFullscreenResolution();
            }
            else
            {
                RestoreHalfscreenResolution();
            }

            _fullscreen = Screen.fullScreen;
        }        
    }

    private void RestoreFullscreenResolution()
    {
        Screen.SetResolution(_fullscreenWidth, _fullscreenHeight, true, _fullscreenAspectRatio);
        
        if (gm.isTitleMenu)
            gm.ReloadScene();
        else
            gm.SetCameraScale();
    }

    private void RestoreHalfscreenResolution()
    {
        Screen.SetResolution(_halfscreenWidth, _halfscreenHeight, false, _halfscreenAspectRatio);

        if (gm.isTitleMenu)
            gm.ReloadScene();
        else
            gm.SetCameraScale();
    }

    private void SetFullScreenValues()
    {
        // Set the screen width and height
        int systemWidth = Display.main.systemWidth;
        int systemHeight = Display.main.systemHeight;

        

        // Get a list of all supported resolutions
        Resolution[] supportedResolutions = Screen.resolutions;

        // Find the closest supported resolution to the native resolution
        Resolution closestResolution = supportedResolutions[0];
        int smallestGapInResolution = int.MaxValue;

        Resolution closestHalfResolution = supportedResolutions[0];
        int smallestGapInHalfResolution = int.MaxValue;

        foreach (Resolution resolution in supportedResolutions)
        {
            int gap = Mathf.Abs(resolution.width - systemWidth) + Mathf.Abs(resolution.height - systemHeight);
            int gapHalf = Mathf.Abs(resolution.width - (systemWidth / 2)) + Mathf.Abs(resolution.height - (systemHeight / 2));

            if (gap < smallestGapInResolution)
            {
                smallestGapInResolution = gap;
                closestResolution = resolution;
            }

            if (gapHalf < smallestGapInHalfResolution)
            {
                smallestGapInHalfResolution = gapHalf;
                closestHalfResolution = resolution;
            }
        }

        _fullscreenWidth = closestResolution.width;
        _fullscreenHeight = closestResolution.height;
        _fullscreenAspectRatio = _fullscreenWidth / _fullscreenHeight;

        _halfscreenWidth = closestHalfResolution.width;
        _halfscreenHeight = closestHalfResolution.height;
        _halfscreenAspectRatio = _halfscreenWidth / _halfscreenHeight;
    }
}
