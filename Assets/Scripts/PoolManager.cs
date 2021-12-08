using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    static PoolManager instance;
    public int initialCount;
    public GameObject[] prefabs;
    Dictionary<GameObject, List<GameObject>> prefabInstances = new Dictionary<GameObject, List<GameObject>>();
    Dictionary<GameObject, IPool[]> prefabComponents = new Dictionary<GameObject, IPool[]>();
    void Awake()
    {
        instance = this;
        foreach (var i in prefabs)
            for (int j = 0; j < initialCount; j++)
                AddInstance(i);
    }
    void AddInstance(GameObject prefab)
    {
        var g = GameObject.Instantiate(prefab);
        List<GameObject> list;
        if (prefabInstances.TryGetValue(prefab, out list))
            list.Add(g);
        else
        {
            list = new List<GameObject>();
            list.Add(g);
            prefabInstances.Add(prefab, list);
        }
        g.name += list.Count;
        prefabComponents.Add(g, g.GetComponentsInChildren<IPool>());
        g.SetActive(false);
    }
    public static GameObject Instantiate(GameObject prefab, Vector3 position)
    {
        return Instantiate(prefab, position, Quaternion.identity);
    }

    public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (instance.prefabInstances.TryGetValue(prefab, out var list))
        {
            foreach (var i in list)
                if (!i.activeSelf)
                {
                    i.transform.position = position;
                    i.transform.rotation = rotation;
                    i.SetActive(true);
                    if (instance.prefabComponents.TryGetValue(i, out var components))
                        foreach (var c in components)
                            c.OnTakeFromPool();
                    return i;
                }
        }
        instance.AddInstance(prefab);
        return Instantiate(prefab, position);
    }
    public static void Destroy(GameObject destroyableObject)
    {
        if (instance.prefabComponents.ContainsKey(destroyableObject))
            destroyableObject.SetActive(false);
        else
            Destroy(destroyableObject);
    }
}

public interface IPool
{
    public void OnTakeFromPool();
}
