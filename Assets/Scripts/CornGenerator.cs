using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
[RequireComponent(typeof(MeshCombiner))]
public class CornGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    public GameObject cornPrefab;
    public float range;
    public float width;
    public int count;
    public float minScale;
    public float maxScale;
    public Vector3 rotation;
    public Material material;
    [ContextMenu("Generate")]
    public void Generate()
    {
        DestroyImmediate(GetComponent<MeshFilter>());
        DestroyImmediate(GetComponent<MeshRenderer>());
        for (int i = 0; i < count; i++)
        {
            var g = Instantiate(cornPrefab);
            var v = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
            g.transform.position = v * range;
            g.transform.localScale = new Vector3(width, 1, width) * Random.Range(minScale, maxScale);

            v = new Vector3(Random.Range(0, rotation.x), Random.Range(0, rotation.y), Random.Range(0, rotation.z));
            g.transform.Rotate(v, Space.World);
            g.transform.parent = transform;
        }
        var mesh = gameObject.GetComponent<MeshCombiner>().Combine(material);
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        AssetDatabase.CreateAsset(mesh, "Assets/" + gameObject.name + System.DateTime.Now.ToString().GetHashCode());
    }
#endif
}
