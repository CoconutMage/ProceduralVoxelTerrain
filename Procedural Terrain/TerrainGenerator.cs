using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class TerrainGenerator : MonoBehaviour
{
    int mapSize;
    float[,] heightMap;
    int[,,] blockIDMap;
    float frequencyMultiplier;
    float amplitudeMultiplier;
    float xOffSet;
    float yOffSet;
    int lowestPoint;
    int highestPoint;
    Vector3 lowestCoord;
    Vector3 highestCoord;
    Vector3 riverEnterCoord;
    GameObject[,] chunks;
    Vector3 pos;
    Quaternion rot;
    TerrainSettings ts;
    public GameObject chunkPrefab;

    void Start()
    {
        //Creates the singleton or gets data from it if its already been created
        ts = TerrainSettings.getInstance();
        BiomeDict biomeDict = BiomeDict.getInstance();
        
        chunkPrefab = ts.chunkPrefab;

        //If this is the first time loading and the custom player is enabled instantiates them
        if (ts.firstLoad && ts.customPlayer)
        {
            pos = new Vector3(50, 36, -20);
            Instantiate(ts.player, pos, rot);
            GameObject uiObject = Instantiate(ts.UIPrefab, pos, rot);
            uiObject.name = "Canvas";
            ts.firstLoad = false;
        }
        else if (ts.firstLoad)
        {
            ts.SetBiomeSettings(biomeDict.biomes[0]);
            ts.firstLoad = false;
        }

        //Gets data from the singleton
        mapSize = ts.mapSize;
        frequencyMultiplier = ts.frequencyMultiplier;
        amplitudeMultiplier = ts.amplitudeMultiplier;

        //Loads world seed from singleton or generates new one if world seed is empty
        if (ts.seed == 0) ts.seed = System.DateTime.Now.Millisecond * this.GetInstanceID();
        //Applies world seed to the random number generator
        Random.InitState(ts.seed);

        //Declares array of block data based on the size of the map
        heightMap = new float[mapSize * 16, mapSize * 16];
        blockIDMap = new int[mapSize * 16, 100, mapSize * 16];
        lowestPoint = 99999;
        highestPoint = -1;
        lowestCoord = new Vector3();
        riverEnterCoord = new Vector3();
        highestCoord = new Vector3();
        chunks = new GameObject[mapSize, mapSize];
        rot = new Quaternion(-1, 0, 0, 1);

        //Picks random place in function to generate the height map for the terrain
        xOffSet = Random.Range(0, 99999);
        yOffSet = Random.Range(0, 99999);

        GenTerrainMap();
        if (ts.riverFreq >= Random.Range(0.0f, 100.0f)) GenRiver(riverEnterCoord);
        if (ts.waterEnabled) GenWater();
        else GenBlockChunks();
        GenEntities();
    }
    void GenTerrainMap()
    {
        float yPos;

        for (float i = 0; i < mapSize * 16; i++)
        {
            for (float j = 0; j < mapSize * 16; j++)
            {
                for (int k = 0; k < ts.octave; k++)
                {
                    //Gets height map data from the Perlin Noise function used to generate the terrain
                    heightMap[(int)i, (int)j] += (Mathf.PerlinNoise(xOffSet + i / (mapSize * 16) * frequencyMultiplier * (Mathf.Pow(ts.lacunarity, k)), yOffSet + j / (mapSize * 16) * frequencyMultiplier * (Mathf.Pow(ts.lacunarity, k)))) * Mathf.Pow(ts.persistance, k);
                }
                
                //Uses amplitude setting and height map to generate block data
                pos = new Vector3(i, (int)(amplitudeMultiplier * heightMap[(int)i, (int)j]), j);
                pos.y += 30;
                heightMap[(int)i, (int)j] = pos.y;

                if (pos.y > highestPoint && (pos.z == (mapSize * 16) - 1 || pos.x == (mapSize * 16) - 1 || pos.z == 0 || pos.x == 0))
                {
                    highestPoint = (int)pos.y;
                    highestCoord = pos;
                }
                else if (pos.y < lowestPoint && (pos.z == (mapSize * 16) - 1 || pos.x == (mapSize * 16) - 1 || pos.z == 0 || pos.x == 0))
                {
                    lowestPoint = (int)pos.y;
                    lowestCoord = pos;
                }

                yPos = pos.y;
                
                for (int layers = 0; layers < ts.numLayers; layers++)
                {
                    for (int depth = 0; depth < ts.layerDepth[layers]; depth++)
                    {
                        pos = new Vector3(pos.x, yPos, pos.z);
                        blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = ts.layerId[layers];
                        yPos -= 1;
                    }
                    if (ts.layerDepth[layers + 1] == -1)
                    {
                        while (yPos >= 0)
                        {
                            pos = new Vector3(pos.x, yPos, pos.z);
                            if (i != 0 && j != 0 && i != (mapSize * 16) - 1 && j != (mapSize * 16) - 1)
                            {
                                GenOres(pos);
                            }
                            else blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = ts.layerId[layers + 1];
                            yPos -= 1;
                        }
                        break;
                    }
                }

                /*
                //Adds stone/ores to block data
                while (yPos >= 0)
                {
                    pos = new Vector3(pos.x, yPos, pos.z);
                    if (i != 0 && j != 0 && i != (mapSize * 16) - 1 && j != (mapSize * 16) - 1)
                    {
                        GenOres(pos);
                    }
                    else blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = 2;
                    yPos -= 1;
                }
                */
            }
        }

        int x = 0;
        int z = 0;

        if (Random.Range(0, 11) >= 5) x = Random.Range(0, (mapSize * 16) + 1);
        else z = Random.Range(0, (mapSize * 16) + 1);
        riverEnterCoord = new Vector3(x, heightMap[x, z], z); 
    }
    void GenWater()
    {
        //Loops through entire map and adds water to blocks below water height setting
        for (int i = 0; i < ts.waterHeight; i++)
        {
            for (int j = 0; j < mapSize * 16; j++)
            {
                for (int k = 0; k < mapSize * 16; k++)
                {
                    if (blockIDMap[j, lowestPoint + 1 + i, k] == 0 || ts.IsEntity(blockIDMap[j, lowestPoint + 1 + i, k])) blockIDMap[j, lowestPoint + 1 + i, k] = 11;
                }
            }
        }
        GenBlockChunks();
    }
    void GenRiver(Vector3 currentLoc)
    {
        blockIDMap[(int)currentLoc.x, (int)currentLoc.y, (int)currentLoc.z] = 11;
        blockIDMap[(int)currentLoc.x, (int)currentLoc.y - 1, (int)currentLoc.z] = 11;

        if (currentLoc.x != 0.0f) blockIDMap[(int)currentLoc.x - 1, (int)currentLoc.y, (int)currentLoc.z] = 11;
        if (currentLoc.x != 0.0f && blockIDMap[(int)currentLoc.x - 1, (int)currentLoc.y + 1, (int)currentLoc.z] != 0 && blockIDMap[(int)currentLoc.x - 1, (int)currentLoc.y + 1, (int)currentLoc.z] != 11) blockIDMap[(int)currentLoc.x - 1, (int)currentLoc.y + 1, (int)currentLoc.z] = 0;
        if (currentLoc.x != (mapSize * 16) - 1) blockIDMap[(int)currentLoc.x + 1, (int)currentLoc.y, (int)currentLoc.z] = 11;
        if (currentLoc.x != (mapSize * 16) - 1 && blockIDMap[(int)currentLoc.x + 1, (int)currentLoc.y + 1, (int)currentLoc.z] != 0 && blockIDMap[(int)currentLoc.x + 1, (int)currentLoc.y + 1, (int)currentLoc.z] != 11) blockIDMap[(int)currentLoc.x + 1, (int)currentLoc.y + 1, (int)currentLoc.z] = 0;
        if (currentLoc.z != 0.0f) blockIDMap[(int)currentLoc.x, (int)currentLoc.y, (int)currentLoc.z - 1] = 11;
        if (currentLoc.z != 0.0f && blockIDMap[(int)currentLoc.x, (int)currentLoc.y + 1, (int)currentLoc.z - 1] != 0 && blockIDMap[(int)currentLoc.x, (int)currentLoc.y + 1, (int)currentLoc.z - 1] != 11) blockIDMap[(int)currentLoc.x, (int)currentLoc.y + 1, (int)currentLoc.z - 1] = 0;
        if (currentLoc.z != (mapSize * 16) - 1) blockIDMap[(int)currentLoc.x, (int)currentLoc.y, (int)currentLoc.z + 1] = 11;
        if (currentLoc.z != (mapSize * 16) - 1 && blockIDMap[(int)currentLoc.x, (int)currentLoc.y + 1, (int)currentLoc.z + 1] != 0 && blockIDMap[(int)currentLoc.x, (int)currentLoc.y + 1, (int)currentLoc.z + 1] != 11) blockIDMap[(int)currentLoc.x, (int)currentLoc.y + 1, (int)currentLoc.z + 1] = 0;

        if (currentLoc.x > lowestCoord.x && currentLoc.z > lowestCoord.z) GenRiver(new Vector3(currentLoc.x - 1, heightMap[(int)currentLoc.x - 1, (int)currentLoc.z - 1], currentLoc.z - 1));
        else if (currentLoc.x > lowestCoord.x && currentLoc.z < lowestCoord.z) GenRiver(new Vector3(currentLoc.x - 1, heightMap[(int)currentLoc.x - 1, (int)currentLoc.z + 1], currentLoc.z + 1));
        else if (currentLoc.x < lowestCoord.x && currentLoc.z > lowestCoord.z) GenRiver(new Vector3(currentLoc.x + 1, heightMap[(int)currentLoc.x + 1, (int)currentLoc.z - 1], currentLoc.z - 1));
        else if (currentLoc.x < lowestCoord.x && currentLoc.z < lowestCoord.z) GenRiver(new Vector3(currentLoc.x + 1, heightMap[(int)currentLoc.x + 1, (int)currentLoc.z + 1], currentLoc.z + 1));
        else if (currentLoc.x == lowestCoord.x && currentLoc.z < lowestCoord.z) GenRiver(new Vector3(currentLoc.x, heightMap[(int)currentLoc.x, (int)currentLoc.z + 1], currentLoc.z + 1));
        else if (currentLoc.x == lowestCoord.x && currentLoc.z > lowestCoord.z) GenRiver(new Vector3(currentLoc.x, heightMap[(int)currentLoc.x, (int)currentLoc.z - 1], currentLoc.z - 1));
        else if (currentLoc.x > lowestCoord.x && currentLoc.z == lowestCoord.z) GenRiver(new Vector3(currentLoc.x - 1, heightMap[(int)currentLoc.x - 1, (int)currentLoc.z], currentLoc.z));
        else if (currentLoc.x < lowestCoord.x && currentLoc.z == lowestCoord.z) GenRiver(new Vector3(currentLoc.x + 1, heightMap[(int)currentLoc.x + 1, (int)currentLoc.z], currentLoc.z));
    }
    void GenBlockChunks()
    {
        //Loops through block data and generates chunks of 16 blocks using block data to render the map
        rot = new Quaternion(0, 0, 0, 1);
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                //Declares variables used to generate chunks
                Vector3 chunkLowestPoint = new Vector3(0, 99999, 0);
                Vector3 chunkHighestPoint = new Vector3(0, -1, 0);
                Mesh mesh = new Mesh();
                pos = new Vector3(i * 16, 0, j * 16);
                int numTrianglesPerChunk;
                Vector3[] vertices;
                int[] triangles;
                int[] waterTriangles;
                Vector2[] UVs;

                //Loops through block data for a chunk to find the highest and lowest points in the chunk
                for (int x = i * 16; x < (i + 1) * 16; x++)
                {
                    for (int z = j * 16; z < (j + 1) * 16; z++)
                    {
                        if (!ts.waterEnabled)
                        {
                            if (chunkLowestPoint.y > heightMap[x, z]) chunkLowestPoint = new Vector3(x, heightMap[x, z], z);
                            else if (chunkHighestPoint.y < heightMap[x, z]) chunkHighestPoint = new Vector3(x, heightMap[x, z], z);
                        }
                        else
                        {
                            if (chunkLowestPoint.y > heightMap[x, z]) chunkLowestPoint = new Vector3(x, heightMap[x, z], z);
                            else if (chunkHighestPoint.y < heightMap[x, z] + ts.waterHeight) chunkHighestPoint = new Vector3(x, heightMap[x, z] + ts.waterHeight, z);
                        }
                    }
                }
                if (i == 0 || j == 0 || i == mapSize - 1 || j == mapSize - 1)
                {
                    chunkLowestPoint.y = 0;
                }

                //Multiples of 1024 for memory reasons
                vertices = new Vector3[16384];
                UVs = new Vector2[vertices.Length];
                numTrianglesPerChunk = 8192;
                triangles = new int[3 * numTrianglesPerChunk];
                waterTriangles = new int[3 * numTrianglesPerChunk];
                int vertLoc = 0;
                int triangleLoc = 0;
                int waterTriangleLoc = 0;

                //This is lots of math to generate a custom mesh for each chunk
                //Each visible face is drawn and made into one chunk of 16 blocks to reduce strain on the cpu and allow for higher fps on larger maps
                //Drastically increases performance when compared to spawning individual blocks
                for (int y = 0; y <= (int)chunkHighestPoint.y; y++)
                {
                    for (int w = 0; w < 16; w++)
                    {
                        for (int k = 0; k < 16; k++)
                        {
                            if (blockIDMap[k + (i * 16), y, w + (j * 16)] != 0 && ts.IsEntity(blockIDMap[k + (i * 16), y, w + (j * 16)]) == false)
                            {
                                //Uncomment out the rest of the if statement if you dont want water to get less transparent as it gets deeper
                                if (blockIDMap[k + (i * 16), y, w + (j * 16)] == 11)// && blockIDMap[k + (i * 16), y + 1, w + (j * 16)] == 0)
                                {
                                    //Says where verticies for the face will be
                                    vertices[vertLoc] = new Vector3(k, y + 1, w);
                                    vertices[vertLoc + 1] = new Vector3(k, y + 1, w + 1);
                                    vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                                    vertices[vertLoc + 3] = new Vector3(k + 1, y + 1, w);

                                    //Draws the faces
                                    waterTriangles[waterTriangleLoc] = vertLoc;
                                    waterTriangles[waterTriangleLoc + 1] = vertLoc + 1;
                                    waterTriangles[waterTriangleLoc + 2] = vertLoc + 2;
                                    waterTriangles[waterTriangleLoc + 3] = vertLoc;
                                    waterTriangles[waterTriangleLoc + 4] = vertLoc + 2;
                                    waterTriangles[waterTriangleLoc + 5] = vertLoc + 3;

                                    //Assigns the texture for each vertice
                                    UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                                    UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                                    UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                                    UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                                    //Increments the memory location of the current triangles and vertices
                                    waterTriangleLoc += 6;
                                    vertLoc += 4;

                                    if (i == 0 && k == 0)
                                    {
                                        vertices[vertLoc] = new Vector3(k, y, w);
                                        vertices[vertLoc + 1] = new Vector3(k, y, w + 1);
                                        vertices[vertLoc + 2] = new Vector3(k, y + 1, w + 1);
                                        vertices[vertLoc + 3] = new Vector3(k, y + 1, w);

                                        //Draws the faces
                                        waterTriangles[waterTriangleLoc] = vertLoc;
                                        waterTriangles[waterTriangleLoc + 1] = vertLoc + 1;
                                        waterTriangles[waterTriangleLoc + 2] = vertLoc + 2;
                                        waterTriangles[waterTriangleLoc + 3] = vertLoc;
                                        waterTriangles[waterTriangleLoc + 4] = vertLoc + 2;
                                        waterTriangles[waterTriangleLoc + 5] = vertLoc + 3;

                                        //Assigns the texture for each vertice
                                        UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                                        UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                                        UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                                        UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                                        //Increments the memory location of the current triangles and vertices
                                        waterTriangleLoc += 6;
                                        vertLoc += 4;
                                    }
                                    if (j == 0 && w == 0)
                                    {
                                        vertices[vertLoc] = new Vector3(k, y, w);
                                        vertices[vertLoc + 1] = new Vector3(k, y + 1, w);
                                        vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w);
                                        vertices[vertLoc + 3] = new Vector3(k + 1, y, w);

                                        //Draws the faces
                                        waterTriangles[waterTriangleLoc] = vertLoc;
                                        waterTriangles[waterTriangleLoc + 1] = vertLoc + 1;
                                        waterTriangles[waterTriangleLoc + 2] = vertLoc + 2;
                                        waterTriangles[waterTriangleLoc + 3] = vertLoc;
                                        waterTriangles[waterTriangleLoc + 4] = vertLoc + 2;
                                        waterTriangles[waterTriangleLoc + 5] = vertLoc + 3;

                                        //Assigns the texture for each vertice
                                        UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                                        UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                                        UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                                        UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                                        //Increments the memory location of the current triangles and vertices
                                        waterTriangleLoc += 6;
                                        vertLoc += 4;
                                    }
                                    if (j == mapSize - 1 && w == 15)
                                    {
                                        vertices[vertLoc] = new Vector3(k, y, w + 1);
                                        vertices[vertLoc + 1] = new Vector3(k + 1, y, w + 1);
                                        vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                                        vertices[vertLoc + 3] = new Vector3(k, y + 1, w + 1);

                                        //Draws the faces
                                        waterTriangles[waterTriangleLoc] = vertLoc;
                                        waterTriangles[waterTriangleLoc + 1] = vertLoc + 1;
                                        waterTriangles[waterTriangleLoc + 2] = vertLoc + 2;
                                        waterTriangles[waterTriangleLoc + 3] = vertLoc;
                                        waterTriangles[waterTriangleLoc + 4] = vertLoc + 2;
                                        waterTriangles[waterTriangleLoc + 5] = vertLoc + 3;

                                        //Assigns the texture for each vertice
                                        UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                                        UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                                        UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                                        UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                                        //Increments the memory location of the current triangles and vertices
                                        waterTriangleLoc += 6;
                                        vertLoc += 4;
                                    }
                                    if (i == mapSize - 1 && k == 15)
                                    {
                                        vertices[vertLoc] = new Vector3(k + 1, y, w);
                                        vertices[vertLoc + 1] = new Vector3(k + 1, y, w + 1);
                                        vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                                        vertices[vertLoc + 3] = new Vector3(k + 1, y + 1, w);

                                        //Draws the faces
                                        waterTriangles[waterTriangleLoc] = vertLoc;
                                        waterTriangles[waterTriangleLoc + 1] = vertLoc + 3;
                                        waterTriangles[waterTriangleLoc + 2] = vertLoc + 2;
                                        waterTriangles[waterTriangleLoc + 3] = vertLoc;
                                        waterTriangles[waterTriangleLoc + 4] = vertLoc + 2;
                                        waterTriangles[waterTriangleLoc + 5] = vertLoc + 1;

                                        //Assigns the texture for each vertice
                                        UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                                        UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                                        UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                                        UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                                        //Increments the memory location of the current triangles and vertices
                                        waterTriangleLoc += 6;
                                        vertLoc += 4;
                                    }
                                    continue;
                                }
                                //Determines which faces are visable for each block and then draws them
                                if (y == 99 || blockIDMap[k + (i * 16), y + 1, w + (j * 16)] == 0 || (blockIDMap[k + (i * 16), y + 1, w + (j * 16)] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16), y + 1, w + (j * 16)]) == true)
                                {
                                    //Says where verticies for the face will be
                                    vertices[vertLoc] = new Vector3(k, y + 1, w);
                                    vertices[vertLoc + 1] = new Vector3(k, y + 1, w + 1);
                                    vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                                    vertices[vertLoc + 3] = new Vector3(k + 1, y + 1, w);

                                    //Draws the faces
                                    triangles[triangleLoc] = vertLoc;
                                    triangles[triangleLoc + 1] = vertLoc + 1;
                                    triangles[triangleLoc + 2] = vertLoc + 2;
                                    triangles[triangleLoc + 3] = vertLoc;
                                    triangles[triangleLoc + 4] = vertLoc + 2;
                                    triangles[triangleLoc + 5] = vertLoc + 3;

                                    //Assigns the texture for each vertice
                                    UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                                    UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                                    UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                                    UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                                    //Increments the memory location of the current triangles and vertices
                                    triangleLoc += 6;
                                    vertLoc += 4;
                                }
                                if ((k == 0 && i == 0) || blockIDMap[k + (i * 16) - 1, y, w + (j * 16)] == 0 || (blockIDMap[k + (i * 16) - 1, y, w + (j * 16)] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16) - 1, y, w + (j * 16)]) == true)
                                {
                                    vertices[vertLoc] = new Vector3(k, y, w);
                                    vertices[vertLoc + 1] = new Vector3(k, y, w + 1);
                                    vertices[vertLoc + 2] = new Vector3(k, y + 1, w + 1);
                                    vertices[vertLoc + 3] = new Vector3(k, y + 1, w);

                                    triangles[triangleLoc] = vertLoc;
                                    triangles[triangleLoc + 1] = vertLoc + 1;
                                    triangles[triangleLoc + 2] = vertLoc + 2;
                                    triangles[triangleLoc + 3] = vertLoc;
                                    triangles[triangleLoc + 4] = vertLoc + 2;
                                    triangles[triangleLoc + 5] = vertLoc + 3;

                                    UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 1, 0);
                                    UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 1, 1);
                                    UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 1, 2);
                                    UVs[vertLoc] = GetTexture(k, i, y, w, j, 1, 3);

                                    triangleLoc += 6;
                                    vertLoc += 4;
                                }
                                if ((w == 0 && j == 0) || blockIDMap[k + (i * 16), y, w + (j * 16) - 1] == 0 || (blockIDMap[k + (i * 16), y, w + (j * 16) - 1] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16), y, w + (j * 16) - 1]) == true)
                                {
                                    vertices[vertLoc] = new Vector3(k, y, w);
                                    vertices[vertLoc + 1] = new Vector3(k, y + 1, w);
                                    vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w);
                                    vertices[vertLoc + 3] = new Vector3(k + 1, y, w);

                                    triangles[triangleLoc] = vertLoc;
                                    triangles[triangleLoc + 1] = vertLoc + 1;
                                    triangles[triangleLoc + 2] = vertLoc + 2;
                                    triangles[triangleLoc + 3] = vertLoc;
                                    triangles[triangleLoc + 4] = vertLoc + 2;
                                    triangles[triangleLoc + 5] = vertLoc + 3;

                                    UVs[vertLoc] = GetTexture(k, i, y, w, j, 2, 0);
                                    UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 2, 1);
                                    UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 2, 2);
                                    UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 2, 3);

                                    triangleLoc += 6;
                                    vertLoc += 4;
                                }
                                if ((k == 15 && i == mapSize - 1) || blockIDMap[k + (i * 16) + 1, y, w + (j * 16)] == 0 || (blockIDMap[k + (i * 16) + 1, y, w + (j * 16)] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16) + 1, y, w + (j * 16)]) == true)
                                {
                                    vertices[vertLoc] = new Vector3(k + 1, y, w);
                                    vertices[vertLoc + 1] = new Vector3(k + 1, y, w + 1);
                                    vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                                    vertices[vertLoc + 3] = new Vector3(k + 1, y + 1, w);

                                    triangles[triangleLoc] = vertLoc;
                                    triangles[triangleLoc + 1] = vertLoc + 3;
                                    triangles[triangleLoc + 2] = vertLoc + 2;
                                    triangles[triangleLoc + 3] = vertLoc;
                                    triangles[triangleLoc + 4] = vertLoc + 2;
                                    triangles[triangleLoc + 5] = vertLoc + 1;

                                    UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 3, 0);
                                    UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 3, 1);
                                    UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 3, 2);
                                    UVs[vertLoc] = GetTexture(k, i, y, w, j, 3, 3);

                                    triangleLoc += 6;
                                    vertLoc += 4;
                                }
                                if ((w == 15 && j == mapSize - 1) || blockIDMap[k + (i * 16), y, w + (j * 16) + 1] == 0 || (blockIDMap[k + (i * 16), y, w + (j * 16) + 1] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16), y, w + (j * 16) + 1]) == true)
                                {
                                    vertices[vertLoc] = new Vector3(k, y, w + 1);
                                    vertices[vertLoc + 1] = new Vector3(k + 1, y, w + 1);
                                    vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                                    vertices[vertLoc + 3] = new Vector3(k, y + 1, w + 1);

                                    triangles[triangleLoc] = vertLoc;
                                    triangles[triangleLoc + 1] = vertLoc + 1;
                                    triangles[triangleLoc + 2] = vertLoc + 2;
                                    triangles[triangleLoc + 3] = vertLoc;
                                    triangles[triangleLoc + 4] = vertLoc + 2;
                                    triangles[triangleLoc + 5] = vertLoc + 3;

                                    UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 4, 0);
                                    UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 4, 1);
                                    UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 4, 2);
                                    UVs[vertLoc] = GetTexture(k, i, y, w, j, 4, 3);

                                    triangleLoc += 6;
                                    vertLoc += 4;
                                }
                                if ((y != 0) && (blockIDMap[k + (i * 16), y - 1, w + (j * 16)] == 0 || (blockIDMap[k + (i * 16), y - 1, w + (j * 16)] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16), y - 1, w + (j * 16)]) == true))
                                {
                                    vertices[vertLoc] = new Vector3(k, y, w);
                                    vertices[vertLoc + 1] = new Vector3(k, y, w + 1);
                                    vertices[vertLoc + 2] = new Vector3(k + 1, y, w + 1);
                                    vertices[vertLoc + 3] = new Vector3(k + 1, y, w);

                                    triangles[triangleLoc] = vertLoc + 3;
                                    triangles[triangleLoc + 1] = vertLoc + 1;
                                    triangles[triangleLoc + 2] = vertLoc;
                                    triangles[triangleLoc + 3] = vertLoc + 2;
                                    triangles[triangleLoc + 4] = vertLoc + 1;
                                    triangles[triangleLoc + 5] = vertLoc + 3;

                                    UVs[vertLoc] = GetTexture(k, i, y, w, j, 5, 0);
                                    UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 5, 1);
                                    UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 5, 2);
                                    UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 5, 3);

                                    triangleLoc += 6;
                                    vertLoc += 4;
                                }
                            }
                        }
                    }
                }

                //Clears any existing mesh, then creates a mesh based on the triangles, and vertices determined above
                //Recalculates normals to make sure they are drawn facing out and are visible
                //Assigns the textures for the mesh based on the UVs determined above
                mesh.Clear();
                mesh.subMeshCount = 2;
                mesh.vertices = vertices;
                mesh.SetTriangles(triangles, 0);
                mesh.SetTriangles(waterTriangles, 1);
                mesh.RecalculateNormals();
                mesh.uv = UVs;

                //Instantiates the chunk with the created mesh, and creates terrain colliders based on the mesh
                chunks[i, j] = Instantiate(chunkPrefab, pos, rot);
                chunks[i, j].name = ("Chunk: (" + i + ", " + j + ")");
                chunks[i, j].GetComponent<MeshFilter>().mesh = mesh;
                chunks[i, j].GetComponent<MeshCollider>().sharedMesh = mesh;
            }
        }
    }
    public void BreakBlock(Vector3 pos)
    {
        if (ts.BreakableBlock(blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z]) == false) return;
        //Removes block at passed in position from block data and redraws the chunk it was in
        blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = 0;
        if ((int)pos.x != 0 && (int)pos.z != 0)
        {
            if ((int)pos.x % 16 == 15 && (int)(pos.x / 16) != mapSize - 1) ReDrawBlockChunk((int)(pos.x / 16) + 1, (int)(pos.z / 16));
            if ((int)pos.x % 16 == 0 && (int)(pos.x / 16) != 0) ReDrawBlockChunk((int)(pos.x / 16) - 1, (int)(pos.z / 16));
            if ((int)pos.z % 16 == 15 && (int)(pos.z / 16) != mapSize - 1) ReDrawBlockChunk((int)(pos.x / 16), (int)(pos.z / 16) + 1);
            if ((int)pos.z % 16 == 0 && (int)(pos.z / 16) != 0) ReDrawBlockChunk((int)(pos.x / 16), (int)(pos.z / 16) - 1);
        }
        ReDrawBlockChunk((int)(pos.x / 16), (int)(pos.z / 16));
    }
    void ReDrawBlockChunk(int i, int j)
    {
        //Declares variables used to generate chunks
        Vector3 chunkLowestPoint = new Vector3(0, 99999, 0);
        Vector3 chunkHighestPoint = new Vector3(0, -1, 0);
        Mesh mesh = new Mesh();
        pos = new Vector3(i * 16, 0, j * 16);
        int numTrianglesPerChunk;
        Vector3[] vertices;
        int[] triangles;
        int[] waterTriangles;
        Vector2[] UVs;

        //Loops through block data for a chunk to find the highest and lowest points in the chunk
        for (int x = i * 16; x < (i + 1) * 16; x++)
        {
            for (int z = j * 16; z < (j + 1) * 16; z++)
            {
                if (!ts.waterEnabled)
                {
                    if (chunkLowestPoint.y > heightMap[x, z]) chunkLowestPoint = new Vector3(x, heightMap[x, z], z);
                    else if (chunkHighestPoint.y < heightMap[x, z]) chunkHighestPoint = new Vector3(x, heightMap[x, z], z);
                }
                else
                {
                    if (chunkLowestPoint.y > heightMap[x, z]) chunkLowestPoint = new Vector3(x, heightMap[x, z], z);
                    else if (chunkHighestPoint.y < heightMap[x, z] + ts.waterHeight) chunkHighestPoint = new Vector3(x, heightMap[x, z] + ts.waterHeight, z);
                }
            }
        }
        if (i == 0 || j == 0 || i == mapSize - 1 || j == mapSize - 1)
        {
            chunkLowestPoint.y = 0;
        }

        //Multiples of 1024 for memory reasons
        vertices = new Vector3[16384];
        UVs = new Vector2[vertices.Length];
        numTrianglesPerChunk = 8192;
        triangles = new int[3 * numTrianglesPerChunk];
        waterTriangles = new int[3 * numTrianglesPerChunk];
        int vertLoc = 0;
        int triangleLoc = 0;
        int waterTriangleLoc = 0;

        //This is lots of math to generate a custom mesh for each chunk
        //Each visible face is drawn and made into one chunk of 16 blocks to reduce strain on the cpu and allow for higher fps on larger maps
        //Drastically increases performance when compared to spawning individual blocks
        for (int y = 0; y <= (int)chunkHighestPoint.y; y++)
        {
            for (int w = 0; w < 16; w++)
            {
                for (int k = 0; k < 16; k++)
                {
                    if (blockIDMap[k + (i * 16), y, w + (j * 16)] != 0 && ts.IsEntity(blockIDMap[k + (i * 16), y, w + (j * 16)]) == false)
                    {
                        //Uncomment out the rest of the if statement if you dont want water to get less transparent as it gets deeper
                        if (blockIDMap[k + (i * 16), y, w + (j * 16)] == 11)// && blockIDMap[k + (i * 16), y + 1, w + (j * 16)] == 0)
                        {
                            //Says where verticies for the face will be
                            vertices[vertLoc] = new Vector3(k, y + 1, w);
                            vertices[vertLoc + 1] = new Vector3(k, y + 1, w + 1);
                            vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                            vertices[vertLoc + 3] = new Vector3(k + 1, y + 1, w);

                            //Draws the faces
                            waterTriangles[waterTriangleLoc] = vertLoc;
                            waterTriangles[waterTriangleLoc + 1] = vertLoc + 1;
                            waterTriangles[waterTriangleLoc + 2] = vertLoc + 2;
                            waterTriangles[waterTriangleLoc + 3] = vertLoc;
                            waterTriangles[waterTriangleLoc + 4] = vertLoc + 2;
                            waterTriangles[waterTriangleLoc + 5] = vertLoc + 3;

                            //Assigns the texture for each vertice
                            UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                            UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                            UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                            UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                            //Increments the memory location of the current triangles and vertices
                            waterTriangleLoc += 6;
                            vertLoc += 4;

                            if (i == 0 && k == 0)
                            {
                                vertices[vertLoc] = new Vector3(k, y, w);
                                vertices[vertLoc + 1] = new Vector3(k, y, w + 1);
                                vertices[vertLoc + 2] = new Vector3(k, y + 1, w + 1);
                                vertices[vertLoc + 3] = new Vector3(k, y + 1, w);

                                //Draws the faces
                                waterTriangles[waterTriangleLoc] = vertLoc;
                                waterTriangles[waterTriangleLoc + 1] = vertLoc + 1;
                                waterTriangles[waterTriangleLoc + 2] = vertLoc + 2;
                                waterTriangles[waterTriangleLoc + 3] = vertLoc;
                                waterTriangles[waterTriangleLoc + 4] = vertLoc + 2;
                                waterTriangles[waterTriangleLoc + 5] = vertLoc + 3;

                                //Assigns the texture for each vertice
                                UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                                UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                                UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                                UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                                //Increments the memory location of the current triangles and vertices
                                waterTriangleLoc += 6;
                                vertLoc += 4;
                            }
                            if (j == 0 && w == 0)
                            {
                                vertices[vertLoc] = new Vector3(k, y, w);
                                vertices[vertLoc + 1] = new Vector3(k, y + 1, w);
                                vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w);
                                vertices[vertLoc + 3] = new Vector3(k + 1, y, w);

                                //Draws the faces
                                waterTriangles[waterTriangleLoc] = vertLoc;
                                waterTriangles[waterTriangleLoc + 1] = vertLoc + 1;
                                waterTriangles[waterTriangleLoc + 2] = vertLoc + 2;
                                waterTriangles[waterTriangleLoc + 3] = vertLoc;
                                waterTriangles[waterTriangleLoc + 4] = vertLoc + 2;
                                waterTriangles[waterTriangleLoc + 5] = vertLoc + 3;

                                //Assigns the texture for each vertice
                                UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                                UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                                UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                                UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                                //Increments the memory location of the current triangles and vertices
                                waterTriangleLoc += 6;
                                vertLoc += 4;
                            }
                            if (j == mapSize - 1 && w == 15)
                            {
                                vertices[vertLoc] = new Vector3(k, y, w + 1);
                                vertices[vertLoc + 1] = new Vector3(k + 1, y, w + 1);
                                vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                                vertices[vertLoc + 3] = new Vector3(k, y + 1, w + 1);

                                //Draws the faces
                                waterTriangles[waterTriangleLoc] = vertLoc;
                                waterTriangles[waterTriangleLoc + 1] = vertLoc + 1;
                                waterTriangles[waterTriangleLoc + 2] = vertLoc + 2;
                                waterTriangles[waterTriangleLoc + 3] = vertLoc;
                                waterTriangles[waterTriangleLoc + 4] = vertLoc + 2;
                                waterTriangles[waterTriangleLoc + 5] = vertLoc + 3;

                                //Assigns the texture for each vertice
                                UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                                UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                                UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                                UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                                //Increments the memory location of the current triangles and vertices
                                waterTriangleLoc += 6;
                                vertLoc += 4;
                            }
                            if (i == mapSize - 1 && k == 15)
                            {
                                vertices[vertLoc] = new Vector3(k + 1, y, w);
                                vertices[vertLoc + 1] = new Vector3(k + 1, y, w + 1);
                                vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                                vertices[vertLoc + 3] = new Vector3(k + 1, y + 1, w);

                                //Draws the faces
                                waterTriangles[waterTriangleLoc] = vertLoc;
                                waterTriangles[waterTriangleLoc + 1] = vertLoc + 3;
                                waterTriangles[waterTriangleLoc + 2] = vertLoc + 2;
                                waterTriangles[waterTriangleLoc + 3] = vertLoc;
                                waterTriangles[waterTriangleLoc + 4] = vertLoc + 2;
                                waterTriangles[waterTriangleLoc + 5] = vertLoc + 1;

                                //Assigns the texture for each vertice
                                UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                                UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                                UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                                UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                                //Increments the memory location of the current triangles and vertices
                                waterTriangleLoc += 6;
                                vertLoc += 4;
                            }
                            continue;
                        }
                        //Determines which faces are visable for each block and then draws them
                        if (y == 99 || blockIDMap[k + (i * 16), y + 1, w + (j * 16)] == 0 || (blockIDMap[k + (i * 16), y + 1, w + (j * 16)] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16), y + 1, w + (j * 16)]) == true)
                        {
                            //Says where verticies for the face will be
                            vertices[vertLoc] = new Vector3(k, y + 1, w);
                            vertices[vertLoc + 1] = new Vector3(k, y + 1, w + 1);
                            vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                            vertices[vertLoc + 3] = new Vector3(k + 1, y + 1, w);

                            //Draws the faces
                            triangles[triangleLoc] = vertLoc;
                            triangles[triangleLoc + 1] = vertLoc + 1;
                            triangles[triangleLoc + 2] = vertLoc + 2;
                            triangles[triangleLoc + 3] = vertLoc;
                            triangles[triangleLoc + 4] = vertLoc + 2;
                            triangles[triangleLoc + 5] = vertLoc + 3;

                            //Assigns the texture for each vertice
                            UVs[vertLoc] = GetTexture(k, i, y, w, j, 0, 0);
                            UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 0, 1);
                            UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 0, 2);
                            UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 0, 3);

                            //Increments the memory location of the current triangles and vertices
                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                        if ((k == 0 && i == 0) || blockIDMap[k + (i * 16) - 1, y, w + (j * 16)] == 0 || (blockIDMap[k + (i * 16) - 1, y, w + (j * 16)] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16) - 1, y, w + (j * 16)]) == true)
                        {
                            vertices[vertLoc] = new Vector3(k, y, w);
                            vertices[vertLoc + 1] = new Vector3(k, y, w + 1);
                            vertices[vertLoc + 2] = new Vector3(k, y + 1, w + 1);
                            vertices[vertLoc + 3] = new Vector3(k, y + 1, w);

                            triangles[triangleLoc] = vertLoc;
                            triangles[triangleLoc + 1] = vertLoc + 1;
                            triangles[triangleLoc + 2] = vertLoc + 2;
                            triangles[triangleLoc + 3] = vertLoc;
                            triangles[triangleLoc + 4] = vertLoc + 2;
                            triangles[triangleLoc + 5] = vertLoc + 3;

                            UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 1, 0);
                            UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 1, 1);
                            UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 1, 2);
                            UVs[vertLoc] = GetTexture(k, i, y, w, j, 1, 3);

                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                        if ((w == 0 && j == 0) || blockIDMap[k + (i * 16), y, w + (j * 16) - 1] == 0 || (blockIDMap[k + (i * 16), y, w + (j * 16) - 1] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16), y, w + (j * 16) - 1]) == true)
                        {
                            vertices[vertLoc] = new Vector3(k, y, w);
                            vertices[vertLoc + 1] = new Vector3(k, y + 1, w);
                            vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w);
                            vertices[vertLoc + 3] = new Vector3(k + 1, y, w);

                            triangles[triangleLoc] = vertLoc;
                            triangles[triangleLoc + 1] = vertLoc + 1;
                            triangles[triangleLoc + 2] = vertLoc + 2;
                            triangles[triangleLoc + 3] = vertLoc;
                            triangles[triangleLoc + 4] = vertLoc + 2;
                            triangles[triangleLoc + 5] = vertLoc + 3;

                            UVs[vertLoc] = GetTexture(k, i, y, w, j, 2, 0);
                            UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 2, 1);
                            UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 2, 2);
                            UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 2, 3);

                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                        if ((k == 15 && i == mapSize - 1) || blockIDMap[k + (i * 16) + 1, y, w + (j * 16)] == 0 || (blockIDMap[k + (i * 16) + 1, y, w + (j * 16)] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16) + 1, y, w + (j * 16)]) == true)
                        {
                            vertices[vertLoc] = new Vector3(k + 1, y, w);
                            vertices[vertLoc + 1] = new Vector3(k + 1, y, w + 1);
                            vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                            vertices[vertLoc + 3] = new Vector3(k + 1, y + 1, w);

                            triangles[triangleLoc] = vertLoc;
                            triangles[triangleLoc + 1] = vertLoc + 3;
                            triangles[triangleLoc + 2] = vertLoc + 2;
                            triangles[triangleLoc + 3] = vertLoc;
                            triangles[triangleLoc + 4] = vertLoc + 2;
                            triangles[triangleLoc + 5] = vertLoc + 1;

                            UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 3, 0);
                            UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 3, 1);
                            UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 3, 2);
                            UVs[vertLoc] = GetTexture(k, i, y, w, j, 3, 3);

                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                        if ((w == 15 && j == mapSize - 1) || blockIDMap[k + (i * 16), y, w + (j * 16) + 1] == 0 || (blockIDMap[k + (i * 16), y, w + (j * 16) + 1] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16), y, w + (j * 16) + 1]) == true)
                        {
                            vertices[vertLoc] = new Vector3(k, y, w + 1);
                            vertices[vertLoc + 1] = new Vector3(k + 1, y, w + 1);
                            vertices[vertLoc + 2] = new Vector3(k + 1, y + 1, w + 1);
                            vertices[vertLoc + 3] = new Vector3(k, y + 1, w + 1);

                            triangles[triangleLoc] = vertLoc;
                            triangles[triangleLoc + 1] = vertLoc + 1;
                            triangles[triangleLoc + 2] = vertLoc + 2;
                            triangles[triangleLoc + 3] = vertLoc;
                            triangles[triangleLoc + 4] = vertLoc + 2;
                            triangles[triangleLoc + 5] = vertLoc + 3;

                            UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 4, 0);
                            UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 4, 1);
                            UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 4, 2);
                            UVs[vertLoc] = GetTexture(k, i, y, w, j, 4, 3);

                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                        if ((y != 0) && (blockIDMap[k + (i * 16), y - 1, w + (j * 16)] == 0 || (blockIDMap[k + (i * 16), y - 1, w + (j * 16)] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16), y - 1, w + (j * 16)]) == true))
                        {
                            vertices[vertLoc] = new Vector3(k, y, w);
                            vertices[vertLoc + 1] = new Vector3(k, y, w + 1);
                            vertices[vertLoc + 2] = new Vector3(k + 1, y, w + 1);
                            vertices[vertLoc + 3] = new Vector3(k + 1, y, w);

                            triangles[triangleLoc] = vertLoc + 3;
                            triangles[triangleLoc + 1] = vertLoc + 1;
                            triangles[triangleLoc + 2] = vertLoc;
                            triangles[triangleLoc + 3] = vertLoc + 2;
                            triangles[triangleLoc + 4] = vertLoc + 1;
                            triangles[triangleLoc + 5] = vertLoc + 3;

                            UVs[vertLoc] = GetTexture(k, i, y, w, j, 5, 0);
                            UVs[vertLoc + 1] = GetTexture(k, i, y, w, j, 5, 1);
                            UVs[vertLoc + 2] = GetTexture(k, i, y, w, j, 5, 2);
                            UVs[vertLoc + 3] = GetTexture(k, i, y, w, j, 5, 3);

                            triangleLoc += 6;
                            vertLoc += 4;
                        }
                    }
                }
            }
        }

        //Clears any existing mesh, then creates a mesh based on the triangles, and vertices determined above
        //Recalculates normals to make sure they are drawn facing out and are visible
        //Assigns the textures for the mesh based on the UVs determined above
        mesh.Clear();
        mesh.subMeshCount = 2;
        mesh.vertices = vertices;
        mesh.SetTriangles(triangles, 0);
        mesh.SetTriangles(waterTriangles, 1);
        mesh.RecalculateNormals();
        mesh.uv = UVs;

        //Instantiates the chunk with the created mesh, and creates terrain colliders based on the mesh
        chunks[i, j].GetComponent<MeshFilter>().mesh = mesh;
        chunks[i, j].GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    Vector2 GetTexture(int k, int i, int y, int w, int j, int s, int c)
    {
        //References the TerrainSettings script to get the block texture based on the side in question and the block id
        return ts.textures[(int)ts.blockIDs[blockIDMap[k + (i * 16), y, w + (j * 16)] - 1, s].x, (int)ts.blockIDs[blockIDMap[k + (i * 16), y, w + (j * 16)] - 1, s].y, c];
    }
    void GenOres(Vector3 pos)
    {
        //Adds ores to block data based on the frequency settings
        //Add code here if youd like to add more ores to the game
        float rand;

        rand = Random.Range(0.0f, 100.0f);
        if (rand < ts.coalFreq)
        {
            blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = 4;
            return;
        }
        rand = Random.Range(0.0f, 100.0f);
        if (rand < ts.ironFreq)
        {
            blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = 5;
            return;
        }
        rand = Random.Range(0.0f, 100.0f);
        if (rand < ts.goldFreq)
        {
            blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = 6;
            return;
        }
        rand = Random.Range(0.0f, 100.0f);
        if (rand < ts.silverFreq)
        {
            blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = 7;
            return;
        }
        rand = Random.Range(0.0f, 100.0f);
        if (rand < ts.crystalFreq)
        {
            blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = 8;
            return;
        }
        blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = ts.layerId[ts.numLayers - 1];
    }
    void GenEntities()
    {
        //Loops through the map and spawns entities in the map
        float rand;
        for (int i = 0; i < blockIDMap.GetLength(0); i++)
        {
            for (int j = 0; j < blockIDMap.GetLength(1); j++)
            {
                for (int k = 0; k < blockIDMap.GetLength(2); k++)
                {
                    //Only spawns entities above ground
                    if (blockIDMap[i, j, k] == ts.layerId[0] && blockIDMap[i, j + 1, k] == 0)
                    {
                        //Generate tree based on the tree frequency setting
                        rand = Random.Range(0.0f, 100.0f);
                        if (rand < ts.treeFreq)
                        {
                            //If a tree is spawned determine its type
                            //By default its a 1/3rd chance to be birch
                            int treeTypeID;
                            if (Random.Range(0, 3) == 2) treeTypeID = 9;
                            else treeTypeID = 3;
                            pos = new Vector3(i, j + 1, k);
                            blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = ts.treeID;
                            continue;
                        }
                        rand = Random.Range(0.0f, 100.0f);
                        if (rand < ts.rockFreq)
                        {
                            //Adds rocks to block dating
                            pos = new Vector3(i, j + 1, k);
                            blockIDMap[(int)pos.x, (int)pos.y, (int)pos.z] = 10;
                            continue;
                        }
                    }

                    //Checks if the block in question is an entity
                    if (ts.IsEntity(blockIDMap[i, j, k]))
                    {
                        //Sets variables to instantiate and which variant of the entity to spawn
                        pos = new Vector3(i + 0.5f, j, k + 0.5f);
                        Quaternion rot = new Quaternion(-1, 0, 0, 1);
                        GameObject tempCube = null;
                        int variantPrefab = Random.Range(0, 3);

                        //Adjusts position based on the offset determined in the terrain settings and instantiates the prefab
                        pos.y += (int)ts.blockIDs[blockIDMap[i, j, k] - 1, variantPrefab].y;
                        tempCube = Instantiate(ts.entityPrefabs[(int)ts.blockIDs[blockIDMap[i, j, k] - 1, 0].x, variantPrefab], pos, rot);
                        tempCube.name = "x: " + i + " y: " + j + " z: " + k;
                    }
                }
            }
        }
    }
}