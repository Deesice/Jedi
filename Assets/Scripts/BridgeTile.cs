using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BridgeTile : MonoBehaviour, IPool
{
    protected bool isBroken { get; private set; }
    protected float timeSinceBreak { get; private set; }
    public float BreakProgress => timeSinceBreak / Road.TileBreakTime;
    Vector3 breakAngularSpeed;
    bool particleSpawned;
    [Header("Break global")]
    public GameObject particle;
    public bool breakGlobal;
    Rigidbody rb;
    bool isPhysical;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isPhysical = rb;
        breakAngularSpeed = new Vector3(Random.Range(-1.0f, 1.0f),
            Random.Range(-1.0f, 1.0f),
            Random.Range(-1.0f, 1.0f));
    }
    public void OnTakeFromPool()
    {
        if (isPhysical)
            rb.isKinematic = false;
        particleSpawned = false;
        timeSinceBreak = 0;
        isBroken = false;
        OnTakeFromPoolExtent();
    }
    protected virtual void OnTakeFromPoolExtent() { }
    protected virtual void Update()
    {
        if (isBroken)
            timeSinceBreak += Time.deltaTime;

        if (breakGlobal && BreakProgress >= 1)
        {
            if (isPhysical)
                rb.isKinematic = true;
            transform.position += (timeSinceBreak - Road.TileBreakTime) * Time.deltaTime * Physics.gravity;
            transform.rotation *= Quaternion.Euler((timeSinceBreak - Road.TileBreakTime) * Time.deltaTime * breakAngularSpeed);
            if (!particleSpawned && transform.position.y <= Ground.instance.offset.y)
            {
                particleSpawned = true;
                PoolManager.Instantiate(particle, transform.position);
            }
        }

        UpdateExtent();        
    }
    protected virtual void UpdateExtent() { }
    public void Break()
    {
        if (isBroken)
            return;

        isBroken = true;
        BreakExtent();
    }
    protected virtual void BreakExtent() { }
}
