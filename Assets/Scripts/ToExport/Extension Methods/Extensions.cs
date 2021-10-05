using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    public static void SetCanvasGroup(CanvasGroup _canvasGroup, float _alpha, bool _isInteractable)
    {
        _canvasGroup.alpha = _alpha;
        _canvasGroup.interactable = _isInteractable;
        _canvasGroup.blocksRaycasts = _isInteractable;
    }
    
    public static void Spawn(string _tag,int _id,Transform _positions)
    {
        var spawnedObject = GameObjectPool.instance.GetPooledObject(_tag, _id);
        if (spawnedObject != null)
        {
            var enemyRange = _positions.transform.position;
            spawnedObject.transform.position = enemyRange;
            spawnedObject.transform.rotation = Quaternion.identity;
            spawnedObject.SetActive(true);
        }
    }
    
    public static void Spawn(string _tag,int _id,Transform _positions,float height)
    {
        var spawnedObject = GameObjectPool.instance.GetPooledObject(_tag, _id);
        if (spawnedObject != null)
        {
            var enemyRange = new Vector3(_positions.transform.position.x, height, _positions.transform.position.z);
            spawnedObject.transform.position = enemyRange;
            spawnedObject.transform.rotation = Quaternion.identity;
            spawnedObject.SetActive(true);
        }
    }
}
