using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GroundCleaner : MonoBehaviour, IPool
{
    [SerializeField] Collider checkingCollider;
    [SerializeField] List<Collider> myColliders;
    static Collider[] results = new Collider[2000];
    public bool cleanNoise;
    public async void OnTakeFromPool()
    {
        await Task.Delay(1);
        var bounds = checkingCollider.bounds;
        var count = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, results);
        for (int i = 0; i < count; i++)
        {
            if (results[i] != checkingCollider && !myColliders.Contains(results[i]))
                Ground.DeleteObstacle(results[i].gameObject);
        }

        if (!cleanNoise)
            return;

        var minPointNormalized = (bounds.min - transform.position) / Ground.instance.tileSize + Vector3.one * 0.5f;
        var maxPointNormalized = (bounds.max - transform.position) / Ground.instance.tileSize + Vector3.one * 0.5f;

        var minPointInt = new Vector3Int((int)(minPointNormalized.x * 10), (int)(minPointNormalized.y * 10), (int)(minPointNormalized.z * 10));
        var maxPointInt = new Vector3Int((int)(maxPointNormalized.x * 10), (int)(maxPointNormalized.y * 10), (int)(maxPointNormalized.z * 10));

        var rows = new List<int>();
        var columns = new List<int>();

        for (int i = minPointInt.x; i <= maxPointInt.x; i++)
            columns.Add(i);

        for (int i = minPointInt.z; i <= maxPointInt.z; i++)
            rows.Add(i);

        Ground.instance.ClearNoiseAtLastGroundTile(transform.position.z, rows.ToArray(), columns.ToArray());
    }
}
