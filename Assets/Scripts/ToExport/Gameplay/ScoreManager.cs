using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
   public static ScoreManager _instance;

   public enum ScoreType
   {
       A,B
   }

   public ScoreType scoreType;
   [SerializeField] private TextMeshProUGUI textMesh_Score;
   [SerializeField] private GameObject sheep;
   [SerializeField] private TextMeshPro textMesh_Sheeps;
   
   [SerializeField] private Slider slider;

   private int score = 0;
   private int countSheeps = 0;

   private Coroutine currentCoroutine = null;

   private RectTransform sliderRect;

   private Vector3 maxSize;
   private Vector3 minSize;
   [SerializeField] private float scaleSpeed;

   [SerializeField] private ParticleSystem _particleSystem;
   
   private void Awake()
   {
       if (_instance == null)
           _instance = this;
       else if(_instance!=null)
           Destroy(this.gameObject);

       if (scoreType == ScoreType.B)
           sheep.SetActive(false);

       _particleSystem.Stop();
       
       sliderRect = slider.GetComponent<RectTransform>();
       maxSize = Vector3.one * 1.2f;

       minSize = Vector3.one * .8f;
       UpdateText();
   }

   public void SetMaxSliderValue(int _maxValue)
   {
       float maxSliderVal = ((float) _maxValue) / 3 * 2;
       slider.maxValue = Mathf.Floor(maxSliderVal);
   }

   public void AddScore(int _scoreToAdd)
   {
       // if (currentCoroutine == null)
       //     currentCoroutine = StartCoroutine(GrowUp(_scoreToAdd));
       
       score += _scoreToAdd;
       score = score < 0 ? 0 : score;

       if (_scoreToAdd > 0)
       {
           slider.value += 1;
           _particleSystem.Play();
           if (currentCoroutine == null)
               currentCoroutine = StartCoroutine(GrowUp());
       }
       else
       {
           if (slider.value > 1)
               slider.value -= 1;
           else
               slider.value = 0;
           
           if (currentCoroutine == null)
               currentCoroutine = StartCoroutine(GrowUp(false));

       }
       
       UpdateText();
       if (scoreType == ScoreType.B && _scoreToAdd>=0)
       {
           if(!sheep.activeSelf)
               sheep.SetActive(true);
           
           countSheeps++;
           textMesh_Sheeps.text = "" + countSheeps;
       }
       
   }

   IEnumerator GrowUp(bool _grow = true)
   {
       float time = 0;
       var currentScale = sliderRect.localScale;

       if (_grow)
       {
           while (sliderRect.localScale != maxSize)
           {
               time += Time.deltaTime * scaleSpeed;
               var scale = Vector3.Lerp(currentScale, maxSize, time);
               sliderRect.localScale = scale;
               yield return null;
           }
       
           yield return new WaitForSeconds(.1f);
       
           time = 0f;
           currentScale = sliderRect.localScale;
           while (sliderRect.localScale != Vector3.one)
           {
               time += Time.deltaTime * scaleSpeed;
               var scale = Vector3.Lerp(currentScale, Vector3.one, time);
               sliderRect.localScale = scale;
               yield return null;
           }
       }
       else
       {
           while (sliderRect.localScale != minSize)
           {
               time += Time.deltaTime * scaleSpeed;
               var scale = Vector3.Lerp(currentScale, minSize, time);
               sliderRect.localScale = scale;
               yield return null;
           }
       
           yield return new WaitForSeconds(.1f);
       
           time = 0f;
           currentScale = sliderRect.localScale;
           while (sliderRect.localScale != Vector3.one)
           {
               time += Time.deltaTime * scaleSpeed;
               var scale = Vector3.Lerp(currentScale, Vector3.one, time);
               sliderRect.localScale = scale;
               yield return null;
           }
       }
       
       currentCoroutine = null;
   }

   public float GetMaxValue()
   {
       return slider.maxValue;
   }

   public float GetCurrentValue()
   {
       return slider.value;
   }

   void UpdateText()
   {
       textMesh_Score.text = "" + score;
   }

   public int GetScore()
   {
       return score;
   }
}
