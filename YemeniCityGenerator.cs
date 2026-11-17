using UnityEngine;

/// <summary>
/// Generates a lightweight placeholder city on a 2000m x 2000m map.
/// Replace primitive meshes with optimized Yemeni art assets during production.
/// </summary>
public class YemeniCityGenerator : MonoBehaviour
{
    [Header("World dimensions")]
    public float worldSize = 2000f;
    public float roadWidth = 12f;
    public float blockSize = 100f;
    public float roadHeight = 0.15f;

    [Header("Generation")]
    public bool generateOnStart = true;
    public int randomSeed = 2026;
    [Range(0f, 1f)] public float buildingDensity = 0.72f;

    [Header("Materials (optional)")]
    public Material groundMaterial;
    public Material roadMaterial;
    public Material stoneMaterial;
    public Material whitePlasterMaterial;

    private Transform generatedRoot;

    private void Start()
    {
        if (generateOnStart) GenerateCity();
    }

    [ContextMenu("Generate City")]
    public void GenerateCity()
    {
        ClearCity();
        Random.InitState(randomSeed);

        generatedRoot = new GameObject("Generated_Yemeni_City").transform;
        generatedRoot.SetParent(transform, false);

        CreateCube("Terrain", new Vector3(0, -0.6f, 0),
            new Vector3(worldSize, 1f, worldSize), groundMaterial, generatedRoot);

        // Main roads: grid layout, with a broad central avenue.
        int blocks = Mathf.FloorToInt(worldSize / blockSize);
        for (int i = 0; i <= blocks; i++)
        {
            float p = -worldSize * 0.5f + i * blockSize;
            CreateCube("Road_EW_" + i, new Vector3(0, roadHeight, p),
                new Vector3(worldSize, 0.25f, roadWidth), roadMaterial, generatedRoot);
            CreateCube("Road_NS_" + i, new Vector3(p, roadHeight, 0),
                new Vector3(roadWidth, 0.25f, worldSize), roadMaterial, generatedRoot);
        }

        // Build lots between roads. Leave occasional open squares for markets/plazas.
        for (int x = 0; x < blocks; x++)
        {
            for (int z = 0; z < blocks; z++)
            {
                if (Random.value > buildingDensity) continue;

                float cx = -worldSize * 0.5f + (x + 0.5f) * blockSize;
                float cz = -worldSize * 0.5f + (z + 0.5f) * blockSize;

                // A few larger open spaces create room for souqs and public squares.
                if (Random.value < 0.06f) continue;

                int count = Random.Range(2, 6);
                for (int b = 0; b < count; b++)
                {
                    float w = Random.Range(12f, 27f);
                    float d = Random.Range(12f, 25f);
                    float h = Random.Range(9f, 24f);
                    float ox = Random.Range(-34f, 34f);
                    float oz = Random.Range(-34f, 34f);
                    Vector3 pos = new Vector3(cx + ox, h * 0.5f, cz + oz);

                    Material mat = Random.value < 0.5f ? stoneMaterial : whitePlasterMaterial;
                    GameObject house = CreateCube("Yemeni_House", pos,
                        new Vector3(w, h, d), mat, generatedRoot);

                    // Distinctive stepped rooftop silhouette, a simple placeholder motif.
                    if (Random.value < 0.65f)
                    {
                        float topH = Random.Range(1.2f, 2.5f);
                        CreateCube("Roof_Parapet", pos + Vector3.up * (h * 0.5f + topH * 0.5f),
                            new Vector3(w * 0.82f, topH, d * 0.82f), mat, generatedRoot);
                    }
                    house.transform.rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0);
                }
            }
        }

        // A central landmark placeholder: a raised market courtyard.
        CreateCube("Central_Souq_Plaza", new Vector3(0, 0.1f, 0),
            new Vector3(58f, 0.3f, 58f), roadMaterial, generatedRoot);
    }

    private GameObject CreateCube(string objectName, Vector3 position, Vector3 scale,
        Material material, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = objectName;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        if (material != null) go.GetComponent<Renderer>().sharedMaterial = material;
        return go;
    }

    private void ClearCity()
    {
        Transform old = transform.Find("Generated_Yemeni_City");
        if (old != null)
        {
            if (Application.isPlaying) Destroy(old.gameObject);
            else DestroyImmediate(old.gameObject);
        }
    }
}
