using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using INART.SceneTransition;


[Serializable]
public class LevelInfo
{
    public Button button;
    public TextMeshProUGUI textmesh;
    public Image[] Images = new Image[3];
}

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private List<LevelInfo> _levelInfo = new List<LevelInfo>();
    [SerializeField] private Sprite activated;
    [SerializeField] private Sprite deActivated;
    [SerializeField] private MainSceneManager _mainSceneManager;
    private Coroutine currentCoroutine;

    [SerializeField] private Image backgroundImage;
    [SerializeField] List<Sprite> sprites = new List<Sprite>();

    private int toggleIndex = 0;
    
    private void Start()
    {
        bool isGamePlayed = PlayerPrefs.GetInt("gameHasPlayed") == 0 ? false : true;
        // _mainSceneManager.Init(SaveDataManager._instance.GetGamePlayed());
        _mainSceneManager.Init(isGamePlayed);
        // _mainSceneManager.Init();

        if (currentCoroutine == null)
            currentCoroutine=StartCoroutine(FadeEffect());

        
        for (var i = 0; i < 3; i++)
        for (var j = 0; j < 3; j++)
            _levelInfo[i].Images[j].gameObject.SetActive(false);

        if (!PlayerPrefs.HasKey("ToggleIndex"))
        {
            toggleIndex = 0;
            PlayerPrefs.SetInt("ToggleIndex", toggleIndex);
            PlayerPrefs.Save();
        }
        else
        {
            toggleIndex = PlayerPrefs.GetInt("ToggleIndex");
        }
        
        ChangeToggle(toggleIndex);
    }

    public void ChangeToggle(int index)
    {
        _mainSceneManager.SetTab(index);
        switch (index)
        {
            case 0:
                for (var i = 0; i < 3; i++)
                {
                    var factor = i + 1;
                    ResetStars(_levelInfo[i].Images);
                    SetButton(_levelInfo[i].button, _levelInfo[i].textmesh, factor);
                    Checker(_levelInfo[i].Images, factor);
                    SetStars(_levelInfo[i].Images, StarPrefsParser(factor));
                }

                break;

            case 1:
                for (var i = 0; i < 3; i++)
                {
                    var factor = i + 4;
                    ResetStars(_levelInfo[i].Images);
                    SetButton(_levelInfo[i].button, _levelInfo[i].textmesh, factor);
                    Checker(_levelInfo[i].Images, factor);
                    SetStars(_levelInfo[i].Images, StarPrefsParser(factor));
                }

                break;

            case 2:
                for (var i = 0; i < 3; i++)
                {
                    var factor = i + 7;
                    ResetStars(_levelInfo[i].Images);
                    SetButton(_levelInfo[i].button, _levelInfo[i].textmesh, factor);
                    Checker(_levelInfo[i].Images, factor);
                    SetStars(_levelInfo[i].Images, StarPrefsParser(factor));
                }

                break;

            case 3:
                for (var i = 0; i < 3; i++)
                {
                    var factor = i + 10;
                    SetButton(_levelInfo[i].button, _levelInfo[i].textmesh, factor);
                    Checker(_levelInfo[i].Images, factor);
                    SetStars(_levelInfo[i].Images, StarPrefsParser(factor));
                }

                break;
        }
        PlayerPrefs.SetInt("ToggleIndex", index);


        backgroundImage.sprite = sprites[index];
    }

    #region Utils

    IEnumerator FadeEffect()
    {
        yield return SceneTransitionController._instance.FadeEffect_IN();
        currentCoroutine = null;
    }

    //Parse data, maybe would not be needed later
    private bool PlayerPrefsParser(int level)
    {
        var temp = level;
        return PlayerPrefs.GetInt(temp+"_locked") != 0;
        // return !SaveDataManager._instance.GetLevelData("level" + level).locked;
    }

    //Parse stars per level, maybe would be needed later
    private int StarPrefsParser(int level)
    {
        return PlayerPrefs.GetInt(level+"_starNumber");
        // return SaveDataManager._instance.GetLevelData("level" + level).starNumber;
    }

    private void Checker(Image[] _images, int _index)
    {
        // if (SaveDataManager._instance.GetLevelData("level" + _index).hasBeenPlayed)
        if (PlayerPrefs.GetInt(_index+"_hasBeenPlayed")==1)
        {
            if (PlayerPrefsParser(_index))
                for (var i = 0; i < 3; i++)
                    _images[i].gameObject.SetActive(true);
            else
                for (var i = 0; i < 3; i++)
                    _images[i].gameObject.SetActive(false);
        }
        else
        {
            for (var i = 0; i < 3; i++) _images[i].gameObject.SetActive(false);
        }
    }

    private void SetButton(Button _button, TextMeshProUGUI _textMesh, int _index)
    {
        _button.interactable = PlayerPrefsParser(_index);
        _textMesh.text = "" + _index;
    }

    private void SetStars(Image[] _images, int _starNumber)
    {
        for (var i = 0; i < _starNumber; i++) _images[i].sprite = activated;
    }
    
    void ResetStars(Image[] _images){
        for (var i = 0; i < _images.Length; i++) _images[i].sprite = deActivated;
    }

    #endregion
}