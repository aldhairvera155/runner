using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using INART.SceneTransition;

[Serializable]
public class ButtonActivated
{
    public List<Button> buttons = new List<Button>();
    public List<bool> activated = new List<bool>();

    public List<Image> checkImage = new List<Image>();

    public void Init()
    {
        // for(int i=0;i<buttons.Count;i++)
        //     activated.Add(true);

        for (var i = 0; i < buttons.Count; i++)
            buttons[i].interactable = false;
    }

    public void SetSprite(int _option, Sprite _sprite)
    {
        checkImage[_option].sprite = _sprite;
    }
    
    public void Deactivate(int _option,Sprite _sprite)
    {
        buttons[_option].interactable = false;
        checkImage[_option].sprite = _sprite;
        // activated[_option] = false;
    }

    public void Deactivate_Color(int _option, Color _tempColor)
    {
        var tempAlpha = buttons[_option].image.color.a;
        buttons[_option].image.color = new Color(_tempColor.r, _tempColor.g, _tempColor.b, tempAlpha);
    }

    public void Reactivate(Color _tempColor)
    {
        for (var i = 0; i < buttons.Count; i++)
            if (activated[i])
            {
                buttons[i].interactable = true;
                var tempAlpha = buttons[i].image.color.a;
                buttons[i].image.color = new Color(_tempColor.r, _tempColor.g, _tempColor.b, tempAlpha);
            }
    }

    public void Reactivate()
    {
        for (var i = 0; i < buttons.Count; i++)
            if (activated[i])
                buttons[i].interactable = true;
    }

    public void Deactivate_Everything()
    {
        for (var i = 0; i < buttons.Count; i++)
            buttons[i].interactable = false;
    }
}

[Serializable]
public class Trivia
{
    // public string question;
    //
    // public TextMeshProUGUI questionText;
    [FormerlySerializedAs("correctOption")]
    public int correctOption_Sun;

    public ButtonActivated sunButtons;
    // public int correctOption_Water;
    // public ButtonActivated waterButtons;

    public void Initialize()
    {
        // questionText.text = question;

        sunButtons.Init();
        // waterButtons.Init();
    }
}

public enum InitialState
{
    TRIVIA,
    MESSAGE,
    SCORE
}

public class EndMessage : MonoBehaviour
{
    public static EndMessage _instance;

    [SerializeField] private InitialState initialState;
    public static int _level;
    [SerializeField] private int level;

    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Score Stuff")] [SerializeField]
    private CanvasGroup score_canvasGroup;

    [SerializeField] private Button scoreButton;

    [SerializeField] private Sprite message0;
    [SerializeField] private Sprite message1;
    [SerializeField] private Sprite message2;
    [SerializeField] private Sprite message3;
    [SerializeField] private Image greetingMessage;

    [Header("Stars")] [SerializeField] private Sprite turnedOff;
    [SerializeField] private Sprite turnedOn;
    [SerializeField] private Image[] images = new Image[3];

    [Header("Message Stuff")] [SerializeField]
    private CanvasGroup message_canvasGroup;

    [SerializeField] private TextMeshProUGUI messageTitleText;
    [SerializeField] private Button messageButton;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private string message;

    [Header("Trivia stuff")] [SerializeField]
    private CanvasGroup trivia_canvasGroup;

    [SerializeField] private CanvasGroup triviaMessage_canvasGroup;
    [SerializeField] private Trivia _trivia;

    [Header("Trivia stuff")] 
    // [SerializeField] private Color backToNormal_Color;
    // [SerializeField] private Color marked_Color;
    [SerializeField] private Sprite _yesSprite;
    [SerializeField] private Sprite _noSprite;

    private Coroutine currentCoroutine;

    [Header("AudioClips")] [SerializeField]
    private AudioClip scoreClip;

    [SerializeField] private AudioClip messageClip;
    [SerializeField] private AudioClip triviaClip;

    [SerializeField] private AudioSource voiceSource;

    [SerializeField] private ParticleSystem rainParticles;

