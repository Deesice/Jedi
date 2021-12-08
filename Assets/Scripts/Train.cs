using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour, IPool
{
    public float speed;
    public bool active;
    public float activateDistance;
    public float maxShakeRadius;
    public float shakeSpeed;
    Vector3 initialLocalPos;
    float colliderExtents;
    public void OnTakeFromPool()
    {
        transform.localPosition = initialLocalPos;
        active = false;
        enabled = true;
    }

    private void Awake()
    {
        initialLocalPos = transform.localPosition;
        var bounds = GetComponent<Collider>().bounds;
        colliderExtents = Mathf.Max(bounds.extents.x,
            bounds.extents.y,
            bounds.extents.z);
    }
    void Update()
    {
        if (active)
        {
            transform.position += Time.deltaTime * speed * transform.right;
            var toPlayer = Mathf.Abs((PlayerController.instance.transform.position - transform.position).x) - colliderExtents;
            var strength = Mathf.InverseLerp(Ground.instance.tileSize, 0, toPlayer);
            CameraBehaviour.SetShakeScreen(strength * maxShakeRadius, shakeSpeed, this);
            if (strength == 0)
                enabled = false;
        }
        else
        {
            if ((transform.position.z - PlayerController.instance.transform.position.z) <= activateDistance)
                active = true;
        }
    }
}
