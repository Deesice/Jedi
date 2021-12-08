using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimpleBridge : BridgeTile
{
    [SerializeField] Transform groundCollider;
    [SerializeField] Transform[] bodies;

    Vector3[] initialPoses;
    Vector3[] degreesSpeeds;
    Vector3[] initialRotes;
    bool[] dustSpawned;
    protected override void OnTakeFromPoolExtent()
    {
        groundCollider.localPosition = Vector3.zero;
        groundCollider.localScale = Vector3.one;
        for (int i = 0; i < bodies.Length; i++)
        {
            var rb = bodies[i];
            rb.localPosition = initialPoses[i];
            rb.localRotation = Quaternion.Euler(initialRotes[i]);
            dustSpawned[i] = false;
        }
    }
    private void Awake()
    {
        bodies = (from t in bodies
                  orderby t.localPosition.z
                  select t).ToArray();

        initialPoses = new Vector3[bodies.Length];
        initialRotes = new Vector3[bodies.Length];
        degreesSpeeds = new Vector3[bodies.Length];
        dustSpawned = new bool[bodies.Length];

        for (int i = 0; i < bodies.Length; i++)
        {
            initialPoses[i] = bodies[i].transform.localPosition;
            initialRotes[i] = bodies[i].transform.localRotation.eulerAngles;
            degreesSpeeds[i] = new Vector3(Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f)) * Mathf.Rad2Deg;
        }
    }
    protected override void UpdateExtent()
    {
        var v = groundCollider.localScale;
        v.z = 1 - timeSinceBreak / Road.TileBreakTime;
        if (v.z > 0)
        {
            groundCollider.localScale = v;
            groundCollider.localPosition = new Vector3(0, 0, timeSinceBreak * Road.instance.tileSize / Road.TileBreakTime / 2);
        }
        else
        {
            v.z = 0;
            groundCollider.localScale = v;
            groundCollider.localPosition = new Vector3(0, 0, Road.instance.tileSize / 2);
        }

        for (int i = 0; i < bodies.Length; i++)
        {
            var curBrokenTime = timeSinceBreak;
            curBrokenTime -= i * Road.TileBreakTime / bodies.Length;
            if (curBrokenTime > 0)
            {
                bodies[i].localPosition += curBrokenTime * Time.deltaTime * Physics.gravity;
                bodies[i].localRotation *= Quaternion.Euler(curBrokenTime * Time.deltaTime * degreesSpeeds[i]);

                if (!dustSpawned[i] && bodies[i].localPosition.y <= Ground.instance.offset.y)
                {
                    dustSpawned[i] = true;
                    PoolManager.Instantiate(particle, bodies[i].position);
                }
            }
        }
    }
}
