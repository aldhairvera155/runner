using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class HeightLimitant
{
    public float bottomLimit;
    public float topLimit;
}

public class Spawner : MonoBehaviour
{
    [SerializeField] private float maxTime;
    private float timer = 0;
    [SerializeField] private List<string> tags = new List<string>();

    [SerializeField] private List<Collectable_Identifier> collectables = new List<Collectable_Identifier>();
    [SerializeField] private List<Collectable_Identifier> obstacles = new List<Collectable_Identifier>();

    [SerializeField] private HeightLimitant collectable_Height_0;
    [SerializeField] private HeightLimitant collectable_Height_1;
    [SerializeField] private HeightLimitant collectable_Height_2;

    [SerializeField] private float midPoint0 = -3f, midPoint1 = -1.5f, midPoint2 = 0;
    
    private int index = 0;

    private bool end = false;
    
    private void Awake()
    {
        tags.Add(StringUtils.tag_Collectable);
        tags.Add(StringUtils.tag_Obstacle);
    }

    private void Start()
    {
        ScoreManager._instance.SetMaxSliderValue(collectables.Count);
        
        if (!end)
        {
            //Spawn Collectables
            for (int i = 0; i < collectables.Count; i++)
            {
                // Extensions.Spawn(tags[0],Random.Range(0,3),collectables[i].transform);
                if(collectables[i].ID<=2)
                    Extensions.Spawn(tags[0],collectables[i].ID,collectables[i].transform,GetHeight(collectables[i].ID));
                else
                    Extensions.Spawn(tags[0],collectables[i].ID,collectables[i].transform);
            }
                
            //Spawn Obstacles
            for (int i = 0; i < obstacles.Count; i++)
            {
                // Extensions.Spawn(tags[1],Random.Range(0,3),obstacles[i].transform);
                Extensions.Spawn(tags[1],obstacles[i].ID,obstacles[i].transform);
            }

            end = true;
        }
    }

    float GetHeight(int tagId)
    {
        float height = 0;

        switch (tagId)
        {
            case 0:
                height = Random.Range(collectable_Height_0.bottomLimit, collectable_Height_0.topLimit);
                break;
            case 1:
                height = Random.Range(collectable_Height_1.bottomLimit, collectable_Height_1.topLimit);
                break;
            case 2:
                height = Random.Range(collectable_Height_2.bottomLimit, collectable_Height_2.topLimit);
                break;
        }

        return height;
    }
    
    // void Update()
    // {
    //     if (GameStates._instance.IsGamePlaying())
    //     {
    //         if (!end)
    //         {
    //             //Spawn Collectables
    //             for (int i = 0; i < collectables.Count; i++)
    //             {
    //                 // Extensions.Spawn(tags[0],Random.Range(0,3),collectables[i].transform);
    //                 Extensions.Spawn(tags[0],collectables[i].ID,collectables[i].transform);
    //             }
    //             
    //             //Spawn Obstacles
    //             for (int i = 0; i < obstacles.Count; i++)
    //             {
    //                 // Extensions.Spawn(tags[1],Random.Range(0,3),obstacles[i].transform);
    //                 Extensions.Spawn(tags[1],obstacles[i].ID,obstacles[i].transform);
    //             }
    //
    //             end = true;
    //         }
    //
    //         // if (!end)
    //         // {
    //         //     timer += Time.deltaTime;
    //         //     if (timer >= maxTime)
    //         //     {
    //         //         if (index < collectables.Count - 1)
    //         //         {
    //         //             Extensions.Spawn(tags[0],Random.Range(0,3),collectables[index]);
    //         //             index++;
    //         //             timer = 0;
    //         //         }
    //         //         else
    //         //         {
    //         //             end = true;
    //         //         }
    //         //     }
    //         // }
    //     }
    // }

    private void OnDrawGizmos()
    {
        string name = "1.png";
        // Gizmos.color=
        for (int i = 0; i < collectables.Count; i++)
        {
            float temp = 0;
            switch (collectables[i].ID)
            {
                case 0:
                    name = "1.png";
                    temp = midPoint0;
                    collectables[i].transform.position=new Vector2(collectables[i].transform.position.x,temp);
                    break;
                case 1:
                    name = "2.png";
                    temp = midPoint1;
                    collectables[i].transform.position=new Vector2(collectables[i].transform.position.x,temp);
                    break;
                case 2:
                    name = "3.png";
                    temp = midPoint2;
                    collectables[i].transform.position=new Vector2(collectables[i].transform.position.x,temp);
                    break;
                case 3:
                    name = "4.png";
                    break;
                case 4:
                    name = "5.png";
                    break;

            }
            Gizmos.DrawIcon(collectables[i].transform.position, name, true);
        }
        
        for (int i = 0; i < obstacles.Count; i++)
        {
            switch (obstacles[i].ID)
            {
                case 0:
                    name = "No_1.png";
                    break;
                case 1:
                    name = "No_2.png";
                    break;
                case 2:
                    name = "No_3.png";
                    break;
                case 3:
                    name = "No_4.png";
                    break;
            }
            
            // print(obstacles[i].name + " - " + obstacles[i].ID);

            Gizmos.DrawIcon(obstacles[i].transform.position, name, true);
        }

    }
}