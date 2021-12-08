using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RandomColor : MonoBehaviour, IPool
{ 
    public Material[] materials;
    bool rendererCatched;
    new Renderer renderer;
    Renderer GetRenderer()
    {
        if (rendererCatched)
            return renderer;
        else
        {
            rendererCatched = true;
            renderer = GetComponent<Renderer>();
            return renderer;
        }
    }
    private void Awake()
    {
        OnTakeFromPool();
    }
    public void OnTakeFromPool()
    {
        if (materials.Length > 0)
            GetRenderer().sharedMaterial = materials[Random.Range(0, materials.Length)];
    }
    public Color GetRandomColor()
    {
        if (materials.Length > 0)
            return materials[Random.Range(0, materials.Length)].color;
        else
            return GetRenderer().sharedMaterial.color;
    }
}
