using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool : MonoBehaviour
{
    private Dictionary<System.Type, List<MonoBehaviour>> pools = new Dictionary<System.Type, List<MonoBehaviour>>();

    public void AddToPool<T>(T obj) where T : MonoBehaviour
    {
        var type = typeof(T);
        if (!pools.ContainsKey(type))
        {
            pools.Add(type, new List<MonoBehaviour>());
        }

        pools[type].Add(obj);
    }

    public T Spawn<T>(T prefab) where T : MonoBehaviour
    {
        var type = typeof(T);
        var hasType = pools.ContainsKey(type);

        if (!hasType || pools[type].Count == 0)
        {
            return InstantiateNewObject(prefab);
        }

        T pooledObj = pools[type].Find(o => !o.gameObject.activeSelf) as T;

        if(pooledObj == null)
        {
            pooledObj = InstantiateNewObject(prefab);
        }     
        
        pooledObj.gameObject.SetActive(true);

        return pooledObj;
    }

    private T InstantiateNewObject<T>(T prefab) where T : MonoBehaviour
    {
        T obj = Instantiate(prefab);
        obj.transform.localScale = Vector3.one;
        AddToPool(obj);
        return obj;
    }
}