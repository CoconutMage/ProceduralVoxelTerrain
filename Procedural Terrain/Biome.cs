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
    public int layers;
    public int[] layerDepth;
    public int[] layerId;
    public int treeType;
    public float riverFreq;
    public float treeFreq;

    public Biome(float amp, float freq, string name, Vector3 avg, Vector3 min, Vector3 max)
    {
        amplitude = amp;
        frequency = freq;
        biomeName = name;
        average = avg;
        minimum = min;
        maximum = max;
    }
    public void AddSettings(int l, int tree, float rf, float tf)
    {
        layers = l;
        treeType = tree;
        riverFreq = rf;
        treeFreq = tf;
    }
    public void DefineLayers(int[] ld, int[] li)
    {
        layerDepth = ld;
        layerId = li;
    }
}