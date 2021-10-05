using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //All this is just for testing

    // [SerializeField] private int level;
    // private void Start()
    // {
    //     SaveDataManager._instance.LoadData();
    //     
    //     int temp = level + 1;
    //     SaveDataManager._instance.GetLevelData("level" + level).currentPoints = 1200;
    //     SaveDataManager._instance.GetLevelData("level" + level).CheckCurrentPoints();
    //     SaveDataManager._instance.GetLevelData("level" + temp).locked = false;
    //     SaveDataManager._instance.SaveData();
    //
    //     Invoke("ToLevelSelection", 3);
    // }

    void ToLevelSelection()
    {
        SceneUtils.ToSelectionScene();
    }
}
