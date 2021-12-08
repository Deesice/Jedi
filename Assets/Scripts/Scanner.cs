using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    Rigidbody myRigidbody;
    public static Scanner instance;
    public GrabObject Selected { get; private set; }
    LinkedList<GrabObject> inTrigger = new LinkedList<GrabObject>();
    bool grabNow;
    private void Start()
    {
        instance = this;
        myRigidbody = GetComponent<Rigidbody>();
    }
    public bool Grab(GrabObject testedObject = null)
    {
        if (!testedObject)
            testedObject = Selected;

        if (testedObject != Selected)
            return false;

        if (Selected)
        {
            grabNow = true;
            Selected.Grab(myRigidbody);
            return true;
        }
        else
            return false;
    }
    public bool Release(GrabObject testedObject = null)
    {
        if (!testedObject)
            testedObject = Selected;

        if (testedObject != Selected)
            return false;

        if (Selected)
        {
            grabNow = false;
            Selected.DestroyJoint();
            return true;
        }
        else
            return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        var grab = other.gameObject.GetComponentInParent<GrabObject>();
        if (grab)
        {
            inTrigger.AddLast(grab);
            if (!Selected && !grabNow)
            {
                Selected = grab;
                Selected.Select();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var grab = other.gameObject.GetComponentInParent<GrabObject>();
        if (grab)
        {
            if (Selected == grab)
                Selected.Unselect();
            inTrigger.Remove(grab);
            if (grabNow)
                return;

            if (inTrigger.Count > 0)
            {
                Selected = inTrigger.First.Value;
                Selected.Select();
            }
            else
                Selected = null;
        }
    }
}
