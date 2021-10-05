using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    [FormerlySerializedAs("backgroundSelection")] [SerializeField] private AudioSource backgroundSelectionSource;
    // [SerializeField] private AudioClip backgroundSelection;
    
    public static BackgroundMusic instance=null;
    
    Scene lastScene;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
    
            DontDestroyOnLoad(this);
            if (!SceneUtils.IsInGameplay())
            {
                if(!backgroundSelectionSource.isPlaying)
                    backgroundSelectionSource.Play();
            }
            else
            {
                backgroundSelectionSource.Stop();
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (lastScene != SceneManager.GetActiveScene())
        {
            if (!SceneUtils.IsInGameplay())
            {
                if(!backgroundSelectionSource.isPlaying)
                    backgroundSelectionSource.Play();
            }
            else
            {
                backgroundSelectionSource.Stop();
            }
            lastScene = SceneManager.GetActiveScene();
        }
    }
}
