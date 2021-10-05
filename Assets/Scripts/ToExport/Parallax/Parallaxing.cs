using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    public Transform[] backgrounds;
    [SerializeField] private float[] parallaxScales;
    public float smoothing = 1f;
    private Transform camera;
    private Vector3 previousCamPosition;

    private void Awake()
    {
        camera = Camera.main.transform;
    }

    private void Start()
    {
        previousCamPosition = camera.position;

        parallaxScales = new float[backgrounds.Length];

        for (var i = 0; i < backgrounds.Length; i++) parallaxScales[i] = backgrounds[i].position.z;
    }

    private void LateUpdate()
    {
        if (!GameStates._instance.IsGamePlaying())
            return;
        else if (GameStates._instance.IsGamePlaying())
        {
            for (var i = 0; i < backgrounds.Length; i++)
            {
                var parallax = (previousCamPosition.x - camera.position.x) * parallaxScales[i];

                var backgroundTargetPosX = backgrounds[i].position.x - parallax;

                var backgroundTargetPos =
                    new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

                backgrounds[i].position =
                    Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
            }

            previousCamPosition = camera.position;
        }
    }
}