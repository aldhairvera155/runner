using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using INART.SceneTransition;
using UnityEngine.Serialization;


[Serializable]
public class Slides
{
    public Sprite sprite;
    public AudioClip clip;
    public GameObject imageGroup;
}

public class MainSceneManager : MonoBehaviour
{
    [Header("MainStuff")] [SerializeField] private CanvasGroup background_canvasGroup;

    [FormerlySerializedAs("_canvasGroup")] [SerializeField]
    private CanvasGroup popUp_canvasGroup;

    [SerializeField] private CanvasGroup gender_canvasGroup;

    [SerializeField] private List<Slides> slides = new List<Slides>();
    [Header("Buttons")] [SerializeField] private Button continueButton;
    [SerializeField] private Button leftButton, rightButton;
    [SerializeField] private Image mainImage;

    [Header("Audio Source")] [SerializeField]
    private AudioSource source;

    private int index = 0;
    private Coroutine currentCoroutine = null;

    private int tab = 0;

    public void SetTab(int _tab)
    {
        tab = _tab;
    }

    public void Init(bool _gamePlayed = false)
    {
        if (!_gamePlayed)
        {
            // Check();
            // Extensions.SetCanvasGroup(_canvasGroup, 1, true);
            if (index < slides.Count - 1)
            {
                continueButton.gameObject.SetActive(false);
            }
            else
            {
                continueButton.gameObject.SetActive(true);
                continueButton.interactable = true;
            }
            
            for(int i=0;i<slides.Count;i++)
                slides[i].imageGroup.SetActive(false);

            if(!PlayerPrefs.HasKey("Gender"))
                PlayerPrefs.SetInt("Gender", -1);
            // SaveDataManager._instance.gameData.gender = -1;
            // SaveDataManager._instance.SaveData();
            
            Extensions.SetCanvasGroup(background_canvasGroup, 1, true);

            Extensions.SetCanvasGroup(popUp_canvasGroup, 0, false);
            Extensions.SetCanvasGroup(gender_canvasGroup, 0, false);

            // if (currentCoroutine == null)
            //     currentCoroutine = StartCoroutine(Animate(gender_canvasGroup, true));
            
            if (currentCoroutine == null)
                currentCoroutine = StartCoroutine(Animate(popUp_canvasGroup, true,false,true));
            // Extensions.SetCanvasGroup(gender_canvasGroup, 1, true);
        }
        else
        {
            Extensions.SetCanvasGroup(popUp_canvasGroup, 0, false);
            Extensions.SetCanvasGroup(gender_canvasGroup, 0, false);
            Extensions.SetCanvasGroup(background_canvasGroup, 0, false);
        }
    }

    
    public void TurnOff_GenderCanvas()
    {
        // if(SaveDataManager._instance.gameData.gender!=-1)
        if (PlayerPrefs.GetInt("Gender") != -1)
        {
            // if (currentCoroutine == null)
            //     currentCoroutine = StartCoroutine(Animate(gender_canvasGroup, popUp_canvasGroup));

            PlayerPrefs.SetInt("gameHasPlayed", 1);

            if (currentCoroutine == null)
                currentCoroutine = StartCoroutine(Animate(gender_canvasGroup, false, true));
        }

        
        // Extensions.SetCanvasGroup(gender_canvasGroup, 0, false);
    }
    
    /// <summary>
    /// Turns off PopUp Canvas Group
    /// </summary>
    public void TurnOff()
    {
        // SaveDataManager._instance.gameData.gameHasPlayed = true;
        // SaveDataManager._instance.SaveData();

        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(Animate(popUp_canvasGroup, gender_canvasGroup));

        // PlayerPrefs.SetInt("gameHasPlayed", 1);
        //
        // if (currentCoroutine == null)
        //     currentCoroutine = StartCoroutine(Animate(popUp_canvasGroup, false, true));

        // Extensions.SetCanvasGroup(_canvasGroup, 0, false);
    }



    #region Change Slides

