using UnityEngine;
using System.IO;

public class ChunkData : MonoBehaviour
{
    TerrainGenerator tg;
    ChunkRenderer cr;
    Vector2 chunkPos;
    float[,] chunkBlockData;
    Mesh[] meshes;
    bool loaded;
    void Start()
    {
        tg = GameObject.Find("Terrain Generator").GetComponent<TerrainGenerator>();
        cr = GameObject.Find("Terrain Generator").GetComponent<ChunkRenderer>();
        chunkPos = new Vector2(float.Parse(name.Split(':')[1]), float.Parse(name.Split(':')[2]));
    }
    void WriteChunkData(int i, int j)
    {
        /*string path = ("Assets/Resources/Chunks/ChunkData-" + i + "-" + j + ".txt");

        using (StreamWriter sw = File.CreateText(path))
        {
            for (int x = (i * 16); x < 16 + (i * 16); x++)
            {
                for (int y = 0; y < 512; y++)
                {
                    for (int z = (j * 16); z < 16 + (j * 16); z++)
                    {
                        sw.Write(chunkBlockData[x, y, z]);
                        sw.Write(",");
                    }
                }
            }
        }
        */
    }
    void ReadChunkData()
    {

    }
    public void LoadChunk(int ticket)
    {
        if (loaded == true) return;
        if (ticket == 3)
        {
            chunkBlockData = tg.GenSingleChunk(chunkPos.x, chunkPos.y);
            meshes = cr.RenderChunk(chunkBlockData);

            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<MeshCollider>().enabled = true;

            GetComponent<MeshFilter>().mesh = meshes[1];
            GetComponent<MeshCollider>().sharedMesh = meshes[0];

            loaded = true;
        }
        else if (ticket == 2)
        {
            chunkBlockData = tg.GenSingleChunk(chunkPos.x, chunkPos.y);
            meshes = cr.RenderChunk(chunkBlockData);

            GetComponent<MeshFilter>().mesh = meshes[1];
            GetComponent<MeshCollider>().sharedMesh = meshes[0];
        }
    }
    public void UnLoadChunk()
    {
        if (loaded == false) return;

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        loaded = false;
    }
}