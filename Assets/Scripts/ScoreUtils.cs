using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelStarScore
{
    // public int levelNumber;
    [SerializeField] int star1_Points;
    [SerializeField] int star2_Points;
    [SerializeField] int star3_Points;

    public int Star1Points => star1_Points;
    public int Star2Points => star2_Points;
    public int Star3Points => star3_Points;
    
    public void CheckCurrentPoints(int levelNumber)
    {
        int maxPoints = PlayerPrefs.GetInt(levelNumber + "_maxPoints");
        int currentPoints = PlayerPrefs.GetInt(levelNumber + "_currentPoints");

        int starNumber = PlayerPrefs.GetInt(levelNumber+"_starNumber");
        int localStarNumber = PlayerPrefs.GetInt(levelNumber+"_localStarNumber");
        
        if (maxPoints <= currentPoints)
            maxPoints = currentPoints;
        
        //Global star number
        if (maxPoints < star1_Points)
            starNumber = 0;
        else if (maxPoints >= star1_Points && maxPoints < star2_Points)
        {
            if(starNumber<=1)
                starNumber = 1;
        }
        else if (maxPoints >= star2_Points && maxPoints < star3_Points)
        {
            if(starNumber<=2)
                starNumber = 2;
        }
        else if (maxPoints >= star3_Points)
        {
            if(starNumber<=3)
                starNumber = 3;
        }

        PlayerPrefs.SetInt(levelNumber + "_starNumber", starNumber);
        
        //Local star Number
        if (currentPoints < star1_Points)
            localStarNumber = 0;
        else if (currentPoints >= star1_Points && currentPoints < star2_Points)
        {
            if(localStarNumber<=1)
                localStarNumber = 1;
        }
        else if (currentPoints >= star2_Points && currentPoints < star3_Points)
        {
            if(localStarNumber<=2)
                localStarNumber = 2;
        }
        else if (currentPoints >= star3_Points)
        {
            if(localStarNumber<=3)
                localStarNumber = 3;
        }
        
        PlayerPrefs.SetInt(levelNumber+"_localStarNumber",localStarNumber);
        PlayerPrefs.Save();
    }
}

public class ScoreUtils : MonoBehaviour
{
    public static ScoreUtils _instance;

    public List<LevelStarScore> levelLimitants = new List<LevelStarScore>();
    
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);
    }
}
