using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScale : MonoBehaviour, IPool
{
    public float minScale;
    public float maxScale;
    Vector3 initialScale;
    private void Awake()
    {
        initialScale = transform.localScale;
    }
    public void OnTakeFromPool()
    {
        transform.localScale = initialScale * Random.Range(minScale, maxScale);
    }
}
