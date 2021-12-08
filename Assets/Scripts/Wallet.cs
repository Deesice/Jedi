using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wallet : MonoBehaviour
{
    public Text text;
    public RectTransform hidePosition;
    public RectTransform showPosition;    
    public float showSpeed;
    public float showDelay;

    static Wallet instance;
    int money;
    new RectTransform transform;
    RectTransform currentPosition;
    Animator cogwheeelAnimator;
    void Awake()
    {
        cogwheeelAnimator = GetComponentInChildren<Animator>(true);
        instance = this;
        transform = GetComponent<RectTransform>();
        Hide();
    }
    public static void Show()
    {
        instance.CancelInvoke();
        instance.currentPosition = instance.showPosition;
        instance.Invoke(nameof(Hide), instance.showDelay);
    }
    public static void Add()
    {
        Show();
        instance.money++;
        instance.text.text = instance.money.ToString();
        instance.Pulse();
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, currentPosition.position, Time.deltaTime * showSpeed);
    }
    void Hide()
    {
        currentPosition = hidePosition;
    }
    void Pulse()
    {
        cogwheeelAnimator.SetTrigger("play");
    }
    public static Vector3 CollectPosition()
    {
        return instance.showPosition.position;
    }
}
