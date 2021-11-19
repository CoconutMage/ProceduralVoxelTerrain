using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TerrainSettings
{
    //Declare variables used in the terrian generator
    private static TerrainSettings _instance = null;

    public int mapSize;
    public float frequencyMultiplier;
    public float amplitudeMultiplier;
    public float xOffSet;
    public float yOffSet;
    public int seed;
    public bool keepSeed;
    public GameObject[,] entityPrefabs;
    public int lacunarity;
    public float persistance;
    public int octave;
    public float treeFreq;
    public float coalFreq;
    public float ironFreq;
    public float goldFreq;
    public float silverFreq;
    public float crystalFreq;
    public float rockFreq;
    public float waterHeight;
    public bool waterEnabled;
    public GameObject chunkPrefab;
    public Vector2[,,] textures;
    int atlasSize;
    public bool firstLoad;
    public Vector2[,] blockIDs;
    int numUniqueBlocks;
    int numEntities;
    int[] entityIdList;
    public bool customPlayer;
    public GameObject player;
    public GameObject UIPrefab;
    int entityVariants;
    public int biomeSelected;
    BiomeDict biomeDict;
    public int treeID;
    public int[] layerDepth;
    public int[] layerId;
    public int numLayers;
    public float riverFreq;

    private TerrainSettings()
    {
        biomeDict = BiomeDict.getInstance();

        //Map size in chunks. Default is 6;
        //Each chunk is 16 blocks by 16 blocks
        mapSize = 6;

        //How frequently hills are gnerated. Default is 1.5f;
        //Low values create very flat terrain while high values generate very hilly
        frequencyMultiplier = 1.5f;

        //How high the hills are and how low the valleys are. Default is 10;
        amplitudeMultiplier = 13;

        //How much smaller detail each layer of noise adds. Default is 2;
        lacunarity = 2;

        //How much smaller the details of each layer adds. Default is .5f;
        persistance = 0.5f;

        //How many layers of noise. Default is 3;
        octave = 3;

        //Percent chance for each block to have this feature
        //default is 2;
        treeFreq = 2;
        //default is 6;
        coalFreq = 6;
        //default is 4;
        ironFreq = 4;
        //default is 2;
        goldFreq = 2;
        //default is 3;
        silverFreq = 3;
        //default is 1;
        crystalFreq = 1;
        //default is 1;
        rockFreq = 1;

        //How many blocks high from the lowest point on the map water should rise. Default is 1;
        waterHeight = 1;

        //Wether or not to spawn the custom player and sliders
        customPlayer = false;

        //Size of texture atlas in squares of unique texture
        atlasSize = 4;
        
        //Number of unique blocks in game
        numUniqueBlocks = 14;

        //Number of unique entities, and the number of variations for each entity
        numEntities = 4;
        entityVariants = 3;
        
        //Wether or not to create a new world seed on load
        keepSeed = true;

        //Wether or not to generate water on the map
        waterEnabled = false;

        //World seed
        seed = 0;

        //These arent settings, but just initializing variables for later use in scripts. Editing them will break other functions
        firstLoad = true;
        //layerId = new int[] { 1, 12, 2 };
        entityPrefabs = new GameObject[numEntities, entityVariants];
        entityIdList = new int[numEntities];
        blockIDs = new Vector2[numUniqueBlocks, 6];
        textures = new Vector2[atlasSize, atlasSize, 4];
        ImportTextureAtlas();
        LoadEntities();
        BlockIDInfo();
    }
    public static TerrainSettings getInstance()
    {
        //This code allows for there to only ever be one instance of this script as to not have duplicate settings files
        //If a script tries to create another one it will just be given this script
        //Files of this nature are called Singletons
        if (_instance == null) _instance = new TerrainSettings();
        return _instance;
    }
    private void LoadEntities()
    {
        //This loads all prefabs required for the terrain generator
        //Names of prefabs and directories must all be within the resources folder
        chunkPrefab = Resources.Load("Chunk", typeof(GameObject)) as GameObject;
        player = Resources.Load("Main Camera", typeof(GameObject)) as GameObject;
        UIPrefab = Resources.Load("Canvas", typeof(GameObject)) as GameObject;

        //Gets all entity prefabs and variants
        //Add lines here if you want to add more entities to the game
        entityPrefabs[0, 0] = Resources.Load("OakTree_1", typeof(GameObject)) as GameObject;
        entityPrefabs[0, 1] = Resources.Load("OakTree_2", typeof(GameObject)) as GameObject;
        entityPrefabs[0, 2] = Resources.Load("OakTree_3", typeof(GameObject)) as GameObject;
        entityPrefabs[1, 0] = Resources.Load("BirchTree_1", typeof(GameObject)) as GameObject;
        entityPrefabs[1, 1] = Resources.Load("BirchTree_2", typeof(GameObject)) as GameObject;
        entityPrefabs[1, 2] = Resources.Load("BirchTree_3", typeof(GameObject)) as GameObject;
        entityPrefabs[2, 0] = Resources.Load("Rock_1", typeof(GameObject)) as GameObject;
        entityPrefabs[2, 1] = Resources.Load("Rock_2", typeof(GameObject)) as GameObject;
        entityPrefabs[2, 2] = Resources.Load("Rock_3", typeof(GameObject)) as GameObject;
        entityPrefabs[3, 0] = Resources.Load("DesertTree_1", typeof(GameObject)) as GameObject;
        entityPrefabs[3, 1] = Resources.Load("DesertTree_2", typeof(GameObject)) as GameObject;
        entityPrefabs[3, 2] = Resources.Load("DesertTree_3", typeof(GameObject)) as GameObject;

        //Declares what the block id of a prefab is relative to its position in the prefab array
        entityIdList[0] = 3;
        entityIdList[1] = 9;
        entityIdList[2] = 10;
        entityIdList[3] = 14;
    }
    private void ImportTextureAtlas()
    {
        //Divides the texture atlas into individual cubes and stores the location of each texture
        float tileSize = 1.00f / atlasSize;
        
        for (int i = 0; i < atlasSize; i++)
        {
            for (int j = 0; j < atlasSize; j++)
            {
                textures[i, j, 0] = new Vector2(i * tileSize, j * tileSize);
                textures[i, j, 1] = new Vector2(i * tileSize, j * tileSize + tileSize);
                textures[i, j, 2] = new Vector2(i * tileSize + tileSize, j * tileSize + tileSize);
                textures[i, j, 3] = new Vector2(i * tileSize + tileSize, j * tileSize);
            }
        }
    }
    private void BlockIDInfo()
    {
        //Tells what textures should be applied to each block based on id
        //The first integer in the array is the block id minus one
        //The second integer in the array is face of the cube. 0 is top. 1 is left side. 2 is front side. 3 is right side. 4 is back side. 5 is the bottom
        //This allows for blocks to have different textures on each face
        //Add code here to add new blocks
        blockIDs[0, 0] = new Vector2(2, 3);
        blockIDs[0, 1] = new Vector2(2, 1);
        blockIDs[0, 2] = new Vector2(2, 1);
        blockIDs[0, 3] = new Vector2(2, 1);
        blockIDs[0, 4] = new Vector2(2, 1);
        blockIDs[0, 5] = new Vector2(1, 3);

        blockIDs[1, 0] = new Vector2(3, 3);
        blockIDs[1, 1] = new Vector2(3, 3);
        blockIDs[1, 2] = new Vector2(3, 3);
        blockIDs[1, 3] = new Vector2(3, 3);
        blockIDs[1, 4] = new Vector2(3, 3);
        blockIDs[1, 5] = new Vector2(3, 3);

        //Block id's with only 3 lines are entities and the values of the vectors are used to offset prefabs on instantiation to counter models that arent on center
        //The first value is the x offset. The second value is the y offset
        blockIDs[2, 0] = new Vector2(0, 0);
        blockIDs[2, 1] = new Vector2(0, 0);
        blockIDs[2, 2] = new Vector2(0, 0);

        blockIDs[3, 0] = new Vector2(1, 2);
        blockIDs[3, 1] = new Vector2(1, 2);
        blockIDs[3, 2] = new Vector2(1, 2);
        blockIDs[3, 3] = new Vector2(1, 2);
        blockIDs[3, 4] = new Vector2(1, 2);
        blockIDs[3, 5] = new Vector2(1, 2);

        blockIDs[4, 0] = new Vector2(2, 2);
        blockIDs[4, 1] = new Vector2(2, 2);
        blockIDs[4, 2] = new Vector2(2, 2);
        blockIDs[4, 3] = new Vector2(2, 2);
        blockIDs[4, 4] = new Vector2(2, 2);
        blockIDs[4, 5] = new Vector2(2, 2);

        blockIDs[5, 0] = new Vector2(0, 1);
        blockIDs[5, 1] = new Vector2(0, 1);
        blockIDs[5, 2] = new Vector2(0, 1);
        blockIDs[5, 3] = new Vector2(0, 1);
        blockIDs[5, 4] = new Vector2(0, 1);
        blockIDs[5, 5] = new Vector2(0, 1);

        blockIDs[6, 0] = new Vector2(3, 2);
        blockIDs[6, 1] = new Vector2(3, 2);
        blockIDs[6, 2] = new Vector2(3, 2);
        blockIDs[6, 3] = new Vector2(3, 2);
        blockIDs[6, 4] = new Vector2(3, 2);
        blockIDs[6, 5] = new Vector2(3, 2);

        blockIDs[7, 0] = new Vector2(1, 1);
        blockIDs[7, 1] = new Vector2(1, 1);
        blockIDs[7, 2] = new Vector2(1, 1);
        blockIDs[7, 3] = new Vector2(1, 1);
        blockIDs[7, 4] = new Vector2(1, 1);
        blockIDs[7, 5] = new Vector2(1, 1);

        blockIDs[8, 0] = new Vector2(1, 0);
        blockIDs[8, 1] = new Vector2(1, 0);
        blockIDs[8, 2] = new Vector2(1, 1.031f);

        blockIDs[9, 0] = new Vector2(2, .01f);
        blockIDs[9, 1] = new Vector2(2, .04f);
        blockIDs[9, 2] = new Vector2(2, .045f);

        blockIDs[10, 0] = new Vector2(0, 3);
        blockIDs[10, 1] = new Vector2(0, 3);
        blockIDs[10, 2] = new Vector2(0, 3);
        blockIDs[10, 3] = new Vector2(0, 3);
        blockIDs[10, 4] = new Vector2(0, 3);
        blockIDs[10, 5] = new Vector2(0, 3);

        //Dirt: 12
        blockIDs[11, 0] = new Vector2(1, 3);
        blockIDs[11, 1] = new Vector2(1, 3);
        blockIDs[11, 2] = new Vector2(1, 3);
        blockIDs[11, 3] = new Vector2(1, 3);
        blockIDs[11, 4] = new Vector2(1, 3);
        blockIDs[11, 5] = new Vector2(1, 3);

        //Sand: 13
        blockIDs[12, 0] = new Vector2(0, 2);
        blockIDs[12, 1] = new Vector2(0, 2);
        blockIDs[12, 2] = new Vector2(0, 2);
        blockIDs[12, 3] = new Vector2(0, 2);
        blockIDs[12, 4] = new Vector2(0, 2);
        blockIDs[12, 5] = new Vector2(0, 2);

        //Desert Tree: 14
        blockIDs[13, 0] = new Vector2(3, 0);
        blockIDs[13, 1] = new Vector2(3, 0);
        blockIDs[13, 2] = new Vector2(3, 0);
    }
    //Checks if a given block id is a prefab
    public bool IsEntity(int id)
    {
        for (int i = 0; i < entityIdList.Length; i++)
        {
            if (id == entityIdList[i]) return true;
        }
        return false;
    }
    public bool BreakableBlock(int id)
    {
        if (id == 11)
        {
            return false;
        }
        return true;
    }
    public void SetBiomeSettings(Biome biome)
    {
        treeID = biome.treeType;
        amplitudeMultiplier = biome.amplitude;
        frequencyMultiplier = biome.frequency;
        numLayers = biome.layers;
        layerDepth = biome.layerDepth;
        layerId = biome.layerId;
        treeFreq = biome.treeFreq;
        riverFreq = biome.riverFreq;
    }
}