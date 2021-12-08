using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaySystemAgent : MonoBehaviour
{
    public bool activeAtNight;
    void Start()
    {
        gameObject.SetActive(!DaySystem.IsDay);
    }
}
