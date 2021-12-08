using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour, IPool
{
    Rigidbody myRigidbody;
    const float massMultiplier = 0.001f;
    FixedJoint joint;
    public Renderer rendererForHighlightColorization;
    public GameObject highlight;
    Vector3 initialHighlightPos;
    bool moveHighlight;
    public float Mass => myRigidbody.mass;
    bool readyToAddScore;
    bool scoreAdded;

    static PhysicMaterial zeroFric;
    static PhysicMaterial fullFric;
    void Awake()
    {
        initialHighlightPos = highlight.transform.localPosition;
        highlight.SetActive(false);
        if (zeroFric == null)
        {
            zeroFric = new PhysicMaterial();
            zeroFric.bounciness = 0;
            zeroFric.bounceCombine = PhysicMaterialCombine.Minimum;
            zeroFric.dynamicFriction = 0;
            zeroFric.staticFriction = 0;
            zeroFric.frictionCombine = PhysicMaterialCombine.Minimum;
        }
        if (fullFric == null)
        {
            fullFric = new PhysicMaterial();
            fullFric.bounciness = 0;
            fullFric.bounceCombine = PhysicMaterialCombine.Minimum;
            fullFric.dynamicFriction = 1;
            fullFric.staticFriction = 1;
            fullFric.frictionCombine = PhysicMaterialCombine.Average;
        }
        SetupRigidbody();
    }
    void SetupRigidbody()
    {
        myRigidbody = gameObject.AddComponentOrGetIfExists<Rigidbody>();
        myRigidbody.SetDensity(1.25f);
        myRigidbody.mass = myRigidbody.mass;
        myRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        foreach (var c in GetComponentsInChildren<Collider>(true))
            c.sharedMaterial = fullFric;
    }
    private void Update()
    {
        if (Mathf.Abs(transform.position.x) > Road.instance.tileSize / 2)
        {
            if (joint != null)
                PlayerController.instance.HandsDown(null, true);
            myRigidbody.velocity += new Vector3(Time.deltaTime * 0.1f * Mathf.Sign(transform.position.x), 0, 0);
        }
        var y = transform.position.y;
        if (y < -0.5f)
        {
            if (readyToAddScore)
            {
                scoreAdded = true;
                Sticker.PlaySticker(myRigidbody.worldCenterOfMass);
                var v = myRigidbody.velocity;
                v.y = 0;
                myRigidbody.velocity = v * 0.1f + Vector3.up * 10; // для отскока вверх
                myRigidbody.angularVelocity *= -4;
            }
            readyToAddScore = false;
        }
        if (scoreAdded)
            myRigidbody.velocity += Time.deltaTime * 3 * Physics.gravity; // увеличиваем гравитацию в 4 раза
        if (y < Ground.instance.offset.y * 2)
        {
            PoolManager.Destroy(gameObject);
        }
        if (moveHighlight)
            highlight.transform.localPosition += 15 * Time.deltaTime * Vector3.up;
    }
    public void DestroyJoint()
    {
        Destroy(joint);
        SetupRigidbody();
        myRigidbody.velocity += Vector3.up;
    }
    public void Grab(Rigidbody hand)
    {
        readyToAddScore = true;
        moveHighlight = true;
        Unselect(Color.white);

        joint = gameObject.AddComponent<FixedJoint>();
        myRigidbody.mass *= massMultiplier;
        //joint.breakForce = myRigidbody.mass * 10;
        //joint.breakTorque = myRigidbody.mass * 10;
        joint.connectedBody = hand;
        myRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        foreach (var c in GetComponentsInChildren<Collider>(true))
            c.sharedMaterial = zeroFric;
    }
    public void Select()
    {
        moveHighlight = false;
        highlight.transform.localPosition = initialHighlightPos;
        OutlineController.Subscribe(HighlightOn);
        OutlineController.SwapColor(rendererForHighlightColorization.sharedMaterial.color);
    }
    void HighlightOn()
    {
        OutlineController.Unsibscribe(HighlightOn);
        highlight.SetActive(true);
    }
    public void Unselect()
    {
        Unselect(rendererForHighlightColorization.sharedMaterial.color);
    }
    void Unselect(Color color)
    {
        OutlineController.Subscribe(HighlightOff);
        color.a = 0;
        OutlineController.SwapColor(color);
    }
    void HighlightOff()
    {
        moveHighlight = false;
        OutlineController.Unsibscribe(HighlightOff);
        highlight.SetActive(false);
    }

    public void OnTakeFromPool()
    {
        scoreAdded = false;
        readyToAddScore = false;
    }
}
