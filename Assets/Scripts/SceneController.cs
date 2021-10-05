using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using INART.SceneTransition;

public class SceneController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Coroutine currentCoroutine = null;

    private bool isFinished = false;

    private void Awake()
    {
        Extensions.SetCanvasGroup(_canvasGroup, 0, false);
    }

    public void Pause()
    {
        if (SceneUtils.IsInGameplay())
        {
            if (currentCoroutine == null)
                currentCoroutine = StartCoroutine(Animate(true));
        }
        else
        {
            Return();
        }
    }

    public void Return()
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CallTransition());
    }

    public void Restart()
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(Animate(false));
    }

    private IEnumerator Animate(bool pause)
    {
        float animation = 0;
        isFinished = true;
        if (pause)
        {
            GameStates._instance.Set_GameState_Pause();

            animation = 0;
            while (animation <= 1)
            {
                Extensions.SetCanvasGroup(_canvasGroup, animation, false);
                animation += Time.deltaTime;
                yield return null;
            }

            Extensions.SetCanvasGroup(_canvasGroup, 1, true);
        }
        else
        {
            animation = 1;
            while (animation >= 0)
            {
                Extensions.SetCanvasGroup(_canvasGroup, animation, false);
                animation -= Time.deltaTime;
                yield return null;
            }

            Extensions.SetCanvasGroup(_canvasGroup, 0, false);
            GameStates._instance.Set_GameState_Play();
        }

        isFinished = false;
        currentCoroutine = null;
    }

    private IEnumerator CallTransition()
    {
        yield return new WaitUntil(() => !isFinished);
        yield return StartCoroutine(SceneTransitionController._instance.FadeEffect_OUT());
        if (SceneUtils.IsInGameplay())
            SceneUtils.ToSelectionScene();
        else
            SceneUtils.ToMainScene();
        currentCoroutine = null;
    }
}