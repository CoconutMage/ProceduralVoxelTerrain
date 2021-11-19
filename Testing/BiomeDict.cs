using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeDict
{
    public Biome[] biomes;
    public int numBiomes;
    public BiomeDict()
    {
        numBiomes = 9;
        biomes = new Biome[numBiomes];

        //Biomes are added to the registry here
        //Be careful using max and min values for biomes because if ranges given arent comprehensive enough it leaves holes in the biome map where there is no assigned biome
        //The biome handler will output an error message to the console and create black regions on the map if 
        //Temperature, rain, and magic can range from -35->55, 0->100, and 0->100 respectively
        //Arguments for new biomes are information about biomes used for terrain generation and are as follows
        //Amplitude, Frequency, Name, Average; temp, rain, and magic, Minimum; temp, rain, magic, Maximum; temp, rain, magic
        //Set min/max to -999 to use default min/max as determined by the biome handler
        biomes[0] = new Biome(10, 1.3f, "Oak Forest", new Vector3(20, 50, 20), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        biomes[1] = new Biome(10, 1.3f, "Birch Forest", new Vector3(10, 46, 25), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        biomes[2] = new Biome(10, 1.3f, "Desert", new Vector3(30, 25, 20), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        biomes[3] = new Biome(10, 1.3f, "Magic Desert", new Vector3(30, 25, 80), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        biomes[4] = new Biome(10, 1.3f, "Magic Forest", new Vector3(20, 50, 80), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        biomes[5] = new Biome(10, 1.3f, "Magic Tundra", new Vector3(-15, 30, 80), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        biomes[6] = new Biome(10, 1.3f, "Tundra", new Vector3(-15, 30, 20), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        biomes[7] = new Biome(10, 1.3f, "Pine Forest", new Vector3(-5, 30, 20), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        biomes[8] = new Biome(10, 1.3f, "Swamp", new Vector3(65, 80, 40), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));

        Name("Oak Forest").AddSettings(1, 13, 13, 3);
    }
    public int GetLength()
    {
        return biomes.Length;
    }
    public Biome Name(string s)
    {
        for (int i = 0; i < biomes.Length; i++)
        {
            if (biomes[i].biomeName == s) return biomes[i];
        }
        return null;
    }
}