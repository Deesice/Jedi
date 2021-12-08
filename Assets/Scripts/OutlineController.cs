using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Material pulseMaterial;
    public Material stationaryMaterial;
    public AnimationCurve curve;
    public float pulseLength;
    Color color;
    [SerializeField] Color baseMultiplier;
    public event Action ColorSwapped;
    public float swapHalfTime;
    IEnumerator coroutine;
    static OutlineController instance;
    float offset;
    private void Awake()
    {
        instance = this;
    }
    void Update()
    {
        var time = (Time.time - offset) / pulseLength;
        var currentColor = color * baseMultiplier;
        currentColor.a = Mathf.Lerp(0, color.a * baseMultiplier.a, curve.Evaluate(time - Mathf.Floor(time)));
        pulseMaterial.color = currentColor;
    }
    public static void Subscribe(Action a)
    {
        instance.ColorSwapped += a;
    }
    public static void Unsibscribe(Action a)
    {
        instance.ColorSwapped -= a;
    }
    public static void SwapColor(Color newColor)
    {
        if (instance.coroutine != null)
            instance.StopCoroutine(instance.coroutine);

        instance.coroutine = instance.SwappingColor(newColor);
        instance.StartCoroutine(instance.coroutine);
    }
    IEnumerator SwappingColor(Color newColor)
    {
        float i = 1 - color.a;
        while (i < 1)
        {
            i += Time.deltaTime / swapHalfTime;
            color.a = 1 - i;
            yield return null;
        }
        color.a = 0;
        color.r = newColor.r;
        color.g = newColor.g;
        color.b = newColor.b;
        ColorSwapped?.Invoke();
        offset = Time.time;
        i = 0;
        while (i < newColor.a)
        {
            i += Time.deltaTime / swapHalfTime;
            color.a = i;
            yield return null;
        }
        color = newColor;
        coroutine = null;
    }
}
