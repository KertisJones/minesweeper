using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource multiplierDrainSource;
    private GameManager gm;
    public AudioClip tileRevealSound1;
    public AudioClip tileRevealSound2;
    public AudioClip tileRevealSound3;
    public AudioClip tileRevealSound4;
    public AudioClip tileRevealSound5;
    public AudioClip tileRevealSound6;
    public AudioClip tileRevealSound7;
    public AudioClip tileRevealSound8;
    public AudioClip tileRevealSound9;
    public AudioClip tileRevealSound10;
    public AudioClip tileRevealSound11;
    public AudioClip tileRevealSound12;
    public AudioClip tileRevealSound13;
    public AudioClip multiplierDrainStartSound;
    private bool tileRevealedThisFrame = false;
    private int tilesRevealedPitch = 0;
    private float tilesRevealedCooldownTimer = 0f;
    private int currentLevel = 1;

    private Tween multiplierDrainTween;

    void OnDestroy()
    {
        multiplierDrainTween = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        musicSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
        gm = GetComponent<GameManager>();
        currentLevel = gm.level;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLevel != gm.level)
            SetMusicPitchToLevel(gm.level);
    }

    void LateUpdate()
    {
        if (tileRevealedThisFrame)
        {
            tileRevealedThisFrame = false;
            if (tilesRevealedPitch > 13)
                tilesRevealedPitch = 7;
            switch (tilesRevealedPitch)
            {
                case 1:
                    AudioSource.PlayClipAtPoint(tileRevealSound1, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 2:
                    AudioSource.PlayClipAtPoint(tileRevealSound2, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 3:
                    AudioSource.PlayClipAtPoint(tileRevealSound3, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 4:
                    AudioSource.PlayClipAtPoint(tileRevealSound4, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 5:
                    AudioSource.PlayClipAtPoint(tileRevealSound5, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 6:
                    AudioSource.PlayClipAtPoint(tileRevealSound6, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 7:
                    AudioSource.PlayClipAtPoint(tileRevealSound7, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 8:
                    AudioSource.PlayClipAtPoint(tileRevealSound8, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 9:
                    AudioSource.PlayClipAtPoint(tileRevealSound9, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 10:
                    AudioSource.PlayClipAtPoint(tileRevealSound10, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 11:
                    AudioSource.PlayClipAtPoint(tileRevealSound11, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 12:
                    AudioSource.PlayClipAtPoint(tileRevealSound12, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 13:
                    AudioSource.PlayClipAtPoint(tileRevealSound13, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                default:
                    AudioSource.PlayClipAtPoint(tileRevealSound13, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;                
            }            
        }
        tilesRevealedCooldownTimer -= Time.deltaTime;
        if (tilesRevealedCooldownTimer < 0)
            ResetTileRevealPitch();
            
    }

    public void PlayTileRevealSound()
    {
        if (!tileRevealedThisFrame)
        {
            tilesRevealedPitch++;
            tileRevealedThisFrame = true;        
            tilesRevealedCooldownTimer = 1f;
        }
    }

    public void ResetTileRevealPitch()
    {
        tilesRevealedPitch = 0;
    }

    public void SetMusicPitchToLevel(int level)
    {
        currentLevel = gm.level;
        float maxPitchIncrease = 0.5f;
        float pitchIncreasePerLevel = maxPitchIncrease / 19f;        
        float currentPitch = 1 + ((level - 1) * pitchIncreasePerLevel);
        if (level >= 20)
            currentPitch = 1 + maxPitchIncrease;

        musicSource.DOPitch(currentPitch, 1);
    }

    public void EnablePauseFilter()
    {
        if (musicSource.GetComponent<AudioReverbFilter>() != null)
            musicSource.GetComponent<AudioReverbFilter>().enabled = true;
        if (musicSource.GetComponent<AudioHighPassFilter>() != null)
            musicSource.GetComponent<AudioHighPassFilter>().enabled = true;
    }

    public void DisablePauseFilter()
    {
        if (musicSource.GetComponent<AudioReverbFilter>() != null)
            musicSource.GetComponent<AudioReverbFilter>().enabled = false;
        if (musicSource.GetComponent<AudioHighPassFilter>() != null)
            musicSource.GetComponent<AudioHighPassFilter>().enabled = false;
    }

    public void PlayMultiplierDrain()
    {        
        if (multiplierDrainTween != null)
            if (multiplierDrainTween.IsActive())
                if (multiplierDrainTween.IsPlaying())
                    return;        
        if (multiplierDrainSource.volume > 0)
            return;
        
        multiplierDrainTween = multiplierDrainSource.DOFade(PlayerPrefs.GetFloat("SoundVolume", 0.5f), 1f);
    }

    public void StopMultiplierDrain() 
    { 
        if (multiplierDrainTween != null)       
            multiplierDrainTween.Kill();
        multiplierDrainSource.DOFade(0, 0.5f).SetUpdate(true);//.OnKill(multiplierDrainSource.Stop);
    }
}
