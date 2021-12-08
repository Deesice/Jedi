using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour, IPool
{
    bool active;
    public float activateDistance;
    public float speed;
    public float windStrength;
    Vector3 initialLocalPos;
    private void Awake()
    {
        initialLocalPos = transform.localPosition;
    }
    void Update()
    {
        if (active)
        {
            transform.position += Time.deltaTime * speed * transform.up;
            Wind.SetAdditionalStrength(Mathf.InverseLerp(Ground.instance.tileSize / 2, 0, Mathf.Abs(transform.position.x)) * windStrength);
        }
        else
        {
            if ((transform.position.z - PlayerController.instance.transform.position.z) <= activateDistance)
                active = true;
        }
    }

    public void OnTakeFromPool()
    {
        transform.localPosition = initialLocalPos;
        active = false;
    }
}
