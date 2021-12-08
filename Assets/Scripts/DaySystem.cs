using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaySystem : MonoBehaviour
{
    enum Mode { Auto, Day, Night}
    [SerializeField] Mode mode;
    [SerializeField] Color dayColor;
    [SerializeField] Color nightColor;
    [SerializeField] Light mainLight;
    public static bool IsDay { get; private set; }
    void Awake()
    {
        var date = DateTime.Now;
        var currentTime = date.Hour * 3600 + date.Minute * 60 + date.Second;
        switch (mode)
        {
            case Mode.Auto:
                IsDay = currentTime >= 21600 && currentTime < 64800;
                break;
            case Mode.Day:
                IsDay = true;
                break;
            case Mode.Night:
                IsDay = false;
                break;
        }

        mainLight.color = IsDay ? dayColor : nightColor;
        RenderSettings.ambientIntensity = IsDay ? 1 : 0.5f;
    }
}
