using System;
using UnityEngine;
using UnityEngine.UI;

public class Meter : MonoBehaviour
{
    Text text;
    static Meter instance;
    void Awake()
    {
        text = GetComponent<Text>();
        instance = this;
    }
    public static void SetValue(int value)
    {
    }
}