    public void CheckButton_Left()
    {
        index--;
        if (index < slides.Count-1)
            slides[index+1].imageGroup.SetActive(false);

        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CallCheck());
    }

    public void CheckButton_Right()
    {
        index++;
        if (index > 0)
            slides[index-1].imageGroup.SetActive(false);
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CallCheck());
    }

    #endregion

    #region Coroutines

    private IEnumerator Animate(CanvasGroup _tempCanvas, bool _turnOn, bool _deactivateCanvasGroup = false, bool start=false)
    {
        float animation = 0;

        if(start)
            yield return StartCoroutine(CallCheck(true));
        
        if (_turnOn)
        {
            while (animation <= 1)
            {
                Extensions.SetCanvasGroup(_tempCanvas, animation, false);
                animation += Time.deltaTime;
                yield return null;
            }

            Extensions.SetCanvasGroup(_tempCanvas, 1, true);
        }
        else
        {
            animation = 1;
            while (animation >= 0)
            {
                Extensions.SetCanvasGroup(_tempCanvas, animation, false);
                animation -= Time.deltaTime;
                yield return null;
            }

            Extensions.SetCanvasGroup(_tempCanvas, 0, false);
        }

        if (_deactivateCanvasGroup)
            Extensions.SetCanvasGroup(background_canvasGroup, 0, false);

        currentCoroutine = null;
    }

    private IEnumerator Animate(CanvasGroup _tempCanvas_A, CanvasGroup _tempCanvas_B)
    {
        float animation = 0;

        // yield return StartCoroutine(CallCheck(true));
        
        animation = 1;
        while (animation >= 0)
        {
            Extensions.SetCanvasGroup(_tempCanvas_A, animation, false);
            animation -= Time.deltaTime;
            yield return null;
        }

        Extensions.SetCanvasGroup(_tempCanvas_A, 0, false);

        CheckActivationButton();

        animation = 0;
        while (animation <= 1)
        {
            Extensions.SetCanvasGroup(_tempCanvas_B, animation, false);
            animation += Time.deltaTime;
            yield return null;
        }

        Extensions.SetCanvasGroup(_tempCanvas_B, 1, true);

        currentCoroutine = null;
    }

    private IEnumerator CallCheck(bool check=false)
    {
        CheckActivationButton();
        Stop();
        RenderImageNSound();

        // yield return new WaitUntil(() => !source.isPlaying);
        ActivateButtons();
        yield return null;
        
        if(!check)
            currentCoroutine = null;
    }

    #endregion

    #region Utils

    public void SetGender(int gender)
    {
        PlayerPrefs.SetInt("Gender", gender);
        // SaveDataManager._instance.gameData.gender = gender;
        // SaveDataManager._instance.SaveData();
    }

    public void NextLevel()
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CallNextLevel());
    }

    private IEnumerator CallNextLevel()
    {
        yield return StartCoroutine(SceneTransitionController._instance.FadeEffect_OUT());
        SceneUtils.NextScene();
        currentCoroutine = null;
    }

    public void NextLevel_A()
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CallLevel(1));
    }

    public void NextLevel_B()
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CallLevel(2));
    }

    public void NextLevel_C()
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CallLevel(3));
    }

    private IEnumerator CallLevel(int _level)
    {
        yield return StartCoroutine(SceneTransitionController._instance.FadeEffect_OUT());

        var level = 1;

        switch (tab)
        {
            case 0:
                level = _level;
                break;

            case 1:
                level = _level + 3;
                break;

            case 2:
                level = _level + 6;
                break;

            case 3:
                level = _level + 9;
                break;
        }

        SceneUtils.LoadScene(level);
        currentCoroutine = null;
    }

    private void Stop()
    {
        leftButton.interactable = false;
        rightButton.interactable = false;
        continueButton.interactable = false;
    }

    private void ActivateButtons()
    {
        if (leftButton.gameObject.activeSelf)
            leftButton.interactable = true;
        if (rightButton.gameObject.activeSelf)
            rightButton.interactable = true;

        if (index < slides.Count - 1)
        {
            continueButton.gameObject.SetActive(false);
        }
        else
        {
            continueButton.gameObject.SetActive(true);
            continueButton.interactable = true;
        }
    }

    private void CheckActivationButton()
    {
        if (index <= 0)
        {
            leftButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(true);
        }
        else if (index > 0 && index < slides.Count - 1)
        {
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(true);
        }
        else
        {
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(false);
        }
    }

    private void RenderImageNSound()
    {
        // mainImage.sprite = slides[index].sprite;
        slides[index].imageGroup.SetActive(true);

        source.clip = slides[index].clip;
        source.Play();
    }

    #endregion
}