    private int sunAnswer, waterAnswer;

    
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != null)
            Destroy(gameObject);

        _level = level;
        scoreButton.interactable = false;
        messageButton.interactable = false;

        Extensions.SetCanvasGroup(score_canvasGroup, 0, false);
        Extensions.SetCanvasGroup(message_canvasGroup, 0, false);
        Extensions.SetCanvasGroup(trivia_canvasGroup, 0, false);
        Extensions.SetCanvasGroup(triviaMessage_canvasGroup, 0, false);

        if (initialState == InitialState.TRIVIA)
            Extensions.SetCanvasGroup(triviaMessage_canvasGroup, 0, false);

        _trivia.Initialize();

        levelText.text = "Nivel " + level;
        messageTitleText.text = "Nivel " + level;
        for (var i = 0; i < 3; i++) images[i].sprite = turnedOff;
        rainParticles.Stop();
    }

    public void EndRain()
    {
        // var temp = level + 1;
        // SaveDataManager._instance.GetLevelData("level" + level).currentPoints = ScoreManager._instance.GetScore();
        // SaveDataManager._instance.GetLevelData("level" + level).CheckCurrentPoints();
        // SaveDataManager._instance.GetLevelData("level" + level).hasBeenPlayed = true;
        // var currentStars = SaveDataManager._instance.GetLevelData("level" + level).localStarNumber;

        PlayerPrefs.SetInt(level + "_currentPoints", ScoreManager._instance.GetScore());
        PlayerPrefs.SetInt(level + "_hasBeenPlayed", 1);
        ScoreUtils._instance.levelLimitants[level - 1].CheckCurrentPoints(level);

        PlayerPrefs.Save();
        var currentStars = PlayerPrefs.GetInt(level + "_localStarNumber");

        // print("Call 1");

        if (currentStars > 0)
            rainParticles.Play();
    }

    /// <summary>
    /// When collide with the endpoint, call all the messages
    /// </summary>
    public void AnimateAndSet()
    {
        // print("Call _Animate");

        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(MessageAnimation());
    }

    #region Trivia Stuff

    public void CheckAnswer(int option)
    {
        sunAnswer = option;

        if (sunAnswer == _trivia.correctOption_Sun)
        {
            AudioMixerManager._instance.SuccessSound();
            // _trivia.sunButtons.Deactivate_Color(option, backToNormal_Color);
            _trivia.sunButtons.SetSprite(option,_yesSprite);

            if (currentCoroutine == null)
                currentCoroutine = StartCoroutine(AnimateCanvasGroup(triviaMessage_canvasGroup, trivia_canvasGroup));
        }
        else
        {
            AudioMixerManager._instance.FailSound();
            // _trivia.sunButtons.Deactivate_Color(option, marked_Color);
            _trivia.sunButtons.Deactivate(option,_noSprite);
        }
    }


    /// <summary>
    /// Check answer
    /// To improve, do some more stuff in the CallMessageTrivia Coroutine
    /// </summary>
    /// <param name="option">Number of the option</param>
    /// 
    // public void CheckAnswer_Sun(int option)
    // {
    //     sunAnswer = option;
    //     _trivia.sunButtons.Deactivate_Color(option,marked_Color);
    //     _trivia.sunButtons.Deactivate_Everything();
    //     _trivia.waterButtons.Reactivate(backToNormal_Color);
    //     AudioMixerManager._instance.CallButtonClick();
    // }
    //
    // public void CheckAnswer_Water(int option)
    // {
    //     waterAnswer = option;
    //     if (waterAnswer == _trivia.correctOption_Water && sunAnswer==_trivia.correctOption_Sun)
    //     {
    //         _trivia.waterButtons.Deactivate_Color(option,marked_Color);
    //         _trivia.waterButtons.Deactivate_Everything();
    //         AudioMixerManager._instance.SuccessSound();
    //
    //         if (currentCoroutine == null)
    //             currentCoroutine = StartCoroutine(AnimateCanvasGroup(triviaMessage_canvasGroup,trivia_canvasGroup));
    //     }
    //     else
    //     {
    //         AudioMixerManager._instance.FailSound();
    //
    //         _trivia.waterButtons.Deactivate_Color(option,marked_Color);
    //         _trivia.waterButtons.Deactivate_Everything();
    //         _trivia.sunButtons.Reactivate(backToNormal_Color);
    //     }
    // }
    public void MessageOK()
    {
        // print("Call me");
        Extensions.SetCanvasGroup(triviaMessage_canvasGroup, 0, false);

        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CallInitialAction(InitialState.SCORE));
    }

    #endregion


    /// <summary>
    ///
    /// SCORE - FOR FIRST LEVEL OF THE SCENES
    /// 
    /// Turns off the Message CanvasGroup and Turns on Score CanvasGroup
    /// </summary>
    public void TurnOff_MessageCanvasGroup()
    {
        // print("Turn Off Message Canvas");
        Extensions.SetCanvasGroup(message_canvasGroup, 0, false);
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CallInitialAction(InitialState.SCORE));
    }

    /// <summary>
    ///
    /// SCORE
    /// 
    /// Sends to the level selection
    /// To Improve, Call a coroutine adding a transition
    /// </summary>
    public void ToLevelSelection()
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CallTransition());
    }

    #region Coroutines

    private IEnumerator CallTransition()
    {
        yield return StartCoroutine(SceneTransitionController._instance.FadeEffect_OUT());
        SceneUtils.ToSelectionScene();
        currentCoroutine = null;
    }

    private IEnumerator MessageAnimation()
    {
        var temp = level + 1;
        // SaveDataManager._instance.GetLevelData("level" + level).currentPoints = ScoreManager._instance.GetScore();
        // SaveDataManager._instance.GetLevelData("level" + level).CheckCurrentPoints();
        // SaveDataManager._instance.GetLevelData("level" + level).hasBeenPlayed = true;
        // var currentStars = SaveDataManager._instance.GetLevelData("level" + level).localStarNumber;
        var currentStars = PlayerPrefs.GetInt(level + "_localStarNumber");

        // if(currentStars>0 && temp<SaveDataManager._instance.GetLevelData("level" + 12).levelNumber)
        if (currentStars > 0 && temp <= 12)
            PlayerPrefs.SetInt(temp + "_locked", 1);
        // SaveDataManager._instance.GetLevelData("level" + temp).locked = false;

        PlayerPrefs.Save();
        // SaveDataManager._instance.SaveData();

        switch (currentStars)
        {
            case 0:
                greetingMessage.sprite = message0;
                break;
            case 1:
                greetingMessage.sprite = message1;
                break;
            case 2:
                greetingMessage.sprite = message2;
                break;
            case 3:
                greetingMessage.sprite = message3;
                break;
        }

        greetingMessage.SetNativeSize();

        for (var i = 0; i < currentStars; i++)
            images[i].sprite = turnedOn;

        yield return null;

        if (initialState == InitialState.TRIVIA)
        {
            // _trivia.Initialize();
            yield return StartCoroutine(CallInitialAction(InitialState.TRIVIA));
        }
        else
        {
            if (initialState == InitialState.MESSAGE)
                yield return StartCoroutine(CallInitialAction(InitialState.MESSAGE));
            else
                yield return StartCoroutine(CallInitialAction(InitialState.SCORE));
        }

        // currentCoroutine = null;
    }

    private IEnumerator AnimateCanvasGroup(CanvasGroup canvas)
    {
        float animation = 0;
        while (animation <= 1)
        {
            Extensions.SetCanvasGroup(canvas, animation, false);
            animation += Time.deltaTime * 2;
            yield return null;
        }

        Extensions.SetCanvasGroup(canvas, 1, true);
        currentCoroutine = null;
    }

    private IEnumerator AnimateCanvasGroup(CanvasGroup turnOn_canvas, CanvasGroup turnOff_canvas)
    {
        yield return new WaitForSeconds(1f);
        float animation = 0;
        while (animation <= 1)
        {
            Extensions.SetCanvasGroup(turnOn_canvas, animation, false);
            Extensions.SetCanvasGroup(turnOff_canvas, 1 - animation, false);

            animation += Time.deltaTime * 2;

            yield return null;
        }

        Extensions.SetCanvasGroup(turnOff_canvas, 0, false);
        Extensions.SetCanvasGroup(turnOn_canvas, 1, true);

        currentCoroutine = null;
    }


    private IEnumerator CallInitialAction(InitialState _initialState)
    {
        float animation = 0;

        CanvasGroup tempCanvas = null;
        AudioClip tempClip = null;

        switch (_initialState)
        {
            case InitialState.TRIVIA:
                tempCanvas = trivia_canvasGroup;
                tempClip = triviaClip;
                break;
            case InitialState.MESSAGE:
                tempCanvas = message_canvasGroup;
                messageText.text = message;
                tempClip = messageClip;
                break;
            case InitialState.SCORE:
                tempCanvas = score_canvasGroup;
                tempClip = scoreClip;
                break;
        }

        while (animation <= 1)
        {
            Extensions.SetCanvasGroup(tempCanvas, animation, false);
            animation += Time.deltaTime;
            yield return null;
        }

        Extensions.SetCanvasGroup(tempCanvas, 1, true);

        voiceSource.clip = tempClip;
        voiceSource.Play();
        yield return new WaitUntil(() => !voiceSource.isPlaying);
        messageButton.interactable = true;

        switch (_initialState)
        {
            case InitialState.TRIVIA:
                _trivia.sunButtons.Reactivate();
                break;
            case InitialState.MESSAGE:
                messageButton.interactable = true;
                break;
            case InitialState.SCORE:
                scoreButton.interactable = true;
                break;
        }

        // print("Calling initial Action");

        currentCoroutine = null;
    }

    #endregion
}