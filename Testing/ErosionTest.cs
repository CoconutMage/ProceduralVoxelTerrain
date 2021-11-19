using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErosionTest : MonoBehaviour
{
    TerrainSettings ts;
    float frequencyMultiplier;
    float amplitudeMultiplier;
    //Max size 4095 chunks, or 65520 blocks
    ushort mapSizeChunks;
    ushort mapSizeBlocks;
    float xOffSet;
    float yOffSet;
    byte[,] heightMap;
    float heightAtPos;
    float endTime;
    float startTime;
    float percentageOfMaxMapSize;
    void Start()
    {
        ts = TerrainSettings.getInstance();
        frequencyMultiplier = ts.frequencyMultiplier;
        amplitudeMultiplier = ts.amplitudeMultiplier;
        //Max size 4095 chunks, or 65520 blocks
        mapSizeChunks = 256;
        mapSizeBlocks = (ushort)(mapSizeChunks * 16);
        xOffSet = 0;
        yOffSet = 0;
        heightMap = new byte[mapSizeBlocks, mapSizeBlocks];
        heightAtPos = 0;
        percentageOfMaxMapSize = (mapSizeChunks / 4095.0f) * 100.0f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GetData();
        }
    }
    void GetData()
    {
        startTime = Time.realtimeSinceStartup;
        for (float i = 0; i < mapSizeBlocks; i++)
        {
            for (float j = 0; j < mapSizeBlocks; j++)
            {
                for (int k = 0; k < ts.octave; k++)
                {
                    heightAtPos += (Mathf.PerlinNoise(xOffSet + i / (mapSizeBlocks) * frequencyMultiplier * (Mathf.Pow(ts.lacunarity, k)), yOffSet + j / (mapSizeBlocks) * frequencyMultiplier * (Mathf.Pow(ts.lacunarity, k)))) * Mathf.Pow(ts.persistance, k);
                }
                heightMap[(int)i, (int)j] = (byte)(amplitudeMultiplier * heightAtPos);
            }
        }
        endTime = Time.realtimeSinceStartup;
        Debug.Log("Done obtaining height map for entire world");
        Debug.Log("Time taken for " + percentageOfMaxMapSize.ToString("F2") + "% of max map size: " + (endTime - startTime));
    }
}