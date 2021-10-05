using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RunnerLevelPrefs
{
    public void Init(int _levelNumber)
    {
        PlayerPrefs.SetInt(_levelNumber+"_levelNumber",_levelNumber);
        
        //0 == false - 1 == true
        PlayerPrefs.SetInt(_levelNumber+"_hasBeenPlayed",0);
        PlayerPrefs.SetInt(_levelNumber+"_locked",0);

        PlayerPrefs.SetInt(_levelNumber + "_starNumber", 0);
        PlayerPrefs.SetInt(_levelNumber+"_localStarNumber",0);
        
        PlayerPrefs.SetInt(_levelNumber + "_currentPoints", 0);
        PlayerPrefs.SetInt(_levelNumber+"_maxPoints",0);
        
        PlayerPrefs.Save();
    }

    public void InitFirst(int _levelNumber)
    {
        PlayerPrefs.SetInt(_levelNumber+"_locked",1);
        PlayerPrefs.Save();
    }
}

public class PlayerPrefsSaveData : MonoBehaviour
{
    [SerializeField] List<RunnerLevelPrefs> _levelPrefs=new List<RunnerLevelPrefs>();

    public void Init()
    {
        if (!PlayerPrefs.HasKey("gameHasPlayed"))
        {
            for (int i = 0; i < _levelPrefs.Count; i++)
            {
                int levelNumber = i + 1;
                _levelPrefs[i].Init(levelNumber);
            }

            _levelPrefs[0].InitFirst(1);
            
            PlayerPrefs.SetInt("gameHasPlayed", 0);
            PlayerPrefs.SetInt("MusicLevel", 1);
            PlayerPrefs.SetInt("SFXLevel", 1);
            PlayerPrefs.SetInt("Gender", -1);
            
            PlayerPrefs.Save();
        }
    }
}
