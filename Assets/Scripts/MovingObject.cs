using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Transform[] points;
    public float speed;

    public void MoveTo(int point)
    {
        if (point >= points.Length)
            Debug.Log(this + " does not contain " + point + " point");
        else
            Tweener.MoveTo(transform, points[point], speed);
    }
}
