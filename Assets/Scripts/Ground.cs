using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Ground : MonoBehaviour
{
    public Material[] groundMaterials;
    public GameObject[] prefabs;
    Material selectedGroundMaterial;
    public Vector3 offset;
    Transform player;
    LinkedList<Transform> groundTiles = new LinkedList<Transform>();
    Dictionary<Transform, List<GameObject>> propsOnParticularTile = new Dictionary<Transform, List<GameObject>>();
    public float tileSize;
    public int maximumNumberOfTiles = 2;
    int currentTile;
    public static Ground instance;

    public Material vertexLitMaterial;

    Dictionary<Mesh, Vector3> meshesForWaterEffect = new Dictionary<Mesh, Vector3>();

    [Header("Animals")]
    public GameObject[] animals;
    public int maxAnimalCount;
    public int minAnimalCount;
    [Header("Obstacles")]
    public GameObject grass;
    public int grassSqrtCount;
    public GameObject[] obstacles;
    public int maxCount;
    public int minCount;
    [Header("Water effect")]
    public bool isWater;
    public float noiseScale;
    public float maxHeight;

    void Awake()
    {
        selectedGroundMaterial = groundMaterials[Random.Range(0, groundMaterials.Length)];
#if !UNITY_EDITOR
        RenderSettings.skybox = selectedGroundMaterial;
#endif
        instance = this;
        for (int i = 0; i < maximumNumberOfTiles; i++)
        {
            PrepareTile();
        }
    }
    private void Start()
    {
        player = PlayerController.instance.transform;
    }
    void PrepareTile()
    {
        currentTile++;
        SpawnTileAtLastPosition();
        var list = SpawnPropsAtLastTile(obstacles, minCount, maxCount);
        if (grass)
        {
            var cornerPos = groundTiles.Last.Value.position;
            cornerPos -= new Vector3(tileSize / 2, 0, tileSize / 2);
            var grassTileSize = tileSize / grassSqrtCount;
            for (int i = 0; i < grassSqrtCount; i++)
                for (int j = 0; j < grassSqrtCount; j++)
                {
                    var grassInstance = PoolManager.Instantiate(grass, cornerPos + new Vector3(grassTileSize * (i + 0.5f), 0, grassTileSize * (j + 0.5f)));
                    list.Add(grassInstance);
                }
        }
        SpawnAdditionalAtLastTile();
        SpawnPropsAtLastTile(animals, minAnimalCount, maxAnimalCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (groundTiles.Last.Value.position.z <= player.position.z)
        {
            DeleteFirstTile();
            PrepareTile();
        }
        if (isWater)
        {
            foreach (var mesh in meshesForWaterEffect)
                SetVertices(mesh.Key, mesh.Value + Time.time * Vector3.one);
        }
    }
    public static void DeleteObstacle(GameObject requestedObject)
    {
        var objectZPos = requestedObject.transform.position.z;
        Transform selectedTile = null;
        foreach (var tile in instance.groundTiles)
            if (Mathf.Abs(tile.transform.position.z - objectZPos) < instance.tileSize / 2)
            {
                selectedTile = tile;
                break;
            }

        if (selectedTile)
        {
            instance.propsOnParticularTile.TryGetValue(selectedTile, out var list);
            foreach (var i in list)
                if (i == requestedObject)
                {
                    list.Remove(i);
                    PoolManager.Destroy(i);
                    return;
                }
        }
    }
    List<GameObject> SpawnPropsAtLastTile(GameObject[] props, int minCount, int maxCount)
    {
        Vector3 dir = Vector3.zero;
        var lastTilePos = groundTiles.Last.Value.position;
        if (!propsOnParticularTile.TryGetValue(groundTiles.Last.Value, out var list))
        {
            list = new List<GameObject>();
            propsOnParticularTile.Add(groundTiles.Last.Value, list);
        }
        for (int i = 0; i < (props.Length < 1 ? 0 : Random.Range(minCount, maxCount + 1)); i++)
        {
            dir.x = Random.Range(-1.0f, 1.0f);
            dir.z = Random.Range(-1.0f, 1.0f);
            var g = PoolManager.Instantiate(props.Random(),
                lastTilePos + dir * tileSize * 0.5f);
            list.Add(g);
        }
        return list;
    }
    void SpawnTileAtLastPosition()
    {
        Transform t;
        if (groundTiles.Count == 0)
            t = SpawnTileAtPosition(offset).transform;
        else
            t = SpawnTileAtPosition(groundTiles.Last.Value.position + Vector3.forward * tileSize).transform;

        groundTiles.AddLast(t);

        if (!isWater)
            t.gameObject.layer = 13; //Устанавливаем слой в бэкграунд, чтобы запечь меш
    }
    void SpawnAdditionalAtLastTile()
    {
        if (prefabs.Length > 0)
        {
            var t = groundTiles.Last.Value;
            if (!propsOnParticularTile.TryGetValue(t, out var list))
            {
                list = new List<GameObject>();
                propsOnParticularTile.Add(t, list);
            }
            //var a = prefabs.Random();
            var a = prefabs[currentTile % prefabs.Length];
            a = PoolManager.Instantiate(a, t.position);
            list.Add(a);
        }
    }
    public void ClearNoiseAtLastGroundTile(float senderZPos, int[] rows, int[] columns)
    {
        var groundMesh = groundTiles.Last.Value.GetComponent<MeshFilter>();
        groundMesh.mesh = SetVertices(groundMesh.mesh, 0, rows, columns);
    }
    void DeleteFirstTile()
    {
        if (groundTiles.Count <= 1)
            return;

        var t = groundTiles.First.Value;
        groundTiles.RemoveFirst();
        if (propsOnParticularTile.TryGetValue(t, out var list))
        {
            foreach (var i in list)
                PoolManager.Destroy(i);
            propsOnParticularTile.Remove(t);
        }
        meshesForWaterEffect.Remove(t.gameObject.GetComponent<MeshFilter>().mesh);
        Destroy(t.gameObject);
    }
    GameObject SpawnTileAtPosition(Vector3 position)
    {
        var g = new GameObject("Ground tile " + currentTile);
        var newMesh = SetVertices(CreateCustomPlane(), position);
        meshesForWaterEffect.Add(newMesh, position);
        g.AddComponent<MeshFilter>().mesh = newMesh;
        g.transform.position = position;
        g.AddComponent<MeshRenderer>().sharedMaterial = vertexLitMaterial;
        return g;
    }
    Mesh CreateCustomPlane(int size = 10)
    {
        float center = size;
        center /= 2;
        var vertices = new Vector3[size * size * 6];
        var normals = new Vector3[size * size * 6];
        var triangles = new int[size * size * 6];
        var colors = new Color[size * size * 6];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                //работаем в квадратике (i, j)
                for (int k = 0; k < 6; k++)
                {
                    vertices[i * size * 6 + j * 6 + k] = new Vector3(j + (k == 2 || k == 3 || k == 4 ? 1 : 0) - center, 0, i + k % 2 - center) * tileSize / 10;
                    normals[i * size * 6 + j * 6 + k] = new Vector3(0, 1, 0);
                    triangles[i * size * 6 + j * 6 + k] = i * size * 6 + j * 6 + k;
                    colors[i * size * 6 + j * 6 + k] = selectedGroundMaterial.color;
                }
            }
        var mesh = new Mesh()
        {
            vertices = vertices,
            normals = normals,
            triangles = triangles,
            colors = colors,
        };
        return mesh;
    }
    Mesh SetVertices(Mesh mesh, Vector3 offset)
    {
        var vertices = mesh.vertices;
        Vector3 v;

        for (int i = 0; i < vertices.Length; i++)
        {
            v = vertices[i];
            vertices[i].y = Mathf.PerlinNoise((v.x + offset.x) * noiseScale, (v.z + offset.z) * noiseScale) * maxHeight;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        return mesh;
    }
    Mesh SetVertices(Mesh mesh, float y, int[] rows, int[] columns)
    {
        var vertices = mesh.vertices;

        foreach (var line in rows)
        {
            Debug.Log("Requested line: " + line);
            if (line < 0 || line > 9)
                continue;

            foreach (int col in columns)
            {
                Debug.Log("Requested column: " + col);
                if (col < 0 || col > 9)
                    continue;

                if (line > 0)
                {
                    vertices[(line - 1) * 10 * 6 + col * 6 + 1].y = y;
                    vertices[(line - 1) * 10 * 6 + col * 6 + 3].y = y;
                    vertices[(line - 1) * 10 * 6 + col * 6 + 5].y = y;
                }

                if (line < 9)
                {
                    vertices[(line + 1) * 10 * 6 + col * 6 + 0].y = y;
                    vertices[(line + 1) * 10 * 6 + col * 6 + 2].y = y;
                    vertices[(line + 1) * 10 * 6 + col * 6 + 4].y = y;
                }


                vertices[line * 10 * 6 + col * 6 + 0].y = y;
                vertices[line * 10 * 6 + col * 6 + 1].y = y;
                vertices[line * 10 * 6 + col * 6 + 2].y = y;
                vertices[line * 10 * 6 + col * 6 + 3].y = y;
                vertices[line * 10 * 6 + col * 6 + 4].y = y;
                vertices[line * 10 * 6 + col * 6 + 5].y = y;
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        return mesh;
    }
}
