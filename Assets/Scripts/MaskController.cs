using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskController : MonoBehaviour
{
    public Material maskShaderMaterial;
    Camera cam;
    public Transform target;
    public Vector2 shift;
    public Vector2 resolution;
    public float goalScale;
    public float speed;
    float scale;
    float multiplier;
    void Start()
    {
        cam = Camera.main;
        maskShaderMaterial.SetFloat("_Aspect", cam.aspect);
        multiplier = cam.aspect * cam.aspect * 2;
        maskShaderMaterial.SetFloat("_Scale", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (scale != multiplier && scale != 0)
        {
            shift.x = -Vector3.SignedAngle(Vector3.ProjectOnPlane((target.position - cam.transform.position), cam.transform.up), cam.transform.forward, cam.transform.up) / cam.aspect;
            shift.y = Vector3.SignedAngle(Vector3.ProjectOnPlane((target.position - cam.transform.position), cam.transform.right), cam.transform.forward, cam.transform.right);
            shift /= cam.fieldOfView;
            maskShaderMaterial.SetFloat("_OffsetX", shift.x);
            maskShaderMaterial.SetFloat("_OffsetY", shift.y);
        }
        if (scale == goalScale)
            return;
        if (scale < goalScale)
        {
            scale += Time.deltaTime * speed;
            if (scale > goalScale)
                scale = goalScale;
        }
        else
        {
            scale -= Time.deltaTime * speed;
            if (scale < goalScale)
                scale = goalScale;
        }
        maskShaderMaterial.SetFloat("_Scale", scale);
    }
    public void SetScale(float scale)
    {
        goalScale = Mathf.Clamp(scale, 0, 1) * multiplier;
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
