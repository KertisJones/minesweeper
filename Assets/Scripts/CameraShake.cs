using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;
    public bool isActive = true;

    // How long the object should shake for.
    private float currentShakeDuration = 0.25f;
    private float shakeDurationClock = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    private float shakeStrength = 0.7f;


	public float durationModifier = 1.0f;
    public float shakeModifier = 1.0f;
	
	Vector3 originalPos;

    public ParticleSystem shakeParticles;
	
	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}
	
	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	void Update()
	{
        if (shakeDurationClock > 0)
        {
            if (shakeParticles != null)
                shakeParticles.Play();
            Vector3 targetPos = GetComponent<UnityStandardAssets._2D.Camera2DFollow>().m_CurrentPosition;
            targetPos = new Vector3(targetPos.x, targetPos.y + GetComponent<UnityStandardAssets._2D.Camera2DFollow>().yAddValue, targetPos.z);
            if (targetPos.y < GetComponent<UnityStandardAssets._2D.Camera2DFollow>().yMinValue)
                targetPos = new Vector3(transform.position.x, GetComponent<UnityStandardAssets._2D.Camera2DFollow>().yMinValue, transform.position.z);

            //targetPos = new Vector3(originalPos.x, targetPos.y + GetComponent<UnityStandardAssets._2D.Camera2DFollow>().yAddValue, originalPos.z); //For 2D games only

            originalPos = targetPos;

            float currentShakeStrength = Mathf.Lerp(0, shakeStrength, shakeDurationClock / currentShakeDuration);
            //Debug.Log("Shake Strength: " + currentShakeStrength);

            camTransform.localPosition = originalPos + Random.insideUnitSphere * currentShakeStrength;

            //Handheld.Vibrate();

            

            shakeDurationClock -= Time.unscaledDeltaTime;
        }
        else {
            if (shakeParticles != null)
                shakeParticles.Stop();
        }
	}

    public void Shake(float duration = 0.25f, float strength = 1)
    {
        if (!isActive)
            return;
        
        if (shakeDurationClock < duration * durationModifier)
        {
            shakeDurationClock = duration * durationModifier;
            currentShakeDuration = duration * durationModifier;

            shakeStrength = strength * shakeModifier;
            originalPos = camTransform.localPosition;
        }        
    }
}
