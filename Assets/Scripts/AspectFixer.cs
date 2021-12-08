using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectFixer : MonoBehaviour
{
    public float referenceVerticalAspect;
    void Start()
    {
        var cam = Camera.main;
        Debug.Log(cam.aspect);
        cam.fieldOfView = cam.fieldOfView * referenceVerticalAspect / cam.aspect;
    }
}
