using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    [SerializeField] protected int points;
    
    public enum CollectableType
    {
        Collectable,Obstacle
    }

    public CollectableType _collectableType;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Add Points
            ScoreManager._instance.AddScore(points);

            if (_collectableType == CollectableType.Obstacle)
            {
                // other.GetComponent<PlayerController>().ApplyForce(this.transform.position);
                other.GetComponent<PlayerController>().BlinkEffect();
                AudioMixerManager._instance.CallSFX(AudioMixerManager.SFXType.Hit);
            }
            else
            {
                AudioMixerManager._instance.CallSFX(AudioMixerManager.SFXType.Catch);
            }
            
            other.GetComponent<PlayerController>().RunParticleEffect(_collectableType);
                        
            
            //Turn off and send to 10000, 10000
            this.transform.position = -Vector3.one * 500;
            this.gameObject.SetActive(false);
            // GetComponent<Collider2D>().enabled = false;
        }
    }
}
