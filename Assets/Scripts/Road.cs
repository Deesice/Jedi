using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Road : MonoBehaviour
{
    public GameObject endTile;
    public BridgeTile[] tiles;
    public GameObject[] additionalPrefabs;
    public int minCarPerTile;
    public int maxCarPerTile;
    public GameObject[] obstacles;
    public GameObject pointerPrefab;
    Transform player;
    public float tileSize;
    public int currentTile;
    public int segmentLength = 10;
    [Range(0,1)]
    public float additionalProbability;

    public int backwardTileCount;
    public int forwardTileCount;

    LinkedList<Transform> roadTiles = new LinkedList<Transform>();
    Dictionary<Transform, GameObject> additionalTiles = new Dictionary<Transform, GameObject>();
    bool endSpawned;
    public UnityEvent onLevelEndEvents;
    public static Road instance;
    public float firstTileZPos { get; private set; }
    BridgeTile lastBreakPos;
    [SerializeField] float breakSpeed;
    public static float TileBreakTime
    {
        get
        {
            if (instance.breakSpeed > 0)
                return 1 / instance.breakSpeed;
            else
                return float.MaxValue;
        }
    }
    int brokenTiles;
    [Header("Shaking")]
    [SerializeField] float shakeRadius;
    [SerializeField] float shakeSpeed;
    [SerializeField] float shakeFalloff;
    void Awake()
    {
        instance = this;        
    }
    private void Start()
    {
        roadTiles.AddFirst(SpawnTileAtPosition(Vector3.zero, tiles[0].gameObject).transform); //спауним первый кусочек моста, на котором игрок стоит в самом начале
        for (int i = 0; i < backwardTileCount; i++)
            SpawnTileAtFirstPosition(tiles[0].gameObject);
        firstTileZPos = roadTiles.First.Value.position.z;
        SpawnTileAtLastPosition(tiles[0].gameObject);
        for (int i = 1; i < forwardTileCount; i++)
        {
            GameplayCycle(false, true);
        }
        currentTile = 0;
        player = PlayerController.instance.transform;
    }

    void GameplayCycle(bool destroyFirstTile = true, bool spawnobstacles = true)
    {
        if (destroyFirstTile)
            DeleteFirstTile();

        currentTile++;
        if (!endSpawned)
        {
            if (currentTile % segmentLength == 0)
            {
                endSpawned = true;
                SpawnTileAtLastPosition(endTile, additionalProbability).GetComponentInChildren<Trigger>().enterEvents += (g) =>
                {
                    OnLevelEnd();
                };
                Meter.SetValue(currentTile);
            }
            else
            {
                SpawnTileAtLastPosition(null, additionalProbability);
                if (spawnobstacles)
                    SpawnObstaclesAtLastTile();
            }
        }
    }
    void LateUpdate()
    {
        if (player.position.z >= firstTileZPos + (backwardTileCount + 1) * tileSize)
        {
            GameplayCycle();
        }

        if (breakSpeed > 0 && Time.timeSinceLevelLoad > brokenTiles * TileBreakTime)
        {
            lastBreakPos = GetTileByIndex(brokenTiles - currentTile)?.GetComponent<BridgeTile>();
            lastBreakPos?.Break();
            brokenTiles++;
        }

        float strength = 0;
        if (lastBreakPos)
            strength = Mathf.InverseLerp(
                    shakeFalloff,
                    0,
                    player.position.z - (lastBreakPos.transform.position.z + (lastBreakPos.BreakProgress - 0.5f) * tileSize));

        CameraBehaviour.SetShakeScreen(shakeRadius * strength, shakeSpeed, this);
    }
    Transform GetTileByIndex(int index)
    {
        int i = 0;
        foreach (var t in roadTiles)
        {
            if (i == index)
                return t;
            i++;
        }
        return null;
    }
    void SpawnObstaclesAtLastTile()
    {
        var lastTilePos = roadTiles.Last.Value.position;
        lastTilePos.x = 0;
        lastTilePos.y = 0;

        for (int i = 0; i < Random.Range(minCarPerTile, maxCarPerTile + 1); i++)
            PoolManager.Instantiate(obstacles[Random.Range(0, obstacles.Length)],
                lastTilePos + new Vector3(Random.Range(-tileSize, tileSize), 0, Random.Range(-tileSize, tileSize)) * 0.4f,
                Quaternion.Euler(0, Random.Range(0, 360), 0));
    }
    Transform SpawnTileAtLastPosition(GameObject tile = null, float additionalProbability = 0)
    {
        Transform t;
        if (roadTiles.Count == 0)
            t = SpawnTileAtPosition(Vector3.zero, tile).transform;
        else
            t = SpawnTileAtPosition(Vector3.forward * (roadTiles.Last.Value.position.z + tileSize), tile).transform;

        if (Random.Range(0.0f, 0.999f) < additionalProbability && additionalPrefabs.Length > 0)
        {
            var selected = SpawnTileAtPosition(Vector3.forward * (roadTiles.Last.Value.position.z + tileSize),
                additionalPrefabs[Random.Range(0, additionalPrefabs.Length)]);
            additionalTiles.Add(t, selected);
        }

        roadTiles.AddLast(t);
        return t;
    }
    Transform SpawnTileAtFirstPosition(GameObject tile = null)
    {
        Transform t;
        if (roadTiles.Count == 0)
            t = SpawnTileAtPosition(Vector3.zero, tile).transform;
        else
            t = SpawnTileAtPosition(Vector3.forward * (roadTiles.First.Value.position.z - tileSize), tile).transform;
        roadTiles.AddFirst(t);
        return t;
    }
    void DeleteFirstTile()
    {
        if (roadTiles.Count <= 1)
            return;

        var t = roadTiles.First.Value;
        if (additionalTiles.TryGetValue(t, out var add))
        {
            PoolManager.Destroy(add);
            additionalTiles.Remove(t);
        }
        roadTiles.RemoveFirst();
        PoolManager.Destroy(t.gameObject);
        firstTileZPos = roadTiles.First.Value.position.z;
    }
    GameObject SpawnTileAtPosition(Vector3 position, GameObject tile = null)
    {
        if (tile == null)
        {
            return PoolManager.Instantiate(tiles.Random().gameObject, position);
        }
        else
            return PoolManager.Instantiate(tile, position);
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Total distance", currentTile);
    }
    public static int SmartRandom(float[] normalizedProbabilities)
    {
        var f = Random.Range(0, normalizedProbabilities[normalizedProbabilities.Length - 1]);
        for (int i = 0; i < normalizedProbabilities.Length; i++)
        {
            if (normalizedProbabilities[i] >= f)
                return i;
        }
        return normalizedProbabilities.Length - 1;
    }
    async void OnLevelEnd()
    {
        onLevelEndEvents?.Invoke();
        var op = SceneManager.LoadSceneAsync((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
        op.allowSceneActivation = false;
        await Task.Delay(1750);
        PlayerPrefs.SetInt("Total distance", currentTile);
        op.allowSceneActivation = true;
    }
}
