using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTextureData
{
    public Texture2D terrainTexture;

    public Vector2 tileSize;

    public float min;

    public float max;

}

public class TreeData
{
    public GameObject treeMesh;

    public float min;

    public float max;

}

public class TerrainRandom : MonoBehaviour
{

    private Terrain terrain;
    
    private TerrainData terrainData;
    private float perlinNoiseWidthScale = 0.01f;
    private float perlinNoiseHeightScale = 0.01f;
    private float min = 0f;
    private float max = 0.01f;
    public List<TerrainTextureData> terrainTextureData = new List<TerrainTextureData>();
    private float terrainTextureBlendOffset = 0.01f;
    public List<TreeData> treeData= new List<TreeData>();
    int maxTrees = 10000;
    int treeSpacing = 10;
    int terrainLayerIndex=8;
    GameObject water;
    float waterHeight=0.25f;
    GameObject cloud;
    float cloudHeight = 1f;
    GameObject steam;
    GameObject player;
    GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        if (terrain==null)
        { terrain = this.GetComponent<Terrain>(); }
        perlinNoiseWidthScale = Random.Range(0.005f, 0.01f);
        perlinNoiseHeightScale = Random.Range(0.005f, 0.01f);
        if (terrainData == null)
        { terrainData = Terrain.activeTerrain.terrainData; }
        GenerateHeights();
        AddTerrainTextures();
        AddTrees();
        AddWater();
        AddClouds();
        AddSteam();
        AddPlayer();
    }
    void GenerateHeights() {
        float[,] heightMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];

        for (int width = 0; width < terrainData.heightmapResolution; width++)
        {
            for (int height=0; height<terrainData.heightmapResolution;height++)
            {
                heightMap[width, height] = Random.Range(min,max);
                heightMap[width, height] += Mathf.PerlinNoise(width * perlinNoiseWidthScale, height * perlinNoiseHeightScale);
            }
        }
        terrainData.SetHeights(0,0,heightMap);
    }

    void AddTerrainTextures()
    {
        GetTextures();
        TerrainLayer[] terrainLayers = new TerrainLayer[terrainTextureData.Count];
        for (int i = 0; i < terrainTextureData.Count; i++)
        {
            Debug.Log(terrainTextureData[i]);
            terrainLayers[i] = new TerrainLayer();
            terrainLayers[i].diffuseTexture = terrainTextureData[i].terrainTexture;
            terrainLayers[i].tileSize = terrainTextureData[i].tileSize;
        }
        terrainData.terrainLayers = terrainLayers;

        float[,] heightMap = terrainData.GetHeights(0,0,terrainData.heightmapResolution, terrainData.heightmapResolution);
        float[, , ] alphamapList = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int height =0; height < terrainData.alphamapHeight; height++)
        {
            for (int width = 0; width < terrainData.alphamapWidth; width++)
            {

                float[] alphamap = new float[terrainData.alphamapLayers];
                for (int i = 0;i < terrainTextureData.Count; i++)
                {

                    float heightBegin = terrainTextureData[i].min - terrainTextureBlendOffset;
                    float heightend = terrainTextureData[i].max + terrainTextureBlendOffset;

                    if ((heightMap[width, height] >= heightBegin) && (heightMap[width, height] <= heightend))
                    {
                        alphamap[i] = 1;
                    }
                }
                Blend(alphamap);
                for (int j = 0; j < terrainTextureData.Count; j++)
                {
                    alphamapList[width, height, j] = alphamap[j];
                }
            }
        }
        terrainData.SetAlphamaps(0,0,alphamapList);
    }

    void Blend(float[] alphamap)
    {
        float total = 0;
        for (int i = 0; i < alphamap.Length; i++)
        {
            total += alphamap[i];
        }
        for (int i = 0; i<alphamap.Length;i++)
        {
            alphamap[i] = alphamap[i] / total;
        }
    }

    void GetTextures()
    {
        TerrainTextureData snow = new TerrainTextureData();
        snow.terrainTexture = Resources.Load("Textures/snow") as Texture2D;
        snow.tileSize = new Vector2(20, 20);
        snow.min = 0.8f;
        snow.max = 1f;
        terrainTextureData.Add(snow);
        TerrainTextureData albedo = new TerrainTextureData();
        albedo.terrainTexture = Resources.Load("Textures/albedo") as Texture2D;
        albedo.tileSize = new Vector2(20, 20);
        albedo.min = 0.5f;
        albedo.max = 0.8f;
        terrainTextureData.Add(albedo);
        TerrainTextureData moss = new TerrainTextureData();
        moss.terrainTexture = Resources.Load("Textures/moss") as Texture2D;
        moss.tileSize = new Vector2(20, 20);
        moss.min = 0.25f;
        moss.max = 0.5f;
        terrainTextureData.Add(moss);
        TerrainTextureData waterT = new TerrainTextureData();
        waterT.terrainTexture = Resources.Load("Textures/water") as Texture2D;
        waterT.tileSize = new Vector2(20, 20);
        waterT.min = 0f;
        waterT.max = 0.25f;
        terrainTextureData.Add(waterT);
    }

    private void AddTrees()
    {
        GetTree();
        TreePrototype[] trees = new TreePrototype[treeData.Count];

        for (int i = 0; i< treeData.Count; i++)
        {
            trees[i] = new TreePrototype();
            trees[i].prefab = treeData[i].treeMesh;
        }

        terrainData.treePrototypes = trees;

        List<TreeInstance> treeInstanceList = new List<TreeInstance>();

        for(int z = 0; z< terrainData.size.z; z+=treeSpacing)
        {
            for (int x = 0; x<terrainData.size.x;x+= treeSpacing)
            {
                for (int treeIndex = 0; treeIndex < trees.Length; treeIndex++)
                {
                    if (treeInstanceList.Count<maxTrees)
                    {
                        float currentHeight = terrainData.GetHeight(x, z) / terrainData.size.y;
                        if (currentHeight >= treeData[treeIndex].min && currentHeight <= treeData[treeIndex].max)
                        {
                            float randomX = (x + Random.Range(-5.0f, 5.0f)) / terrainData.size.x;
                            float randomZ = (z + Random.Range(-5.0f, 5.0f)) / terrainData.size.z;

                            Vector3 treePosition = new Vector3(randomX * terrainData.size.x, currentHeight * terrainData.size.y, randomZ * terrainData.size.z) + this.transform.position;
                            RaycastHit raycastHit;
                            int layerMask = 1 << terrainLayerIndex;
                            if (Physics.Raycast(treePosition, -Vector3.up, out raycastHit, 100, layerMask) || 
                                Physics.Raycast(treePosition, Vector3.up, out raycastHit, 100, layerMask))
                            {
                                float treeDistance = (raycastHit.point.y - this.transform.position.y) / terrainData.size.y;
                                TreeInstance treeInstance = new TreeInstance();
                                treeInstance.position = new Vector3(randomX, treeDistance, randomZ);
                                treeInstance.rotation = Random.Range(0, 360);
                                treeInstance.prototypeIndex = treeIndex;
                                treeInstance.color = Color.white;
                                treeInstance.lightmapColor = Color.white;
                                treeInstance.heightScale = 0.95f;
                                treeInstance.widthScale = 0.95f;
                                treeInstanceList.Add(treeInstance);
                            }
                        }
                    }
                }
            }
        }
        terrainData.treeInstances = treeInstanceList.ToArray();
    }

    void GetTree()
    {
        TreeData Tsnow = new TreeData();
        Tsnow.treeMesh = Resources.Load("Prefabs/Tree9_2") as GameObject;
        Tsnow.min = 0.7f;
        Tsnow.max = 1f;
        treeData.Add(Tsnow);
        TreeData Talbedo = new TreeData();
        Talbedo.treeMesh = Resources.Load("Prefabs/Tree9_3") as GameObject;
        Talbedo.min = 0.5f;
        Talbedo.max = 0.7f;
        treeData.Add(Talbedo);
        TreeData Tmoss = new TreeData();
        Tmoss.treeMesh = Resources.Load("Prefabs/Tree9_4") as GameObject;
        Tmoss.min = 0.3f;
        Tmoss.max = 0.5f;
        treeData.Add(Tmoss);
    }

    void AddWater()
    {
        water = Resources.Load("Prefabs/Water") as GameObject;
        GameObject waterGameObject = Instantiate(water,this.transform.position, this.transform.rotation);
        waterGameObject.name = "Water";
        waterGameObject.transform.position = this.transform.position+new Vector3(terrainData.size.x/2,waterHeight * terrainData.size.y,terrainData.size.z/2);
        waterGameObject.transform.localScale = new Vector3(terrainData.size.x, 1, terrainData.size.z);
    }
    void AddClouds()
    {
        Debug.Log("YES");
        cloud = Resources.Load("Prefabs/Cloud") as GameObject;
        GameObject cloudGameObject = Instantiate(cloud, this.transform.position, this.transform.rotation);
        cloudGameObject.name = "Cloud";
        cloudGameObject.transform.position = this.transform.position + new Vector3(terrainData.size.x / 2, cloudHeight * terrainData.size.y, terrainData.size.z / 2);
        cloudGameObject.transform.localScale = new Vector3(terrainData.size.x/2, 1, terrainData.size.z/2);
    }
    void AddSteam()
    {
        steam = Resources.Load("Prefabs/Steam") as GameObject;
        GameObject steamGameObject = Instantiate(steam, this.transform.position, this.transform.rotation);
        steamGameObject.name = "Steam";
        steamGameObject.transform.position = this.transform.position + new Vector3(terrainData.size.x / 2, 0.25f * terrainData.size.y, terrainData.size.z / 2);
        steamGameObject.transform.localScale = new Vector3(terrainData.size.x / 2, 3, terrainData.size.z / 2);
    }
    void AddPlayer()
    {
        player = Resources.Load("Prefabs/Player") as GameObject;
        camera = Resources.Load("Prefabs/Camera") as GameObject;
        GameObject playerGameObject = Instantiate(player, new Vector3(Random.Range(250,750), 1f * terrainData.size.y, Random.Range(250, 750)), this.transform.rotation);
        GameObject camGameObject = Instantiate(camera, this.transform.position, this.transform.rotation);
    }
}
