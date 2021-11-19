using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour
{
    TerrainSettings ts;
    Quaternion rot;
    int mapSize;
    Vector3 pos;
    float[,] blockIDMap;
    GameObject chunkPrefab;
    GameObject[,] chunks;
    ushort mapHeight;
    /*
    public GameObject[,] RenderMap(int[,,] bim)
    {
        mapHeight = 512;
        blockIDMap = bim;
        ts = TerrainSettings.getInstance();
        rot = new Quaternion(0, 0, 0, 1);
        mapSize = ts.mapSize;
        chunkPrefab = ts.chunkPrefab;
        chunks = new GameObject[mapSize, mapSize];

        //Loops through block data and generates chunks of 16 blocks using block data to render the map
        rot = new Quaternion(0, 0, 0, 1);
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                Mesh mesh = new Mesh();
                pos = new Vector3((i * 16), 0, j * 16);
                int numTrianglesPerChunk;
                Vector3[] vertices;
                int[] triangles;
                int[] waterTriangles;
                Vector2[] UVs;

                //Multiples of 1024 for memory reasons
                vertices = new Vector3[131072];
                UVs = new Vector2[vertices.Length];
                numTrianglesPerChunk = 65536;
                triangles = new int[3 * numTrianglesPerChunk];
                waterTriangles = new int[3 * numTrianglesPerChunk];
                int vertLoc = 0;
                int triangleLoc = 0;
                int waterTriangleLoc = 0;

                //This is lots of math to generate a custom mesh for each chunk
                //Each visible face is drawn and made into one chunk of 16 blocks to reduce strain on the cpu and allow for higher fps on larger maps
                //Drastically increases performance when compared to spawning individual blocks
                for (int y = 0; y < mapHeight; y++)
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
                                if (y == mapHeight - 1 || blockIDMap[k + (i * 16), y + 1, w + (j * 16)] == 0 || (blockIDMap[k + (i * 16), y + 1, w + (j * 16)] == 11 && blockIDMap[k + (i * 16), y, w + (j * 16)] != 11) || ts.IsEntity(blockIDMap[k + (i * 16), y + 1, w + (j * 16)]) == true)
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
                Mesh waterMesh = new Mesh();
                waterMesh.Clear();
                waterMesh.vertices = vertices;
                waterMesh.triangles = triangles;

                //Instantiates the chunk with the created mesh, and creates terrain colliders based on the mesh
                chunks[i, j] = Instantiate(chunkPrefab, pos, rot);
                chunks[i, j].name = ("Chunk: (" + i + ", " + j + ")");
                chunks[i, j].GetComponent<MeshFilter>().mesh = mesh;
                chunks[i, j].GetComponent<MeshCollider>().sharedMesh = waterMesh;
            }
        }
        return chunks;
    }
    */
    public Mesh[] RenderChunk(float[,] bim)
    {
        //Declares variables used to generate chunks
        Vector3 chunkLowestPoint = new Vector3(0, 99999, 0);
        Vector3 chunkHighestPoint = new Vector3(0, -1, 0);
        blockIDMap = bim;
        ts = TerrainSettings.getInstance();
        Mesh mesh = new Mesh();
        pos = new Vector3(0, 0, 0);
        int numTrianglesPerChunk;
        Vector3[] vertices;
        int[] triangles;
        int[] waterTriangles;
        Vector2[] UVs;

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
        for (int x = 1; x < 17; x++)
        {
            for (int z = 1; z < 17; z++)
            {
                float stoneGradient = 0.4f;

                //if (x == 17 || z == 15) continue;
                vertices[vertLoc + 2] = new Vector3(x, bim[x, z], z);
                vertices[vertLoc + 1] = new Vector3(x + 0.5f, bim[x, z] + ((bim[x + 1, z + 1] - bim[x, z]) / 2), z + 0.5f);
                vertices[vertLoc] = new Vector3(x, bim[x, z] + ((bim[x, z + 1] - bim[x, z]) / 2), z + 0.5f);
                
                vertices[vertLoc + 5] = new Vector3(x, bim[x, z], z);
                vertices[vertLoc + 4] = new Vector3(x + 0.5f, bim[x, z] + ((bim[x + 1, z] - bim[x, z]) / 2), z);
                vertices[vertLoc + 3] = new Vector3(x + 0.5f, bim[x, z] + ((bim[x + 1, z + 1] - bim[x, z]) / 2), z + 0.5f);

                triangles[triangleLoc] = vertLoc;
                triangles[triangleLoc + 1] = vertLoc + 1;
                triangles[triangleLoc + 2] = vertLoc + 2;
                
                triangles[triangleLoc + 3] = vertLoc + 3;
                triangles[triangleLoc + 4] = vertLoc + 4;
                triangles[triangleLoc + 5] = vertLoc + 5;

                if (Mathf.Abs(((bim[x + 1, z + 1] - bim[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((bim[x, z + 1] - bim[x, z]) / 2)) <= stoneGradient)
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

                if (Mathf.Abs(((bim[x + 1, z] - bim[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((bim[x + 1, z + 1] - bim[x, z]) / 2)) <= stoneGradient)
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
                
                vertices[vertLoc + 2] = new Vector3(x + 1, bim[x + 1, z] + ((bim[x + 1, z + 1] - bim[x + 1, z]) / 2), z + 0.5f);
                vertices[vertLoc + 1] = new Vector3(x + 1, bim[x + 1, z + 1], z + 1);
                vertices[vertLoc] = new Vector3(x + 0.5f, bim[x, z] + ((bim[x + 1, z + 1] - bim[x, z]) / 2), z + 0.5f);
                
                vertices[vertLoc + 5] = new Vector3(x + 1, bim[x + 1, z + 1], z + 1);
                vertices[vertLoc + 4] = new Vector3(x + 0.5f, bim[x, z + 1] + ((bim[x + 1, z + 1] - bim[x, z + 1]) / 2), z + 1);
                vertices[vertLoc + 3] = new Vector3(x + 0.5f, bim[x, z] + ((bim[x + 1, z + 1] - bim[x, z]) / 2), z + 0.5f);

                triangles[triangleLoc] = vertLoc;
                triangles[triangleLoc + 1] = vertLoc + 1;
                triangles[triangleLoc + 2] = vertLoc + 2;

                triangles[triangleLoc + 3] = vertLoc + 3;
                triangles[triangleLoc + 4] = vertLoc + 4;
                triangles[triangleLoc + 5] = vertLoc + 5;

                if (Mathf.Abs(((bim[x + 1, z + 1] - bim[x + 1, z]) / 2)) <= stoneGradient || Mathf.Abs(((bim[x + 1, z + 1] - bim[x, z]) / 2)) <= stoneGradient)
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

                if (Mathf.Abs(((bim[x + 1, z + 1] - bim[x, z + 1]) / 2)) <= stoneGradient || Mathf.Abs(((bim[x + 1, z + 1] - bim[x, z]) / 2)) <= stoneGradient)
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

                vertices[vertLoc] = new Vector3(x, bim[x, z] + ((bim[x, z + 1] - bim[x, z]) / 2), z + 0.5f);
                vertices[vertLoc + 1] = new Vector3(x, bim[x, z + 1], z + 1);
                vertices[vertLoc + 2] = new Vector3(x + 0.5f, bim[x, z + 1] + ((bim[x + 1, z + 1] - bim[x, z + 1]) / 2), z + 1);

                vertices[vertLoc + 3] = new Vector3(x, bim[x, z] + ((bim[x, z + 1] - bim[x, z]) / 2), z + 0.5f);
                vertices[vertLoc + 4] = new Vector3(x + 0.5f, bim[x, z + 1] + ((bim[x + 1, z + 1] - bim[x, z + 1]) / 2), z + 1);
                vertices[vertLoc + 5] = new Vector3(x + 0.5f, bim[x, z] + ((bim[x + 1, z + 1] - bim[x, z]) / 2), z + 0.5f);

                triangles[triangleLoc] = vertLoc;
                triangles[triangleLoc + 1] = vertLoc + 1;
                triangles[triangleLoc + 2] = vertLoc + 2;

                triangles[triangleLoc + 3] = vertLoc + 3;
                triangles[triangleLoc + 4] = vertLoc + 4;
                triangles[triangleLoc + 5] = vertLoc + 5;

                if (Mathf.Abs(((bim[x, z + 1] - bim[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((bim[x + 1, z + 1] - bim[x, z + 1]) / 2)) <= stoneGradient)
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

                if (Mathf.Abs(((bim[x, z + 1] - bim[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((bim[x + 1, z + 1] - bim[x, z + 1]) / 2)) <= stoneGradient || Mathf.Abs(((bim[x + 1, z + 1] - bim[x, z]) / 2)) <= stoneGradient)
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

                vertices[vertLoc] = new Vector3(x + 0.5f, bim[x, z] + ((bim[x + 1, z] - bim[x, z]) / 2), z);
                vertices[vertLoc + 1] = new Vector3(x + 0.5f, bim[x, z] + ((bim[x + 1, z + 1] - bim[x, z]) / 2), z + 0.5f);
                vertices[vertLoc + 2] = new Vector3(x + 1, bim[x + 1, z] + ((bim[x + 1, z + 1] - bim[x + 1, z]) / 2), z + 0.5f);

                vertices[vertLoc + 3] = new Vector3(x + 0.5f, bim[x, z] + ((bim[x + 1, z] - bim[x, z]) / 2), z);
                vertices[vertLoc + 4] = new Vector3(x + 1, bim[x + 1, z] + ((bim[x + 1, z + 1] - bim[x + 1, z]) / 2), z + 0.5f);
                vertices[vertLoc + 5] = new Vector3(x + 1, bim[x + 1, z], z);

                triangles[triangleLoc] = vertLoc;
                triangles[triangleLoc + 1] = vertLoc + 1;
                triangles[triangleLoc + 2] = vertLoc + 2;

                triangles[triangleLoc + 3] = vertLoc + 3;
                triangles[triangleLoc + 4] = vertLoc + 4;
                triangles[triangleLoc + 5] = vertLoc + 5;

                if (Mathf.Abs(((bim[x + 1, z] - bim[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((bim[x + 1, z + 1] - bim[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((bim[x + 1, z + 1] - bim[x + 1, z]) / 2)) <= stoneGradient)
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

                if (Mathf.Abs(((bim[x + 1, z] - bim[x, z]) / 2)) <= stoneGradient || Mathf.Abs(((bim[x + 1, z + 1] - bim[x + 1, z]) / 2)) <= stoneGradient)
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
        /*
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
        */
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

        Mesh landMesh = new Mesh();
        landMesh.Clear();
        landMesh.vertices = vertices;
        landMesh.triangles = triangles;

        Mesh[] meshes = { landMesh, mesh };

        return meshes;
}
    Vector2 GetTexture(int k, int y, int w, int s, int c)
    {
        //References the TerrainSettings script to get the block texture based on the side in question and the block id
        //return ts.textures[(int)ts.blockIDs[blockIDMap[k, y, w] - 1, s].x, (int)ts.blockIDs[blockIDMap[k, y, w] - 1, s].y, c];
        return new Vector2();
    }
}
