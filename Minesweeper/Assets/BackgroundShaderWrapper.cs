using UnityEngine;

public class BackgroundShaderWrapper : MonoBehaviour
{
    private Material material;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("_UnscaledTime", Time.unscaledTime);
    }
}
