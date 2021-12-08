using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum StickerType { None, Good, Great }

public class Sticker : MonoBehaviour
{
    public Sprite[] sprites;
    public GameObject particle;
    public Image stickerImage;
    [Range(0, 0.5f)]
    public float normalizedBorder = 0.1f;

    Camera cam;
    new RectTransform transform;
    Vector3 lastScorePosition;
    Vector2Int resolution;
    Animator stickerAnimator;
    static Sticker instance;
    NutElement[] nuts;
    private void Awake()
    {
        nuts = FindObjectsOfType<NutElement>(true);
        stickerAnimator = GetComponentInChildren<Animator>(true);
        transform = GetComponent<RectTransform>();
        cam = Camera.main;
        instance = this;
        resolution.x = Screen.width;
        resolution.y = Screen.height;
    }
    private void LateUpdate()
    {
        UpdatePos();
    }
    public static void PlaySticker(Vector3 particlePos)
    {
        PoolManager.Instantiate(instance.particle, particlePos);
        instance.lastScorePosition = particlePos;

        var stickerType = GetStickerType(Mathf.Abs(particlePos.x));
        foreach (var i in instance.nuts)
        {
            i.ResetPosition();
            i.gameObject.SetActive(i.type == stickerType);
        }
        switch (stickerType)
        {
            case StickerType.None:
                instance.stickerImage.sprite = instance.sprites[0];
                break;
            case StickerType.Good:
                instance.stickerImage.sprite = instance.sprites[1];
                break;
            case StickerType.Great:
                instance.stickerImage.sprite = instance.sprites[2];
                break;
        }

        instance.stickerAnimator.SetTrigger("play");
    }
    public void OnReadyToCollect()
    {
        Wallet.Show();
        foreach (var i in instance.nuts)
        {
            if (i.gameObject.activeSelf)
            {
                i.MoveToWallet();
                return;
            }
        }
    }
    void UpdatePos()
    {
        var newPos = cam.WorldToScreenPoint(lastScorePosition);
        newPos.x = Mathf.Clamp(newPos.x, normalizedBorder * resolution.x, (1 - normalizedBorder) * resolution.x);
        newPos.y = Mathf.Clamp(newPos.y, normalizedBorder * resolution.y * cam.aspect, (1 - normalizedBorder * cam.aspect) * resolution.y);
        newPos.z = 0;
        transform.position = newPos;
    }
    static StickerType GetStickerType(float distance)
    {
        distance /= Road.instance.tileSize;

        if (distance < 1.5f)
            return StickerType.None;
        if (distance < 2.5f)
            return StickerType.Good;

        return StickerType.Great;
    }
}
