using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAgent : MonoBehaviour, IPool
{
    [HideInInspector] public Quaternion defauldRotation;
    Renderer[] renderers;
    private void OnDisable()
    {
        defauldRotation = transform.localRotation;
        Wind.RemoveAgent(this);
    }
    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }
    public bool IsVisible()
    {
        foreach (var r in renderers)
            if (r.isVisible)
                return true;
        return false;
    }
    public void OnTakeFromPool()
    {
        Wind.AddAgent(this);
    }
}
