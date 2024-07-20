using UnityEngine;
using UnityEngine.UI;

public class WebGLMaterialSwitcher : MonoBehaviour
{
    // Reference to the default material for WebGL
    public Material webGLDefaultMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
#if UNITY_WEBGL
        if (webGLDefaultMaterial != null)
        {
            if (GetComponent<SpriteRenderer>() != null)
                GetComponent<SpriteRenderer>().material = webGLDefaultMaterial;
            if (GetComponent<Image>() != null)
                GetComponent<Image>().material = webGLDefaultMaterial;
        }
#endif
    }
}
