using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform virtualCamera;

    private Vector3 lastCameraPosition;
    [SerializeField] private Vector2 parallaxEffectMultiplier;
    [SerializeField] private float textureUnitSizeX;

    private void Start()
    {
        virtualCamera = Camera.main.transform;

        lastCameraPosition = virtualCamera.position;
        var sprite = GetComponent<SpriteRenderer>().sprite;
        var texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    }

    private void Update()
    {
        var deltaMovement = virtualCamera.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x,
            deltaMovement.y * parallaxEffectMultiplier.y);
        lastCameraPosition = virtualCamera.position;

        if (Mathf.Abs(virtualCamera.position.x - transform.position.x) >= textureUnitSizeX)
        {
            var offSetPositionX = (virtualCamera.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(virtualCamera.position.x + offSetPositionX, transform.position.y);
        }
    }
}