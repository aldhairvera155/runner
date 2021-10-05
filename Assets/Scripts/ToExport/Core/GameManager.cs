using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using INART.SceneTransition;


public class GameManager : MonoBehaviour
{
    private Coroutine currentCoroutine = null;

    private void Awake()
    {
        //We set the framerate of the game
        QualitySettings.vSyncCount = 1; // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        // SaveDataManager._instance.LoadData();
    }

    void Start()
    {
        //We load everything
        GameStates._instance.Set_GameState_Load();
        
        if (currentCoroutine == null) currentCoroutine = StartCoroutine(StartGame(1f));
    }

    private IEnumerator StartGame(float _delayDuration)
    {
        //Fade In
        yield return StartCoroutine(SceneTransitionController._instance.FadeEffect_IN());
        
        yield return new WaitForSeconds(_delayDuration);

        GameStates._instance.Set_GameState_Play();
        currentCoroutine = null;
    }
}
