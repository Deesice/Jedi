using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    static Tweener _instance;
    static Tweener Instance {
        get { 
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<Tweener>();
            }
            return _instance;
        } }
    Dictionary<Transform, IEnumerator> coroutines = new Dictionary<Transform, IEnumerator>();
    public static void MoveTo(Transform target, Transform goal, float speed)
    {
        if (Instance.coroutines.TryGetValue(target, out var coroutine))
        {
            Instance.StopCoroutine(coroutine);
            Instance.coroutines.Remove(target);
        }
        coroutine = Instance.MovingTo(target, goal, speed);
        Instance.coroutines.Add(target, coroutine);
        Instance.StartCoroutine(coroutine);
    }
    IEnumerator MovingTo(Transform target, Transform goal, float speed)
    {
        Vector3 delta;
        while (target.position != goal.position)
        {
            yield return null;
            delta = goal.position - target.position;
            target.position += delta.normalized * speed * Time.deltaTime;
            if (delta.magnitude <= speed * Time.deltaTime)
                target.position = goal.position;
        }
        Instance.coroutines.Remove(target);
    }
}
