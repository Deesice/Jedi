using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour, IPool
{
    Quaternion initialRotation;
    public Vector3 rotation;
    public void OnTakeFromPool()
    {
        transform.rotation = initialRotation;
        var v = new Vector3(Random.Range(0, rotation.x), Random.Range(0, rotation.y), Random.Range(0, rotation.z));
        transform.Rotate(v, Space.World);
    }

    // Start is called before the first frame update
    private void Awake()
    {
        initialRotation = transform.rotation;
    }
}
