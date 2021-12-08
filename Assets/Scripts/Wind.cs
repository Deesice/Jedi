using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    static Wind instance;
    List<WindAgent> agents = new List<WindAgent>();
    public Vector3 direction;
    public float speed;
    public float baseStrength;
    public float additionalStrength;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        var angle = Mathf.PerlinNoise(Time.time * speed, Time.time * speed * 0.5f) * (baseStrength + additionalStrength);
        var q = Quaternion.AngleAxis(angle, direction);

        foreach (var agent in agents)
        {
            if (agent.IsVisible())
                agent.transform.localRotation = agent.defauldRotation * q;
        }
    }
    public static void SetAdditionalStrength(float f)
    {
        instance.additionalStrength = f;
    }
    public static void AddAgent(WindAgent agent)
    {
        instance.agents.Add(agent);
    }
    public static void RemoveAgent(WindAgent agent)
    {
        instance.agents.Remove(agent);
    }
}
