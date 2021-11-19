using UnityEngine;

public class AestheticTerrain : MonoBehaviour
{
    float[,] heightMap;
    int mapSize;
    float frequency;
    float amplitude;
    float xOffSet;
    float yOffSet;
    TerrainSettings ts;

    void Start()
    {
        ts = TerrainSettings.getInstance();

        if (ts.firstLoad && ts.customPlayer)
        {
            Vector3 pos = new Vector3(50, 36, -20);
            Quaternion rot = new Quaternion();

            Instantiate(ts.player, pos, rot);
            //GameObject uiObject = Instantiate(ts.UIPrefab, pos, rot);
            //uiObject.name = "Canvas";
            ts.firstLoad = false;
        }

        mapSize = 8;
        heightMap = new float[mapSize * 16, mapSize * 16];
        frequency = 1.6f;
        amplitude = 17;
        Random.InitState(System.DateTime.Now.Millisecond * this.GetInstanceID());
        xOffSet = Random.Range(0.0f, 999999.9f);
        yOffSet = Random.Range(0.0f, 999999.9f);

        frequency = ts.frequencyMultiplier;
        amplitude = ts.amplitudeMultiplier;

        for (float i = 0; i < (mapSize * 16) - 1; i++)
        {
            for (float j = 0; j < (mapSize * 16) - 1; j++)
            {
                //Debug.Log(ts.frequencyMultiplier * ((Mathf.Pow((i / (mapSize * 16)), 2) / 2) + 1));
                //frequency = ts.frequencyMultiplier * ((Mathf.Pow((i / (mapSize * 16)), 2) / 2) + 1);

                heightMap[(int)i, (int)j] = (amplitude * Mathf.PerlinNoise((xOffSet + i) / 320 * frequency, (yOffSet + j) / 320 * frequency));
                heightMap[(int)i, (int)j] += ((amplitude / 2) * Mathf.PerlinNoise((xOffSet + i) / 320 * frequency * 2, (yOffSet + j) / 320 * frequency * 2));
                heightMap[(int)i, (int)j] += ((amplitude / 4) * Mathf.PerlinNoise((xOffSet + i) / 320 * frequency * 4, (yOffSet + j) / 320 * frequency * 4));

                //heightMap[(int)i, (int)j] += 42 * (Mathf.Sin((((i / (mapSize * 16)) * 180) - 90) * Mathf.Deg2Rad) + 2);
            }
        }

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                Mesh mesh = new Mesh();
                Vector3[] vertices = new Vector3[30720 * 8];
                int numTrianglesPerChunk = 30720 * 8;
                int[] triangles = new int[3 * numTrianglesPerChunk];
                int triangleLoc = 0;
                int vertLoc = 0;
                Vector2[] UVs = new Vector2[vertices.Length];
                float stoneGradient = 0.4f;

                for (int x = i * 16; x < (i * 16) + 16; x++)
                {
                    for (int z = j * 16; z < (j * 16) + 16; z++)
                    {
                        if ((i == mapSize - 1 && x == (mapSize * 16) - 1) || (j == mapSize - 1 && z == (mapSize * 16) - 1)) continue;

                        vertices[vertLoc + 2] = new Vector3(x, heightMap[x, z], z);
                        vertices[vertLoc + 1] = new Vector3(x + 0.5f, heightMap[x, z] + ((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2), z + 0.5f);
                        vertices[vertLoc] = new Vector3(x, heightMap[x, z] + ((heightMap[x, z + 1] - heightMap[x, z]) / 2), z + 0.5f);

                        vertices[vertLoc + 5] = new Vector3(x, heightMap[x, z], z);
                        vertices[vertLoc + 4] = new Vector3(x + 0.5f, heightMap[x, z] + ((heightMap[x + 1, z] - heightMap[x, z]) / 2), z);
                        vertices[vertLoc + 3] = new Vector3(x + 0.5f, heightMap[x, z] + ((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2), z + 0.5f);

                        triangles[triangleLoc] = vertLoc;
                        triangles[triangleLoc + 1] = vertLoc + 1;
                        triangles[triangleLoc + 2] = vertLoc + 2;

                        triangles[triangleLoc + 3] = vertLoc + 3;
                        triangles[triangleLoc + 4] = vertLoc + 4;
                        triangles[triangleLoc + 5] = vertLoc + 5;

                        if (Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((heightMap[x, z + 1] - heightMap[x, z]) / 2)) <= stoneGradient)
                        {
                            UVs[vertLoc] = new Vector2(0.5f, 0.75f);
                            UVs[vertLoc + 1] = new Vector2(0.5f, 1);
                            UVs[vertLoc + 2] = new Vector2(0.75f, 1);
                        }
                        else
                        {
                            UVs[vertLoc] = new Vector2(0.75f, 0.75f);
                            UVs[vertLoc + 1] = new Vector2(0.75f, 1);
                            UVs[vertLoc + 2] = new Vector2(1, 1);
                        }

                        if (Mathf.Abs(((heightMap[x + 1, z] - heightMap[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2)) <= stoneGradient)
                        {
                            UVs[vertLoc + 3] = new Vector2(0.5f, 0.75f);
                            UVs[vertLoc + 4] = new Vector2(0.5f, 1);
                            UVs[vertLoc + 5] = new Vector2(0.75f, 1);
                        }
                        else
                        {
                            UVs[vertLoc + 3] = new Vector2(0.75f, 0.75f);
                            UVs[vertLoc + 4] = new Vector2(0.75f, 1);
                            UVs[vertLoc + 5] = new Vector2(1, 1);
                        }

                        triangleLoc += 6;
                        vertLoc += 6;

                        vertices[vertLoc + 2] = new Vector3(x + 1, heightMap[x + 1, z] + ((heightMap[x + 1, z + 1] - heightMap[x + 1, z]) / 2), z + 0.5f);
                        vertices[vertLoc + 1] = new Vector3(x + 1, heightMap[x + 1, z + 1], z + 1);
                        vertices[vertLoc] = new Vector3(x + 0.5f, heightMap[x, z] + ((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2), z + 0.5f);

                        vertices[vertLoc + 5] = new Vector3(x + 1, heightMap[x + 1, z + 1], z + 1);
                        vertices[vertLoc + 4] = new Vector3(x + 0.5f, heightMap[x, z + 1] + ((heightMap[x + 1, z + 1] - heightMap[x, z + 1]) / 2), z + 1);
                        vertices[vertLoc + 3] = new Vector3(x + 0.5f, heightMap[x, z] + ((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2), z + 0.5f);

                        triangles[triangleLoc] = vertLoc;
                        triangles[triangleLoc + 1] = vertLoc + 1;
                        triangles[triangleLoc + 2] = vertLoc + 2;

                        triangles[triangleLoc + 3] = vertLoc + 3;
                        triangles[triangleLoc + 4] = vertLoc + 4;
                        triangles[triangleLoc + 5] = vertLoc + 5;

                        if (Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x + 1, z]) / 2)) <= stoneGradient || Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2)) <= stoneGradient)
                        {
                            UVs[vertLoc] = new Vector2(0.5f, 0.75f);
                            UVs[vertLoc + 1] = new Vector2(0.5f, 1);
                            UVs[vertLoc + 2] = new Vector2(0.75f, 1);
                        }
                        else
                        {
                            UVs[vertLoc] = new Vector2(0.75f, 0.75f);
                            UVs[vertLoc + 1] = new Vector2(0.75f, 1);
                            UVs[vertLoc + 2] = new Vector2(1, 1);
                        }

                        if (Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x, z + 1]) / 2)) <= stoneGradient || Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2)) <= stoneGradient)
                        {
                            UVs[vertLoc + 3] = new Vector2(0.5f, 0.75f);
                            UVs[vertLoc + 4] = new Vector2(0.5f, 1);
                            UVs[vertLoc + 5] = new Vector2(0.75f, 1);
                        }
                        else
                        {
                            UVs[vertLoc + 3] = new Vector2(0.75f, 0.75f);
                            UVs[vertLoc + 4] = new Vector2(0.75f, 1);
                            UVs[vertLoc + 5] = new Vector2(1, 1);
                        }

                        triangleLoc += 6;
                        vertLoc += 6;

                        vertices[vertLoc] = new Vector3(x, heightMap[x, z] + ((heightMap[x, z + 1] - heightMap[x, z]) / 2), z + 0.5f);
                        vertices[vertLoc + 1] = new Vector3(x, heightMap[x, z + 1], z + 1);
                        vertices[vertLoc + 2] = new Vector3(x + 0.5f, heightMap[x, z + 1] + ((heightMap[x + 1, z + 1] - heightMap[x, z + 1]) / 2), z + 1);

                        vertices[vertLoc + 3] = new Vector3(x, heightMap[x, z] + ((heightMap[x, z + 1] - heightMap[x, z]) / 2), z + 0.5f);
                        vertices[vertLoc + 4] = new Vector3(x + 0.5f, heightMap[x, z + 1] + ((heightMap[x + 1, z + 1] - heightMap[x, z + 1]) / 2), z + 1);
                        vertices[vertLoc + 5] = new Vector3(x + 0.5f, heightMap[x, z] + ((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2), z + 0.5f);

                        triangles[triangleLoc] = vertLoc;
                        triangles[triangleLoc + 1] = vertLoc + 1;
                        triangles[triangleLoc + 2] = vertLoc + 2;

                        triangles[triangleLoc + 3] = vertLoc + 3;
                        triangles[triangleLoc + 4] = vertLoc + 4;
                        triangles[triangleLoc + 5] = vertLoc + 5;

                        if (Mathf.Abs(((heightMap[x, z + 1] - heightMap[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x, z + 1]) / 2)) <= stoneGradient)
                        {
                            UVs[vertLoc] = new Vector2(0.5f, 0.75f);
                            UVs[vertLoc + 1] = new Vector2(0.5f, 1);
                            UVs[vertLoc + 2] = new Vector2(0.75f, 1);
                        }
                        else
                        {
                            UVs[vertLoc] = new Vector2(0.75f, 0.75f);
                            UVs[vertLoc + 1] = new Vector2(0.75f, 1);
                            UVs[vertLoc + 2] = new Vector2(1, 1);
                        }

                        if (Mathf.Abs(((heightMap[x, z + 1] - heightMap[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x, z + 1]) / 2)) <= stoneGradient || Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2)) <= stoneGradient)
                        {
                            UVs[vertLoc + 3] = new Vector2(0.5f, 0.75f);
                            UVs[vertLoc + 4] = new Vector2(0.5f, 1);
                            UVs[vertLoc + 5] = new Vector2(0.75f, 1);
                        }
                        else
                        {
                            UVs[vertLoc + 3] = new Vector2(0.75f, 0.75f);
                            UVs[vertLoc + 4] = new Vector2(0.75f, 1);
                            UVs[vertLoc + 5] = new Vector2(1, 1);
                        }

                        triangleLoc += 6;
                        vertLoc += 6;

                        vertices[vertLoc] = new Vector3(x + 0.5f, heightMap[x, z] + ((heightMap[x + 1, z] - heightMap[x, z]) / 2), z);
                        vertices[vertLoc + 1] = new Vector3(x + 0.5f, heightMap[x, z] + ((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2), z + 0.5f);
                        vertices[vertLoc + 2] = new Vector3(x + 1, heightMap[x + 1, z] + ((heightMap[x + 1, z + 1] - heightMap[x + 1, z]) / 2), z + 0.5f);

                        vertices[vertLoc + 3] = new Vector3(x + 0.5f, heightMap[x, z] + ((heightMap[x + 1, z] - heightMap[x, z]) / 2), z);
                        vertices[vertLoc + 4] = new Vector3(x + 1, heightMap[x + 1, z] + ((heightMap[x + 1, z + 1] - heightMap[x + 1, z]) / 2), z + 0.5f);
                        vertices[vertLoc + 5] = new Vector3(x + 1, heightMap[x + 1, z], z);

                        triangles[triangleLoc] = vertLoc;
                        triangles[triangleLoc + 1] = vertLoc + 1;
                        triangles[triangleLoc + 2] = vertLoc + 2;

                        triangles[triangleLoc + 3] = vertLoc + 3;
                        triangles[triangleLoc + 4] = vertLoc + 4;
                        triangles[triangleLoc + 5] = vertLoc + 5;

                        if (Mathf.Abs(((heightMap[x + 1, z] - heightMap[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x + 1, z]) / 2)) <= stoneGradient)
                        {
                            UVs[vertLoc] = new Vector2(0.5f, 0.75f);
                            UVs[vertLoc + 1] = new Vector2(0.5f, 1);
                            UVs[vertLoc + 2] = new Vector2(0.75f, 1);
                        }
                        else
                        {
                            UVs[vertLoc] = new Vector2(0.75f, 0.75f);
                            UVs[vertLoc + 1] = new Vector2(0.75f, 1);
                            UVs[vertLoc + 2] = new Vector2(1, 1);
                        }

                        if (Mathf.Abs(((heightMap[x + 1, z] - heightMap[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((heightMap[x + 1, z + 1] - heightMap[x + 1, z]) / 2)) <= stoneGradient)
                        {
                            UVs[vertLoc + 3] = new Vector2(0.5f, 0.75f);
                            UVs[vertLoc + 4] = new Vector2(0.5f, 1);
                            UVs[vertLoc + 5] = new Vector2(0.75f, 1);
                        }
                        else
                        {
                            UVs[vertLoc + 3] = new Vector2(0.75f, 0.75f);
                            UVs[vertLoc + 4] = new Vector2(0.75f, 1);
                            UVs[vertLoc + 5] = new Vector2(1, 1);
                        }

                        triangleLoc += 6;
                        vertLoc += 6;
                    }
                }

                mesh.Clear();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.RecalculateNormals();
                mesh.uv = UVs;

                Vector3 chunkPos = new Vector3(0, 0, 0);
                GameObject chunkPrefab = ts.chunkPrefab;
                GameObject chunk = Instantiate(chunkPrefab, chunkPos, new Quaternion());
                chunk.name = ("Chunk: (" + i + ":" + j + ")");
                chunk.GetComponent<MeshFilter>().mesh = mesh;
                chunk.GetComponent<MeshCollider>().sharedMesh = mesh;
            }
        }

        /*
        for (float i = xCoord; i < mapSize - 1 + xCoord; i++)
        {
            for (float j = yCoord; j < mapSize - 1 + yCoord; j++)
            {
                heightMap[(int)(i - xCoord), (int)(j - yCoord)] = (amplitude * Mathf.PerlinNoise((xOffSet + i) / 320 * frequency, (yOffSet + j) / 320 * frequency));
                heightMap[(int)(i - xCoord), (int)(j - yCoord)] += ((amplitude / 2) * Mathf.PerlinNoise((xOffSet + i) / 320 * frequency * 2, (yOffSet + j) / 320 * frequency * 2));
                heightMap[(int)(i - xCoord), (int)(j - yCoord)] += ((amplitude / 4) * Mathf.PerlinNoise((xOffSet + i) / 320 * frequency * 4, (yOffSet + j) / 320 * frequency * 4));
            }
        }

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[30720 * 4];
        int numTrianglesPerChunk = 30720 * 4;
        int[] triangles = new int[3 * numTrianglesPerChunk];
        int triangleLoc = 0;
        int vertLoc = 0;
        Vector2[] UVs = new Vector2[vertices.Length];

        for (int i = 0; i < mapSize - 1; i++)
        {
            for (int j = 0; j < mapSize - 1; j++)
            {
                //if (heightMap[i + 1, j + 1] == 0 || heightMap[i, j + 1] == 0 || heightMap[i + 1, j] == 0) continue;
                vertices[vertLoc + 2] = new Vector3(i, heightMap[i, j], j);
                vertices[vertLoc + 1] = new Vector3(i + 1, heightMap[i + 1, j + 1], j + 1);
                vertices[vertLoc] = new Vector3(i, heightMap[i, j + 1], j + 1);

                vertices[vertLoc + 5] = new Vector3(i, heightMap[i, j], j);
                vertices[vertLoc + 4] = new Vector3(i + 1, heightMap[i + 1, j], j);
                vertices[vertLoc + 3] = new Vector3(i + 1, heightMap[i + 1, j + 1], j + 1);

                triangles[triangleLoc] = vertLoc;
                triangles[triangleLoc + 1] = vertLoc + 1;
                triangles[triangleLoc + 2] = vertLoc + 2;

                triangles[triangleLoc + 3] = vertLoc + 3;
                triangles[triangleLoc + 4] = vertLoc + 4;
                triangles[triangleLoc + 5] = vertLoc + 5;

                UVs[vertLoc + 2] = new Vector2(0, 0);
                UVs[vertLoc + 1] = new Vector2(1, 1);
                UVs[vertLoc] = new Vector2(0, 1);

                UVs[vertLoc + 5] = new Vector2(0, 0);
                UVs[vertLoc + 4] = new Vector2(1, 0);
                UVs[vertLoc + 3] = new Vector2(1, 1);

                triangleLoc += 6;
                vertLoc += 6;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = UVs;
        this.GetComponent<MeshFilter>().mesh = mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
        */
    }
    void Update()
    {
        
    }
}