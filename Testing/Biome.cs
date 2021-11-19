using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome
{
    public float amplitude;
    public float frequency;
    public string biomeName;
    public Vector3 average;
    public Vector3 minimum;
    public Vector3 maximum;
    public int topSoilBlockId;
    public int middleSoilBlockId;
    public int bottomSoilBlockId;
    public int treeType;

    public Biome(float amp, float freq, string name, Vector3 avg, Vector3 min, Vector3 max)
    {
        amplitude = amp;
        frequency = freq;
        biomeName = name;
        average = avg;
        minimum = min;
        maximum = max;
    }
    public void AddSettings(int top, int mid, int bot, int tree)
    {
        topSoilBlockId = top;
        middleSoilBlockId = mid;
        bottomSoilBlockId = bot;
        treeType = tree;
    }
}