using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProperties {
    public enum ObjectType {
        Collectable,
        Particles,
        Obstacle
    }
}

[System.Serializable]
public class ObjectPoolItem {
    public int amountToPool;
    public GameObject objectToPool;
    public ObjectProperties.ObjectType objectType;
    public int objectID;
    public bool shouldExpand;
}

public class GameObjectPool : MonoBehaviour
{
        public static GameObjectPool instance;

        // private const string tag_Collectable = "Collectable";
        // private const string tag_Particles = "Particles";
        // private const string tag_Obstacle = "Obstacle";

        //const string tag_Bullet = "Bullet";

        [SerializeField] private List<ObjectPoolItem> itemsToPool = new List<ObjectPoolItem> ();
        [SerializeField] private List<GameObject> pooledObjects = new List<GameObject> ();

        private void Awake () {
            if (instance == null)
                instance = this;
            CreateElementsInPool_Complex ();
        }

        private void CreateElementsInPool_Complex () {
            pooledObjects = new List<GameObject> ();
            var particleCounter = 0;
            var powerUpCounter = 0;
            // int bulletCounter = 0;
            var obstacleCounter = 0;

            var isDifferent = false;

            foreach (var item in itemsToPool)
                for (var i = 0; i < item.amountToPool; i++) {
                    SetTag (item);

                    if (!isDifferent) {
                        switch (item.objectType) {
                            case ObjectProperties.ObjectType.Collectable:
                                item.objectID = powerUpCounter;
                                item.objectToPool.GetComponent<ObjectInGame> ().objectID = item.objectID;
                                powerUpCounter++;
                                break;
                            case ObjectProperties.ObjectType.Particles:
                                item.objectID = particleCounter;
                                item.objectToPool.GetComponent<ObjectInGame> ().objectID = item.objectID;
                                particleCounter++;
                                break;
                            case ObjectProperties.ObjectType.Obstacle:
                                item.objectID = obstacleCounter;
                                item.objectToPool.GetComponent<ObjectInGame> ().objectID = item.objectID;
                                obstacleCounter++;
                                break;
                        }
                        isDifferent = true;
                    }
                    
                    var obj = (GameObject) Instantiate (item.objectToPool);
                    obj.SetActive (false);
                    pooledObjects.Add (obj);

                    if (i == item.amountToPool - 1) isDifferent = false;
                }
        }

        private void SetID(ObjectPoolItem _item, int _counter)
        {
            _item.objectID = _counter;
            _counter++;
        }

        public GameObject GetPooledObject (string tag) {
            for (var i = 0; i < pooledObjects.Count; i++)
                if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag) return pooledObjects[i];
            foreach (var item in itemsToPool)
                if (item.objectToPool.tag == tag)
                    if (item.shouldExpand) {
                        var obj = (GameObject) Instantiate (item.objectToPool);
                        obj.SetActive (false);
                        pooledObjects.Add (obj);
                        return obj;
                    }

            return null;
        }

        public GameObject GetPooledObject (string tag, int id) {
            //if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag && pooledObjects[i].GetComponent<Enemy>().GetEnemyType()==id) {
            // && pooledObjects[i].GetComponent<ObjectInGame> ().objectID == id

            for (var i = 0; i < pooledObjects.Count; i++)
                if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag &&
                    pooledObjects[i].GetComponent<ObjectInGame>().objectID == id)
                    return pooledObjects[i];

            foreach (var item in itemsToPool)
                if (item.objectToPool.CompareTag (tag))
                    if (item.shouldExpand) {
                        var obj = (GameObject) Instantiate (item.objectToPool);
                        obj.SetActive (false);
                        pooledObjects.Add (obj);
                        return obj;
                    }

            return null;
        }

        private void SetTag (ObjectPoolItem _item) {
            _item.objectToPool.GetComponent<ObjectInGame> ().objectType = _item.objectType;
            switch (_item.objectType) {
                case ObjectProperties.ObjectType.Collectable:
                    _item.objectToPool.tag = StringUtils.tag_Collectable;
                    break;
                case ObjectProperties.ObjectType.Particles:
                    _item.objectToPool.tag = StringUtils.tag_Particles;
                    break;
                case ObjectProperties.ObjectType.Obstacle:
                    _item.objectToPool.tag = StringUtils.tag_Obstacle;
                    break;
            }
        }
}
