using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float customGravityAcc;
    public float speed;
    public float angularSpeed;

    public Transform[] wheels;
    public bool mobileControl;
    
    Rigidbody myRigidbody;
    Vector2 input;    
    Joystick joystick;
    ConfigurableJoint joint;
    public float angle;
    Vector3 euler;
    IEnumerator coroutine;
    bool readyToDoubleTap;
    bool handsUp;

    public static PlayerController instance;
    public bool forcedMovement { get; set; }
    public GameObject trailPrefab;
    public float speedMultiplierWhenInteract;
    Camera cam;

    float lastTapTime;
    int lastTapCount;
    RaycastHit[] hits = new RaycastHit[5];

    void Awake()
    {
        cam = Camera.main;
        forcedMovement = true;
        instance = this;
        myRigidbody = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();
        joystick = FindObjectOfType<Joystick>(true);
        //StartCoroutine(Trailing());
        var collider = GetComponentInChildren<Collider>();
        var mat = new PhysicMaterial();
        mat.bounciness = 0;
        mat.dynamicFriction = 0;
        mat.staticFriction = 0;
        collider.material = mat;
        Invoke(nameof(ChangeFriction), 1);
    }
    void ChangeFriction()
    {
        var collider = GetComponentInChildren<Collider>();
        collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
    }
    void Update()
    {
        input.y = joystick.Vertical;
        input.x = joystick.Horizontal;
        input = Vector3.ClampMagnitude(input, 1);

        bool doubleTap = false;
        if (Input.touchCount == 0)
            lastTapCount = 0;
        else
        {
            var touch = Input.GetTouch(0);
            if (touch.tapCount > lastTapCount)
            {
                lastTapCount = touch.tapCount;
                if (Time.time - lastTapTime < 0.25f)
                    doubleTap = true;
                lastTapTime = Time.time;
            }
        }
        GrabObject test = null;
        if (Input.GetMouseButtonDown(0))
        {
            var count = Physics.RaycastNonAlloc(cam.ScreenPointToRay(Input.mousePosition), hits);
            for (int i = 0; i < count; i++)
            {
                test = hits[i].collider.gameObject.GetComponentInParent<GrabObject>();
                if (test)
                {
                    doubleTap = true;
                    break;
                }
            }
        }
        if (doubleTap || Input.GetMouseButtonDown(1) || Input.GetButtonDown("Jump"))
            SwitchHands(test);

        if (transform.position.y < Ground.instance.offset.y)
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
    void SwitchHands(GrabObject test)
    {
        if (handsUp)
            HandsDown(test);
        else
            HandsUp(test);
    }
    public void HandsUp(GrabObject test)
    {
        if (handsUp || coroutine != null)
            return;
        handsUp = Scanner.instance.Grab(test);

        if (!handsUp)
            return;

        coroutine = Hands(true, 0.5f);
        StartCoroutine(coroutine);
    }
    public void HandsDown(GrabObject test, bool forced = false)
    {
        if (!handsUp || (!forced && coroutine != null))
            return;

        handsUp = !Scanner.instance.Release(test);

        if (handsUp)
            return;

        coroutine = Hands(false, 0.5f);
        StartCoroutine(coroutine);
    }
    IEnumerator Hands(bool isUp, float time)
    {
        var startPos = joint.targetPosition;
        float i = 0;
        while (i < 1)
        {
            if (isUp)
                joint.targetPosition = Vector3.Lerp(startPos, Vector3.right * joint.linearLimit.limit, i);
            else
                joint.targetPosition = Vector3.Lerp(startPos, Vector3.left * joint.linearLimit.limit, i);
            yield return null;
            i += Time.deltaTime / time;
        }
        joint.targetPosition = isUp ? Vector3.right * joint.linearLimit.limit : Vector3.left * joint.linearLimit.limit;
        coroutine = null;
    }
    IEnumerator Trailing()
    {
        while(true)
        {
            yield return null;
            PoolManager.Instantiate(trailPrefab, transform.position, transform.rotation);
        }
    }
    private void FixedUpdate()
    {
        myRigidbody.velocity -= Time.fixedDeltaTime * customGravityAcc * Vector3.up;
        Vector3 inputInWorldCoordinates;
        if (!forcedMovement)
        {
            inputInWorldCoordinates = cam.transform.right * input.x + cam.transform.forward * input.y;
            inputInWorldCoordinates.y = 0;
            inputInWorldCoordinates = inputInWorldCoordinates.normalized * input.magnitude;
        }
        else
        {
            inputInWorldCoordinates = Vector3.forward;
        }

        angle = Vector3.SignedAngle(inputInWorldCoordinates, transform.forward, Vector3.up);
        foreach (var i in wheels)
        {
            euler = i.transform.localRotation.eulerAngles;
            euler.y = Mathf.Clamp(angle, -30, 30) + 90;
            i.transform.localRotation = Quaternion.Euler(euler);
        }
        var multiplier = 1.0f;
        if (handsUp)
        {
            multiplier = speedMultiplierWhenInteract / Scanner.instance.Selected.Mass;
            if (multiplier > 1)
                multiplier = 1;
        }
        multiplier *= inputInWorldCoordinates.magnitude;
        myRigidbody.angularVelocity = multiplier * multiplier * angle * angularSpeed * -Vector3.up;

        myRigidbody.velocity = multiplier * speed * Mathf.Cos(angle * Mathf.Deg2Rad) * transform.forward
            + new Vector3(0, myRigidbody.velocity.y, 0); //для сохранения вертикальной скорости
    }
}
