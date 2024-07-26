using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource multiplierDrainSource;
    [SerializeField]
    private AudioSource fireSource;
    private GameManager gm;
    public AudioClip[] tileRevealSounds;
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
    public AudioClip tileRevealComboResetSound;
    public AudioClip multiplierDrainStartSound;
    private bool tileRevealedThisFrame = false;
    private bool tileRevealComboIsActive = false;
    private int tilesRevealedPitch = 0;
    private int tilesRevealedManually = 0;
    private float tilesRevealedCooldownTimer = 0f;
    private int currentLevel = 1;
    private int currentBurningTiles = 0;

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
        
        currentBurningTiles = gm.numBurningTiles;
        float tilesNeededForMaxVolume = 40;
        float tilesAdjusted = Mathf.Min(currentBurningTiles, tilesNeededForMaxVolume);

        if (tilesAdjusted > 0) 
            fireSource.volume = (tilesAdjusted / tilesNeededForMaxVolume) * PlayerPrefs.GetFloat("SoundVolume", 0.5f);
    }

    void LateUpdate()
    {
        if (tileRevealedThisFrame)
        {
            tileRevealedThisFrame = false;

            int n1 = tilesRevealedPitch % ((tileRevealSounds.Length - 1) * 2);
            int n2 = n1;
            
            if (n1 >= tileRevealSounds.Length)
                n2 = (tileRevealSounds.Length - 2) - (n1 % tileRevealSounds.Length);

            //Debug.Log("n1:" + n1 + ", n2:" + n2 + ", len:" + tileRevealSounds.Length);

            PlayClip(tileRevealSounds[n2], 0.8f, false);
            tilesRevealedPitch++;

            /*switch (tilesRevealedPitch)
            {
                case 0:
                    AudioSource.PlayClipAtPoint(tileRevealSound1, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 1:
                    AudioSource.PlayClipAtPoint(tileRevealSound2, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 2:
                    AudioSource.PlayClipAtPoint(tileRevealSound3, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 3:
                    AudioSource.PlayClipAtPoint(tileRevealSound4, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 4:
                    AudioSource.PlayClipAtPoint(tileRevealSound5, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 5:
                    AudioSource.PlayClipAtPoint(tileRevealSound6, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 6:
                    AudioSource.PlayClipAtPoint(tileRevealSound7, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 7:
                    AudioSource.PlayClipAtPoint(tileRevealSound8, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 8:
                    AudioSource.PlayClipAtPoint(tileRevealSound9, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 9:
                    AudioSource.PlayClipAtPoint(tileRevealSound10, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 10:
                    AudioSource.PlayClipAtPoint(tileRevealSound11, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 11:
                    AudioSource.PlayClipAtPoint(tileRevealSound12, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                case 12:
                    AudioSource.PlayClipAtPoint(tileRevealSound13, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;
                default:
                    AudioSource.PlayClipAtPoint(tileRevealSound13, new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                    break;                
            }*/
            
        }
        
        if (tileRevealComboIsActive)
        {
            tilesRevealedCooldownTimer -= Time.deltaTime;
            if (tilesRevealedCooldownTimer <= 0)
                ResetTileRevealPitch(true);
        }            
    }

    public void PlayTileRevealSound(bool isManual = false)
    {
        if (!tileRevealedThisFrame)
        {
            tileRevealedThisFrame = true;        
            tileRevealComboIsActive = true;
            tilesRevealedCooldownTimer = 1.5f;
            if (isManual)
                tilesRevealedManually++;
        }
    }

    public void ResetTileRevealPitch(bool playSoundEffect = false)
    {
        tilesRevealedPitch = 0;
        tileRevealComboIsActive = false;

        if (playSoundEffect && tilesRevealedManually > 4)
            gm.soundManager.PlayClip(tileRevealComboResetSound, 1, true);
        tilesRevealedManually = 0;

        gm.ResetRevealCombo();
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
        if (Application.platform != RuntimePlatform.WebGLPlayer || true)
        {
            if (musicSource.GetComponent<AudioReverbFilter>() != null)
                musicSource.GetComponent<AudioReverbFilter>().enabled = true;
            if (musicSource.GetComponent<AudioHighPassFilter>() != null)
                musicSource.GetComponent<AudioHighPassFilter>().enabled = true;
        }        
    }

    public void DisablePauseFilter()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer || true)
        {
            if (musicSource.GetComponent<AudioReverbFilter>() != null)
                musicSource.GetComponent<AudioReverbFilter>().enabled = false;
            if (musicSource.GetComponent<AudioHighPassFilter>() != null)
                musicSource.GetComponent<AudioHighPassFilter>().enabled = false;
        }
    }

    public void PlayMultiplierDrain()
    {        
        if (multiplierDrainTween != null)
            if (multiplierDrainTween.IsActive())
                if (multiplierDrainTween.IsPlaying())
                    return;        
        if (multiplierDrainSource.volume > 0 || gm.isGameOver)
            return;
        
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            multiplierDrainTween = multiplierDrainSource.DOFade(PlayerPrefs.GetFloat("SoundVolume", 0.5f), 1f);
        else
            multiplierDrainSource.volume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        ResetTileRevealPitch();
    }

    public void StopMultiplierDrain() 
    { 
        if (multiplierDrainTween != null)       
            multiplierDrainTween.Kill();

        multiplierDrainSource.DOFade(0, 0.5f).SetUpdate(true);//.OnKill(multiplierDrainSource.Stop);
        /*if (Application.platform != RuntimePlatform.WebGLPlayer)
            multiplierDrainSource.DOFade(0, 0.5f).SetUpdate(true);//.OnKill(multiplierDrainSource.Stop);
        else
            multiplierDrainSource.volume = 0;*/
    }

    public AudioSource PlayClip(AudioClip clip, float volume, bool doRandomPitch)
    {
        var tempAudio = new GameObject("TempAudio"); // create the temp object
        tempAudio.transform.position = this.transform.position; // set its position
      
        var tempAudioSource = tempAudio.AddComponent(typeof(AudioSource)) as AudioSource; // add an audio source      
        tempAudioSource.clip = clip; // define the clip
        tempAudioSource.volume = volume * PlayerPrefs.GetFloat("SoundVolume", 0.5f);

        if (doRandomPitch)
            tempAudioSource.pitch = Random.Range(0.9f, 1.1f);

        tempAudioSource.Play(); // start the sound      
        Destroy(tempAudio, clip.length); // destroy object after clip duration      
        return tempAudioSource; // return the AudioSource reference
    }
}
