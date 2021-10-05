using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeSaveData : MonoBehaviour
{
    [SerializeField] private PlayerPrefsSaveData _playerPrefsSaveData;
    
    void Start()
    {
        // if (PlayerPrefs.HasKey("HasSavedBefore"))
        // {
        //     // SaveDataManager._instance.LoadData();
        // }
        // else
        if(!PlayerPrefs.HasKey("HasSavedBefore"))
        {
            PlayerPrefs.SetInt("HasSavedBefore", 1);
            _playerPrefsSaveData.Init();
            // SaveDataManager._instance.SaveData();
            PlayerPrefs.Save();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            ResetWholeGame();
    }

    public void ResetWholeGame()
    {
        PlayerPrefs.DeleteAll();
        // SaveDataManager._instance.SaveData();
        SceneManager.LoadScene("MainScreen");
    }
}
