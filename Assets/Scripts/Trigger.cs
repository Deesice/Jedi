using System;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public event Action<GameObject> enterEvents;
    public event Action<GameObject> exitEvents;
    public bool checkPlayerOnly;
    private void OnTriggerEnter(Collider other)
    {
        if (checkPlayerOnly)
        {
            if (other.GetComponent<PlayerController>())
                enterEvents?.Invoke(other.gameObject);
        }
        else
            enterEvents?.Invoke(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        if (checkPlayerOnly)
        {
            if (other.GetComponent<PlayerController>())
                exitEvents?.Invoke(other.gameObject);
        }
        else
            exitEvents?.Invoke(other.gameObject);
    }
}
