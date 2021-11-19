using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiomeMap : MonoBehaviour
{
    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    public float waterHeight;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;
    float randRange;

    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    Vector3[] voronoiPoints;
    Color[] regionColors;

    float[,] heightMap;
    float[,] temperatureMap;
    float[,] magicDensityMap;
    float[,] rainMap;
    int[,] biomesMap;
    int[,,] whittakerDiagram;
    BiomeDict biomeDict;
    int biomeSelected;

    Vector2 heightStart;
    Vector2 tempStart;
    Vector2 magicStart;
    Vector2 rainStart;


    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond * this.GetInstanceID());

        heightMap = new float[pixWidth, pixHeight];
        temperatureMap = new float[pixWidth, pixHeight];
        magicDensityMap = new float[pixWidth, pixHeight];
        rainMap = new float[pixWidth, pixHeight];
        biomesMap = new int[pixWidth, pixHeight];
        biomeDict = BiomeDict.getInstance();

        //Temperature range is -35 degrees to 55 degrees, a difference of 90 degrees
        //Rain and Magic values are from 0 to 100
        //Values for array dimensions are axisSize + 1 to accomidate situations where the perlin noise may return 1
        whittakerDiagram = new int[91, 101, 101];
        voronoiPoints = new Vector3[biomeDict.GetLength()];
        regionColors = new Color[biomeDict.GetLength()];

        for (int i = 0; i < biomeDict.GetLength(); i++)
        {
            voronoiPoints[i] = new Vector3(biomeDict.biomes[i].average.x, biomeDict.biomes[i].average.y, biomeDict.biomes[i].average.z);
        }
        regionColors[0] = new Color(0, 1, 0);
        regionColors[1] = new Color(0.5f, 0.93f, 0.5f);
        regionColors[2] = new Color(1, 1, 0);
        regionColors[3] = new Color(0.6f, 0.6f, 0);
        //regionColors[3] = new Color(0, 0.4f, 0);
        //regionColors[5] = new Color(0, 0.6f, 0.6f);
        //regionColors[4] = new Color(0, 1, 1);
        //regionColors[5] = new Color(0, 0.3f, 0.1f);
        //regionColors[8] = new Color(0, 0.2f, 0.1f);

        rend = GetComponent<Renderer>();

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;
        randRange = 99999;

        CalcClimateMaps();

        //CalcHeightMap();
        //CalcTempMap();
        //CalcRainMap();
        //CalcMagicMap();
        CalcWhittakar();
        CalcBiomes();
    }
    void CalcClimateMaps()
    {
        heightStart = new Vector2(Random.Range(-randRange, randRange), Random.Range(-randRange, randRange));
        tempStart = new Vector2(Random.Range(-randRange, randRange), Random.Range(-randRange, randRange));
        rainStart = new Vector2(Random.Range(-randRange, randRange), Random.Range(-randRange, randRange));
        magicStart = new Vector2(Random.Range(-randRange, randRange), Random.Range(-randRange, randRange));

        for (float y = 0; y < noiseTex.height; y++)
        {
            for (float x = 0; x < noiseTex.width; x++)
            {
                heightMap[(int)x, (int)y] = Mathf.PerlinNoise(heightStart.x + x / noiseTex.width * scale, heightStart.y + y / noiseTex.height * scale) + (Mathf.PerlinNoise(heightStart.x + x / noiseTex.width * scale * 2, heightStart.y + y / noiseTex.height * scale * 2) * 0.5f) + (Mathf.PerlinNoise(heightStart.x + x / noiseTex.width * scale * 4, heightStart.y + y / noiseTex.height * scale * 4) * 0.25f);
                
                float temp = 35 * Mathf.Cos(Mathf.Deg2Rad * ((y - (noiseTex.height / 2)) * 1.40625f) * 2) + 10;
                float sample = Mathf.PerlinNoise(tempStart.x + x / noiseTex.width * scale, tempStart.y + y / noiseTex.height * scale) + (Mathf.PerlinNoise(tempStart.x + x / noiseTex.width * scale * 2, tempStart.y + y / noiseTex.height * scale * 2) * 0.5f) + (Mathf.PerlinNoise(tempStart.x + x / noiseTex.width * scale * 4, tempStart.y + y / noiseTex.height * scale * 4) * 0.25f);

                if (sample > 1.0f) sample = 1.0f;
                else if (sample < 0.0f) sample = 0.0f;

                temp += (sample - 0.5f) * 20;

                temperatureMap[(int)x, (int)y] = temp;

                rainMap[(int)x, (int)y] = 100 * (Mathf.PerlinNoise(rainStart.x + x / noiseTex.width * scale, rainStart.y + y / noiseTex.height * scale) + (Mathf.PerlinNoise(rainStart.x + x / noiseTex.width * scale * 2, rainStart.y + y / noiseTex.height * scale * 2) * 0.5f) + (Mathf.PerlinNoise(rainStart.x + x / noiseTex.width * scale * 4, rainStart.y + y / noiseTex.height * scale * 4) * 0.25f));
                if (rainMap[(int)x, (int)y] > 100.0f) rainMap[(int)x, (int)y] = 100.0f;
                else if (rainMap[(int)x, (int)y] < 0.0f) rainMap[(int)x, (int)y] = 0.0f;

                magicDensityMap[(int)x, (int)y] = 100 * Mathf.PerlinNoise(magicStart.x + x / noiseTex.width * scale, magicStart.y + y / noiseTex.height * scale) + (Mathf.PerlinNoise(magicStart.x + x / noiseTex.width * scale * 2, magicStart.y + y / noiseTex.height * scale * 2) * 0.5f) + (Mathf.PerlinNoise(magicStart.x + x / noiseTex.width * scale * 4, magicStart.y + y / noiseTex.height * scale * 4) * 0.25f);
                if (magicDensityMap[(int)x, (int)y] > 100.0f) magicDensityMap[(int)x, (int)y] = 100.0f;
                else if (magicDensityMap[(int)x, (int)y] < 0.0f) magicDensityMap[(int)x, (int)y] = 0.0f;
            }
        }
    }
    void CalcHeightMap()
    {
        heightStart = new Vector2(Random.Range(-randRange, randRange), Random.Range(-randRange, randRange));

        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                heightMap[(int)x, (int)y] = Mathf.PerlinNoise(heightStart.x + x / noiseTex.width * scale, heightStart.y + y / noiseTex.height * scale) + (Mathf.PerlinNoise(heightStart.x + x / noiseTex.width * scale * 2, heightStart.y + y / noiseTex.height * scale * 2) * 0.5f) + (Mathf.PerlinNoise(heightStart.x + x / noiseTex.width * scale * 4, heightStart.y + y / noiseTex.height * scale * 4) * 0.25f);
                x++;
            }
            y++;
        }
    }
    void CalcTempMap()
    {
        tempStart = new Vector2(Random.Range(-randRange, randRange), Random.Range(-randRange, randRange));
        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float lat = y - (noiseTex.height / 2);
                float temp = 35 * Mathf.Cos(Mathf.Deg2Rad * (lat * 1.40625f) * 2) + 10;
                float sample = Mathf.PerlinNoise(tempStart.x + x / noiseTex.width * scale, tempStart.y + y / noiseTex.height * scale) + (Mathf.PerlinNoise(tempStart.x + x / noiseTex.width * scale * 2, tempStart.y + y / noiseTex.height * scale * 2) * 0.5f) + (Mathf.PerlinNoise(tempStart.x + x / noiseTex.width * scale * 4, tempStart.y + y / noiseTex.height * scale * 4) * 0.25f);

                if (sample > 1.0f) sample = 1.0f;
                else if (sample < 0.0f) sample = 0.0f;

                sample -= 0.5f;
                sample *= 20;
                temp += sample;

                temperatureMap[(int)x, (int)y] = temp;
                x++;
            }
            y++;
        }
    }
    void CalcMagicMap()
    {
        magicStart = new Vector2(Random.Range(-randRange, randRange), Random.Range(-randRange, randRange));
        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                magicDensityMap[(int)x, (int)y] = 100 * Mathf.PerlinNoise(magicStart.x + x / noiseTex.width * scale, magicStart.y + y / noiseTex.height * scale) + (Mathf.PerlinNoise(magicStart.x + x / noiseTex.width * scale * 2, magicStart.y + y / noiseTex.height * scale * 2) * 0.5f) + (Mathf.PerlinNoise(magicStart.x + x / noiseTex.width * scale * 4, magicStart.y + y / noiseTex.height * scale * 4) * 0.25f);
                if (magicDensityMap[(int)x, (int)y] > 100.0f) magicDensityMap[(int)x, (int)y] = 100.0f;
                else if (magicDensityMap[(int)x, (int)y] < 0.0f) magicDensityMap[(int)x, (int)y] = 0.0f;
                x++;
            }
            y++;
        }
    }
    void CalcRainMap()
    {
        rainStart = new Vector2(Random.Range(-randRange, randRange), Random.Range(-randRange, randRange));
        
        for (float y = 0; y < noiseTex.height; y++)
        {
            for (float x = 0; x < noiseTex.width; x++)
            {
                rainMap[(int)x, (int)y] = 100 * (Mathf.PerlinNoise(rainStart.x + x / noiseTex.width * scale, rainStart.y + y / noiseTex.height * scale) + (Mathf.PerlinNoise(rainStart.x + x / noiseTex.width * scale * 2, rainStart.y + y / noiseTex.height * scale * 2) * 0.5f) + (Mathf.PerlinNoise(rainStart.x + x / noiseTex.width * scale * 4, rainStart.y + y / noiseTex.height * scale * 4) * 0.25f));
                if (rainMap[(int)x, (int)y] > 100.0f) rainMap[(int)x, (int)y] = 100.0f;
                else if (rainMap[(int)x, (int)y] < 0.0f) rainMap[(int)x, (int)y] = 0.0f;
            }
        }
    }
    void CalcBiomes()
    {
        for (int y = 0; y < noiseTex.height; y++)
        {
            for (int x = 0; x < noiseTex.width; x++)
            {
                if (whittakerDiagram[(int)(temperatureMap[x, y] + 35), (int)rainMap[x, y], (int)magicDensityMap[x, y]] == -1 && heightMap[x, y] >= waterHeight)
                {
                    pix[y * noiseTex.width + x] = Color.black;
                    continue;
                }
                if (heightMap[x, y] >= waterHeight)
                {
                    pix[y * noiseTex.width + x] = regionColors[whittakerDiagram[(int)(temperatureMap[x, y] + 35), (int)rainMap[x, y], (int)magicDensityMap[x, y]]];
                    biomesMap[x, y] = whittakerDiagram[(int)(temperatureMap[x, y] + 35), (int)rainMap[x, y], (int)magicDensityMap[x, y]];
                }
                else pix[y * noiseTex.width + x] = Color.blue;
            }
        }

        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }
    void CalcWhittakar()
    {
        float z = 0.0f;
        while (z < whittakerDiagram.GetLength(2))
        {
            float y = 0.0F;
            while (y < whittakerDiagram.GetLength(1))
            {
                float x = 0.0F;
                while (x < whittakerDiagram.GetLength(0))
                {
                    float lowestDistance = 99999999;
                    whittakerDiagram[(int)x, (int)y, (int)z] = -1;

                    for (int i = 0; i < biomeDict.GetLength(); i++)
                    {
                        if ((biomeDict.biomes[i].minimum.x != -999 && biomeDict.biomes[i].minimum.x > (x - 35)) || (biomeDict.biomes[i].minimum.y != -999 && biomeDict.biomes[i].minimum.y > y) || (biomeDict.biomes[i].minimum.z != -999 && biomeDict.biomes[i].minimum.z > z))
                        {
                            continue;
                        }
                        if ((biomeDict.biomes[i].maximum.x != -999 && biomeDict.biomes[i].maximum.x < (x - 35)) || (biomeDict.biomes[i].maximum.y != -999 && biomeDict.biomes[i].maximum.y < y) || (biomeDict.biomes[i].maximum.z != -999 && biomeDict.biomes[i].maximum.z < z))
                        {
                            continue;
                        }
                        //For a voronoi points temperature value (x) offset by 35 to compensate for negative temperatures
                        if (lowestDistance > Mathf.Sqrt(Mathf.Pow(((voronoiPoints[i].x + 35) - x), 2) + Mathf.Pow((voronoiPoints[i].y - y), 2) + Mathf.Pow((voronoiPoints[i].z - z), 2)))
                        {
                            whittakerDiagram[(int)x, (int)y, (int)z] = i;
                            lowestDistance = Mathf.Sqrt(Mathf.Pow(((voronoiPoints[i].x + 35) - x), 2) + Mathf.Pow((voronoiPoints[i].y - y), 2) + Mathf.Pow((voronoiPoints[i].z - z), 2));
                        }
                    }
                    if (whittakerDiagram[(int)x, (int)y, (int)z] == -1) Debug.Log("Input not in function domain");
                    x++;
                }
                y++;
            }
            z++;
        }
    }
    public void DisplayClicked(Vector3 pos)
    {
        /*
        Debug.Log(noiseTex.GetPixel((int)pos.x, (int)pos.y));
        Debug.Log("X: " + ((int)pos.x) + " Y: " + ((int)pos.y) + " Biome Id: " + biomesMap[(int)pos.x, (int)pos.y]);
        pix[(int)pos.y * noiseTex.width + (int)pos.x] = Color.black;
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
        */

        GameObject.Find("Biome Name").GetComponent<Text>().text = "Selected Biome: " + biomeDict.GetName(biomesMap[(int)pos.x + 19, (int)pos.y]);
        biomeSelected = biomesMap[(int)pos.x + 19, (int)pos.y];
    }
    public void BiomeChosen()
    {
        TerrainSettings ts = TerrainSettings.getInstance();
        ts.SetBiomeSettings(biomeDict.biomes[biomeSelected]);
        GameObject.Find("Terrain Generator").GetComponent<TerrainGenerator>().enabled = true;
        Camera.main.orthographic = false;
        GameObject.Find("Player").GetComponent<PlayerScript>().TerrainGenerated();
        Destroy(GameObject.Find("Biome Name"));
        Destroy(GameObject.Find("Load Map"));
        GameObject.Find("Canvas").gameObject.transform.GetChild(2).gameObject.SetActive(true);
        Destroy(this.gameObject);
    }
}