using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraBehaviour : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float wall;
    public Vector3 offset;
    bool nearGarage;
    public UnityEvent onGarageEnter;
    public UnityEvent onGarageExit;

    static CameraBehaviour instance;
    float nextShakeTime;
    Vector3 targetShakeOffset;
    Vector3 currentShakeOffset;

    Dictionary<Component, Vector2> shakeParams = new Dictionary<Component, Vector2>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            throw new Exception("Second camera behaviour found!");
    }
    void LateUpdate()
    {
        float shakeRadius = 0;
        float shakeSpeed = 0;
        foreach (var i in shakeParams.Values)
        {
            if (shakeRadius == 0)
            {
                if (i.x == 0)
                    shakeSpeed = Mathf.Max(shakeSpeed, i.y);
                else
                    shakeSpeed = i.y;
            }
            else if (i.x > 0)
                shakeSpeed = Mathf.Max(shakeSpeed, i.y);
            
            shakeRadius = Mathf.Max(shakeRadius, i.x);
        }

        nextShakeTime -= Time.deltaTime * shakeSpeed;
        if (nextShakeTime <= 0)
        {
            nextShakeTime = 1;
            targetShakeOffset = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f),
                UnityEngine.Random.Range(-1.0f, 1.0f),
                UnityEngine.Random.Range(-1.0f, 1.0f)).normalized * shakeRadius;
        }
        currentShakeOffset = Vector3.Lerp(currentShakeOffset, targetShakeOffset, Time.deltaTime * shakeSpeed);
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * speed) + currentShakeOffset;
        //if (transform.position.z < Road.instance.firstTileZPos - Road.instance.tileSize / 2 + wall)
        //{
        //    transform.position = new Vector3(transform.position.x, transform.position.y, Road.instance.firstTileZPos - Road.instance.tileSize / 2 + wall);
        //    if (nearGarage == false)
        //        onGarageEnter?.Invoke();
        //    nearGarage = true;
        //}
        //else
        //{
        //    if (nearGarage == true)
        //        onGarageExit?.Invoke();
        //    nearGarage = false;
        //}
    }
    public static void SetShakeScreen(float radius, float speed, Component sender)
    {
        instance.shakeParams.Remove(sender);
        instance.shakeParams.Add(sender, new Vector2(radius, speed));
    }
}
