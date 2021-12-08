using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleAction : MonoBehaviour
{
    public UnityEvent actions;
    public void Action()
    {
        actions.Invoke();
    }
}
