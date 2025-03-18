using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectSpawner : MonoBehaviour
{
    private Camera mainCamera;
    public Volume volume;
    public Renderer2DData renderer2DData;
    public Material materialCRT;
    LensDistortion lensDistortion;
    Vignette vignette;
    ClampedFloatParameter vignetteStartingSmoothness;
    FullScreenPassRendererFeature fullscreenRenderer;

    public bool isCRT;

    [System.Serializable]
    public struct BackgroundType
    {
        public string Name;
        public GameObject Background;
        public BackgroundType(string name, GameObject background)
        {
            this.Name = name;
            this.Background = background;
        }
    }

    public BackgroundType[] backgroundTypes;
    private GameObject activeBackground;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        SetUpVolumeReferences();

        SetActiveBackground("random");

        if (isCRT)
            ActivateCRT();
        else
            DeactivateCRT();
    }

    public void SetActiveBackground(string newBackgroundName)
    {
        if (activeBackground != null)
            GameObject.Destroy(activeBackground);
        GameObject newBackground = null;
        if (newBackgroundName == "default")
            return;
        else if (newBackgroundName == "random")
        {
            newBackground = backgroundTypes[Random.Range(0, backgroundTypes.Length)].Background;
        }
        else
        {
            foreach (BackgroundType bg in backgroundTypes)
            {
                if (bg.Name == newBackgroundName)
                    newBackground = bg.Background;
            }
        }
        if (newBackground != null)
            activeBackground = Instantiate(newBackground, this.transform);
    }

    void SetUpVolumeReferences()
    {
        LensDistortion ld;
        if (volume.profile.TryGet<LensDistortion>(out ld))
        {
            lensDistortion = ld;
        }

        Vignette vg;
        if (volume.profile.TryGet<Vignette>(out vg))
        {
            vignette = vg;
            vignetteStartingSmoothness = vignette.smoothness;
        }

        FullScreenPassRendererFeature fs;
        if (renderer2DData.TryGetRendererFeature<FullScreenPassRendererFeature>(out fs))
        {
            fullscreenRenderer = fs;
        }
    }

    public void ActivateCRT()
    {
        if (lensDistortion !=  null)
            lensDistortion.active = true;
        if (vignette != null)
            vignette.smoothness = new ClampedFloatParameter(1, 0, 1);
        if (fullscreenRenderer != null)
        {
            fullscreenRenderer.SetActive(true);
            fullscreenRenderer.passMaterial = materialCRT;
        }
        isCRT = true;
    }

    public void DeactivateCRT()
    {
        if (lensDistortion != null)
            lensDistortion.active = false;
        if (vignette != null)
            vignette.smoothness = vignetteStartingSmoothness;
        if (fullscreenRenderer != null)
            fullscreenRenderer.SetActive(false);
        isCRT = false;
    }
}
