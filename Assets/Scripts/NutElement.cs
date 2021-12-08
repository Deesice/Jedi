using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutElement : MonoBehaviour
{
    public StickerType type;
    Vector3 localPosition;
    new RectTransform transform;
    const float transferTime = 0.3f;
    float currentTimeNormalized;
    void Awake()
    {
        transform = GetComponent<RectTransform>();
        localPosition = transform.localPosition;
    }
    public void ResetPosition()
    {
        currentTimeNormalized = 0;
        transform.localPosition = localPosition;
    }
    public void MoveToWallet()
    {
        currentTimeNormalized = 1;
    }
    private void Update()
    {
        if (currentTimeNormalized > 0)
        {
            currentTimeNormalized -= Time.deltaTime / transferTime;
            if (currentTimeNormalized > 0)
            {
                var v = transform.position;
                var v1 = Wallet.CollectPosition();
                var m = (v - v1).magnitude;
                transform.position = Vector3.Lerp(transform.position,
                    Wallet.CollectPosition(),
                    Time.deltaTime / currentTimeNormalized / transferTime);
            }
            else
            {
                Wallet.Add();
                gameObject.SetActive(false);
            }
        }
    }
}
