using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour {
    private static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance { get { return _instance; } }

    private Dictionary<string, ObjectPool> objectPools;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        objectPools = new Dictionary<string, ObjectPool>();
    }

    private ObjectPool getPoolForObject(GameObject obj) {
        ObjectPool pool;
        var didGet = objectPools.TryGetValue(obj.name, out pool);
        if (!didGet) {
            pool = new ObjectPool(obj);
            objectPools[obj.name] = pool;
        }
        return pool;
    }

    public GameObject getObjectInstance(GameObject prefab) {
        var pool = getPoolForObject(prefab);
        return pool.getObjectInstance();
    }

    public ObjectPool getObjectPool(GameObject prefab) {
        return getPoolForObject(prefab);
    }
}
