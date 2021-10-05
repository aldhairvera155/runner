using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AudioMixerManager : MonoBehaviour
{
    public static AudioMixerManager _instance;
    [SerializeField] private AudioMixer mixer;

    private bool isMusicOn = true;
    private bool isSFXOn = true;

    [Header("Music Sprites and Images")]
    
    [SerializeField] private Image musicButton, sfxButton;
    [SerializeField] private Sprite musicOn, musicOff;
    [SerializeField] private Sprite sfxOn, sfxOff;

    [Header("Background Music")]

    [SerializeField] private AudioClip background_Intro_Clip;
    [SerializeField] private AudioClip backgroundClip;
    [SerializeField] private AudioClip goodEndClip, badEndClip;
    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] private AudioSource background2Source;

    [Header("SFX")]

    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip catchClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip click_1_Clip ,click_2_Clip;
    [SerializeField] private AudioClip success_Clip ,fail_Clip;
    [SerializeField] private AudioSource sfxSource;

    private Coroutine currentCoroutine = null;
    
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if(_instance!=null)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("MusicLevel"))
            PlayerPrefs.SetInt("MusicLevel", 1);
        
        if (!PlayerPrefs.HasKey("SFXLevel"))
            PlayerPrefs.SetInt("SFXLevel", 1);

        isMusicOn = PlayerPrefs.GetInt("MusicLevel") == 1;
        isSFXOn = PlayerPrefs.GetInt("SFXLevel") == 1;

        // isMusicOn = SaveDataManager._instance.gameData.musicLevel;
        // isSFXOn = SaveDataManager._instance.gameData.sfxLevel;
        
        CheckSFXValue();
        CheckBackgroundValue();

        CheckButton_Music();
        CheckButton_SFX();

        if (SceneUtils.IsInGameplay())
        {
            if (currentCoroutine == null)
                currentCoroutine = StartCoroutine(InitBackground());
        }
        
        // backgroundSource.volume=
    }

    IEnumerator InitBackground()
    {
        backgroundSource.loop = false;
        backgroundSource.clip = background_Intro_Clip;
        backgroundSource.Play();

        yield return new WaitUntil(() => !backgroundSource.isPlaying);
        
        backgroundSource.clip = backgroundClip;
        backgroundSource.Play();
        backgroundSource.loop = true;
        currentCoroutine = null;
    }

    public void PlayBackgroundSource(bool _end)
    {
        //Make a fade effect
        // if (currentCoroutine == null)
        //     currentCoroutine = StartCoroutine(BackgroundFadeEffect(_end));
            
        AudioClip tempClip = _end ? goodEndClip : badEndClip;

        if (!_end)
            backgroundSource.loop = false;
        backgroundSource.clip = tempClip;
        backgroundSource.Play();
    }

    #region In case of changes

    IEnumerator BackgroundFadeEffect(bool _end)
    {
        float i = 0;
        // float maxValue = _end ? .5f : .4f;
        
        float maxValue = .4f;
        AudioClip tempClip = _end ? goodEndClip : badEndClip;

        background2Source.loop = _end;
        background2Source.clip = tempClip;
        background2Source.Play();
        
        while (i < maxValue)
        {
            backgroundSource.volume = .5f - i;
            i += Time.deltaTime;
            yield return null;
        }

        currentCoroutine = null;
    }

    public void PlayBackgroundSource2(bool _end)
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(BackgroundFadeEffect(_end));
    }

    #endregion
    
    
    private void CheckButton_Music()
    {
        if (PlayerPrefs.GetInt("MusicLevel") > .5f)
            musicButton.sprite = musicOn;
        else
            musicButton.sprite = musicOff;

        // musicButton.sprite = SaveDataManager._instance.gameData.musicLevel ? musicOn : musicOff;
    }

    private void CheckButton_SFX()
    {
        if (PlayerPrefs.GetInt("SFXLevel") > .5f)
            sfxButton.sprite = sfxOn;
        else
            sfxButton.sprite = sfxOff;

        // sfxButton.sprite = SaveDataManager._instance.gameData.sfxLevel ? sfxOn : sfxOff;
    }

    public void SetBackgroundMusicVolume()
    {
        isMusicOn = !isMusicOn;
        var _soundLevel = 0.0001f;
        if (isMusicOn)
            _soundLevel = 1;
        mixer.SetFloat("MusicVol", Mathf.Log10(_soundLevel) * 20);

        // SaveDataManager._instance.gameData.musicLevel = isMusicOn;
        // SaveDataManager._instance.SaveData();
        var value = isMusicOn ? 1 : 0;
        PlayerPrefs.SetInt("MusicLevel", value);
        CheckButton_Music();
    }

    public void SetBSFXMusicVolume()
    {
        isSFXOn = !isSFXOn;
        var _soundLevel = 0.0001f;
        if (isSFXOn)
            _soundLevel = 1;
        mixer.SetFloat("SFXVol", Mathf.Log10(_soundLevel) * 20);

        // SaveDataManager._instance.gameData.sfxLevel = isSFXOn;
        // SaveDataManager._instance.SaveData();
        var value = isSFXOn ? 1 : 0;
        PlayerPrefs.SetInt("SFXLevel", value);
        CheckButton_SFX();
    }

    private void CheckSFXValue()
    {
        var _soundLevel = 0.0001f;
        if (isSFXOn)
            _soundLevel = 1;
        mixer.SetFloat("SFXVol", Mathf.Log10(_soundLevel) * 20);
    }

    private void CheckBackgroundValue()
    {
        var _soundLevel = 0.0001f;
        if (isMusicOn)
            _soundLevel = 1;
        mixer.SetFloat("MusicVol", Mathf.Log10(_soundLevel) * 20);
    }

    public enum SFXType
    {
        Jump,Catch,Hit
    }

    public void CallSFX(SFXType sfxType)
    {
        AudioClip clip = null;
        switch (sfxType)
        {
            case SFXType.Catch:
                clip = catchClip;
                break;
            case SFXType.Jump:
                clip = jumpClip;
                break;          
            case SFXType.Hit:
                clip = hitClip;
                break;
        }
        
        sfxSource.PlayOneShot(clip);
    }

    public void CallButtonClick(int clickSound)
    {
        AudioClip clip = null;
        switch (clickSound)
        {
            case 0:
                clip = click_1_Clip;
                break;
            case 1:
                clip = click_2_Clip;
                break;
        }
        
        sfxSource.PlayOneShot(clip);
    }
    
    public void CallButtonClick()
    {
        AudioClip clip = null;
        int tempRand = Random.Range(0, 2);
        switch (tempRand)
        {
            case 0:
                clip = click_1_Clip;
                break;
            case 1:
                clip = click_2_Clip;
                break;
        }
        
        sfxSource.PlayOneShot(clip);
    }

    public void SuccessSound()
    {
        sfxSource.PlayOneShot(success_Clip);
    }

    public void FailSound()
    {
        sfxSource.PlayOneShot(fail_Clip);
    }
    #region Not Used

    public void SetBackgroundMusicVolume(float _soundLevel)
    {
        print(_soundLevel);
        mixer.SetFloat("MusicVol", Mathf.Log10(_soundLevel) * 20);
        CheckButton_Music();
    }

    public void SetBSFXMusicVolume(float _soundLevel)
    {
        print(_soundLevel);
        mixer.SetFloat("SFXVol", Mathf.Log10(_soundLevel) * 20);
        CheckButton_SFX();
    }

    #endregion
}