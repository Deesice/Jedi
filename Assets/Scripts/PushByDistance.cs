using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushByDistance : MonoBehaviour
{
    private void Update()
    {
        if (PlayerController.instance.transform.position.z - transform.position.z > Road.instance.tileSize * 1.5f)
            PoolManager.Destroy(gameObject);
    }
}
