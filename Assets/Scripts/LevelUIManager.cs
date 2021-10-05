using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    [SerializeField] private Transform endPosition;
    [SerializeField] private Transform player;
    private Vector2 initialPosition;
    
    [SerializeField] private RectTransform Ui_Path;
    [SerializeField] private Image fillImg;
    
    [SerializeField] private float currentPos;

    [Header("Player Head Indicator")]
    // [SerializeField] private RectTransform characterPosition;

    [SerializeField] private float range;

    [SerializeField] private Sprite boy, girl;
    
    void Start()
    {
        // if (PlayerPrefs.GetInt("Gender") == 0 || !PlayerPrefs.HasKey("Gender"))
        //     characterPosition.GetComponent<Image>().sprite = boy;
        // else
        //     characterPosition.GetComponent<Image>().sprite = girl;
        initialPosition = player.position;
        fillImg.fillAmount = 0;
        Update_UIMovement();
    }
    
    void Update()
    {
        Update_UIMovement();
    }
    
    private void Update_UIMovement()
    {
        currentPos = player.position.x / endPosition.position.x;
        // characterPosition.localPosition=new Vector3(Mathf.Lerp(-range,range,currentPos),0,0);
        fillImg.fillAmount = Mathf.InverseLerp(initialPosition.x,endPosition.position.x,player.position.x);
    }
}
