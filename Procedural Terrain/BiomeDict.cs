using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeDict
{
    private static BiomeDict _instance = null;

    public Biome[] biomes;
    public int numBiomes;
    private BiomeDict()
    {
        numBiomes = 4;
        biomes = new Biome[numBiomes];

        //Biomes are added to the registry here
        //Be careful using max and min values for biomes because if ranges given arent comprehensive enough it leaves holes in the biome map where there is no assigned biome
        //The biome handler will output an error message to the console and create black regions on the map if 
        //Temperature, rain, and magic can range from -35->55, 0->100, and 0->100 respectively
        //Arguments for new biomes are information about biomes used for terrain generation and are as follows
        //Amplitude, Frequency, Name, Average; temp, rain, and magic, Minimum; temp, rain, magic, Maximum; temp, rain, magic
        //Set min/max to -999 to use default min/max as determined by the biome handler
        biomes[0] = new Biome(13, 1.5f, "Oak Forest", new Vector3(20, 50, 20), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        biomes[1] = new Biome(13, 1.5f, "Birch Forest", new Vector3(0, 46, 25), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        biomes[2] = new Biome(13, 1.5f, "Desert", new Vector3(30, 25, 40), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        biomes[3] = new Biome(10, 1.3f, "Magic Desert", new Vector3(30, 25, 80), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        //biomes[3] = new Biome(13, 1.5f, "Magic Forest", new Vector3(20, 50, 80), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        //biomes[5] = new Biome(10, 1.3f, "Magic Tundra", new Vector3(-15, 30, 80), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        //biomes[4] = new Biome(13, 1.5f, "Tundra", new Vector3(-15, 30, 20), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        //biomes[5] = new Biome(13, 1.5f, "Pine Forest", new Vector3(-5, 30, 20), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));
        //biomes[8] = new Biome(10, 1.3f, "Swamp", new Vector3(85, 85, 40), new Vector3(-999, -999, -999), new Vector3(-999, -999, -999));

        //Number of layers, tree type, river frequency, and tree frequency
        //Depth of each layer in blocks; -1 means continue till bottom; Block ID of each layer
        Name("Oak Forest").AddSettings(3, 3, /*65.0f*/ 100.0f, /*2.5f*/ 2.0f);
        Name("Oak Forest").DefineLayers(new int[] { 1, 2, -1 }, new int[] { 1, 12, 2 });

        Name("Birch Forest").AddSettings(3, 9, 60.0f, 2.0f);
        Name("Birch Forest").DefineLayers(new int[] { 1, 2, -1 }, new int[] { 1, 12, 2 });

        Name("Desert").AddSettings(2, 14, 5.5f, 0.5f);
        Name("Desert").DefineLayers(new int[] { 4, -1 }, new int[] { 13, 2 });

        Name("Magic Desert").AddSettings(2, 3, 5.5f, 0.5f);
        Name("Magic Desert").DefineLayers(new int[] { 4, -1 }, new int[] { 13, 2 });
    }
    public static BiomeDict getInstance()
    {
        //This code allows for there to only ever be one instance of this script as to not have duplicate settings files
        //If a script tries to create another one it will just be given this script
        //Files of this nature are called Singletons
        if (_instance == null) _instance = new BiomeDict();
        return _instance;
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
    public string GetName(int id)
    {
        return biomes[id].biomeName;
    }
}