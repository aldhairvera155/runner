using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStates : MonoBehaviour
{
    public static GameStates _instance;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public enum GameState
    {
        LOAD,
        PLAY,
        PAUSE,
        PRE_END,
        END
    }

    public GameState _gameStates;

    public void Set_GameState_Load()
    {
        _gameStates = GameState.LOAD;
    }

    public void Set_GameState_Play()
    {
        _gameStates = GameState.PLAY;
    }

    public void Set_GameState_Pause()
    {
        _gameStates = GameState.PAUSE;
    }

    public void Set_GameState_Pre_End()
    {
        _gameStates = GameState.PRE_END;
    }
    
    public void Set_GameState_End()
    {
        _gameStates = GameState.END;
    }

    public GameState GetGameState()
    {
        return _gameStates;
    }

    public bool IsGameLoading()
    {
        return _gameStates == GameState.LOAD;
    }

    public bool IsGameEnd()
    {
        return _gameStates == GameState.END;
    }
    
    public bool IsGamePlaying()
    {
        return _gameStates == GameState.PLAY ? true : false;
    }
    
    
    public bool IsGamePre_End()
    {
        return _gameStates == GameState.PRE_END ? true : false;
    }
}