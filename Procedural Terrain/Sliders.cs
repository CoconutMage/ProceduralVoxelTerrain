using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Sliders : MonoBehaviour
{
    TerrainSettings ts;
    GameObject mapSizeText;
    GameObject frequencyText;
    GameObject amplitudeText;
    GameObject lacunarityText;
    GameObject persistanceText;
    GameObject octaveText;
    GameObject treeFreqText;
    GameObject coalFreqText;
    GameObject ironFreqText;
    GameObject goldFreqText;
    GameObject silverFreqText;
    GameObject crystalFreqText;
    GameObject rockFreqText;
    GameObject waterHeightText;

    void Start()
    {
        ts = TerrainSettings.getInstance();

        mapSizeText = GameObject.Find("Map Size Text");
        frequencyText = GameObject.Find("Frequency Text");
        amplitudeText = GameObject.Find("Amplitude Text");
        lacunarityText = GameObject.Find("Lacunarity Text");
        persistanceText = GameObject.Find("Persistance Text");
        octaveText = GameObject.Find("Octave Text");
        treeFreqText = GameObject.Find("Tree Frequency Text");
        coalFreqText = GameObject.Find("Coal Frequency Text");
        ironFreqText = GameObject.Find("Iron Frequency Text");
        goldFreqText = GameObject.Find("Gold Frequency Text");
        silverFreqText = GameObject.Find("Silver Frequency Text");
        crystalFreqText = GameObject.Find("Crystal Frequency Text");
        rockFreqText = GameObject.Find("Rock Frequency Text");
        waterHeightText = GameObject.Find("Water Height Text");

        mapSizeText.GetComponent<Text>().text = ("Map Size: " + ts.mapSize);
        frequencyText.GetComponent<Text>().text = ("Frequency: " + ts.frequencyMultiplier);
        amplitudeText.GetComponent<Text>().text = ("Amplitude: " + ts.amplitudeMultiplier);
        lacunarityText.GetComponent<Text>().text = ("Lacunarity: " + ts.lacunarity);
        persistanceText.GetComponent<Text>().text = ("Persistance: " + ts.persistance);
        octaveText.GetComponent<Text>().text = ("Octave: " + ts.octave);
        treeFreqText.GetComponent<Text>().text = ("Tree Frequency: " + ts.treeFreq);
        coalFreqText.GetComponent<Text>().text = ("Coal Frequency: " + ts.coalFreq);
        ironFreqText.GetComponent<Text>().text = ("Iron Frequency: " + ts.ironFreq);
        goldFreqText.GetComponent<Text>().text = ("Gold Frequency: " + ts.goldFreq);
        silverFreqText.GetComponent<Text>().text = ("Silver Frequency: " + ts.silverFreq);
        crystalFreqText.GetComponent<Text>().text = ("Crystal Frequency: " + ts.crystalFreq);
        rockFreqText.GetComponent<Text>().text = ("Rock Frequency: " + ts.rockFreq);
        //waterHeightText.GetComponent<Text>().text = ("Water Height: " + ts.waterHeight);

        (mapSizeText.GetComponentInParent(typeof(Slider)) as Slider).value = (float) ts.mapSize;
        (frequencyText.GetComponentInParent(typeof(Slider)) as Slider).value = (float) ts.frequencyMultiplier;
        (amplitudeText.GetComponentInParent(typeof(Slider)) as Slider).value = (float) ts.amplitudeMultiplier;
        (lacunarityText.GetComponentInParent(typeof(Slider)) as Slider).value = (float) ts.lacunarity;
        (persistanceText.GetComponentInParent(typeof(Slider)) as Slider).value = ts.persistance;
        (octaveText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.octave;
        (treeFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.treeFreq;
        (coalFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.coalFreq;
        (ironFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.ironFreq;
        (goldFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.goldFreq;
        (silverFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.silverFreq;
        (crystalFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.crystalFreq;
        (rockFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.rockFreq;
        //(waterHeightText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.waterHeight;

        GameObject.Find("Seed").GetComponent<Toggle>().isOn = ts.keepSeed;
        //GameObject.Find("Water").GetComponent<Toggle>().isOn = ts.waterEnabled;
    }
    public void Regen()
    {
        if (!GameObject.Find("Seed").GetComponent<Toggle>().isOn) ts.seed = System.DateTime.Now.Millisecond * this.GetInstanceID();
        SceneManager.LoadScene(0);
    }
    public void SetMapSize(float size)
    {
        mapSizeText.GetComponent<Text>().text = ("Map Size: " + size);
        ts.mapSize = (int) size;
    }
    public void SetFreq(float freq)
    {
        frequencyText.GetComponent<Text>().text = ("Frequency: " + freq);
        ts.frequencyMultiplier = freq;
    }
    public void SetAmp(float amp)
    {
        amplitudeText.GetComponent<Text>().text = ("Amplitude: " + amp);
        ts.amplitudeMultiplier = amp;
    }
    public void DefaultSettings()
    {
        ts.mapSize = 6;
        ts.frequencyMultiplier = 1.5f;
        ts.amplitudeMultiplier = 10;
        ts.lacunarity = 2;
        ts.persistance = 0.5f;
        ts.octave = 3;
        ts.treeFreq = 2;
        ts.coalFreq = 6;
        ts.ironFreq = 4;
        ts.goldFreq = 2;
        ts.silverFreq = 3;
        ts.crystalFreq = 1;
        ts.rockFreq = 1;
        ts.waterHeight = 1;
        ts.waterEnabled = false;

        mapSizeText.GetComponent<Text>().text = ("Map Size: " + ts.mapSize);
        frequencyText.GetComponent<Text>().text = ("Frequency: " + ts.frequencyMultiplier);
        amplitudeText.GetComponent<Text>().text = ("Amplitude: " + ts.amplitudeMultiplier);
        lacunarityText.GetComponent<Text>().text = ("Lacunarity: " + ts.lacunarity);
        persistanceText.GetComponent<Text>().text = ("Persistance: " + ts.persistance);
        octaveText.GetComponent<Text>().text = ("Octave: " + ts.octave);
        treeFreqText.GetComponent<Text>().text = ("Tree Frequency: " + ts.treeFreq);
        coalFreqText.GetComponent<Text>().text = ("Coal Frequency: " + ts.coalFreq);
        ironFreqText.GetComponent<Text>().text = ("Iron Frequency: " + ts.ironFreq);
        goldFreqText.GetComponent<Text>().text = ("Gold Frequency: " + ts.goldFreq);
        silverFreqText.GetComponent<Text>().text = ("Silver Frequency: " + ts.silverFreq);
        crystalFreqText.GetComponent<Text>().text = ("Crystal Frequency: " + ts.crystalFreq);
        rockFreqText.GetComponent<Text>().text = ("Rock Frequency: " + ts.rockFreq);
        waterHeightText.GetComponent<Text>().text = ("Water Height: " + ts.waterHeight);

        (mapSizeText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.mapSize;
        (frequencyText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.frequencyMultiplier;
        (amplitudeText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.amplitudeMultiplier;
        (lacunarityText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.lacunarity;
        (persistanceText.GetComponentInParent(typeof(Slider)) as Slider).value = ts.persistance;
        (octaveText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.octave;
        (treeFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.treeFreq;
        (coalFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.coalFreq;
        (ironFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.ironFreq;
        (goldFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.goldFreq;
        (silverFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.silverFreq;
        (crystalFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.crystalFreq;
        (rockFreqText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.rockFreq;
        (waterHeightText.GetComponentInParent(typeof(Slider)) as Slider).value = (float)ts.waterHeight;

        GameObject.Find("Seed").GetComponent<Toggle>().isOn = ts.keepSeed;
        GameObject.Find("Water").GetComponent<Toggle>().isOn = ts.waterEnabled;

        Regen();
    }
    public void KeepSeedSwitch()
    {
        ts.keepSeed = GameObject.Find("Seed").GetComponent<Toggle>().isOn;
    }
    public void SetLacunarity(float lacunarity)
    {
        lacunarityText.GetComponent<Text>().text = ("Lacunarity: " + lacunarity);
        ts.lacunarity = (int) lacunarity;
    }
    public void SetPersistance(float persistance)
    {
        persistanceText.GetComponent<Text>().text = ("Persistance: " + persistance);
        ts.persistance = persistance;
    }
    public void SetOctave(float octave)
    {
        octaveText.GetComponent<Text>().text = ("Octave: " + octave);
        ts.octave = (int) octave;
    }
    public void SetTreeFrequency(float treeFreq)
    {
        treeFreqText.GetComponent<Text>().text = ("Tree Frequency: " + treeFreq);
        ts.treeFreq = treeFreq;
    }
    public void SetCoalFrequency(float coalFreq)
    {
        coalFreqText.GetComponent<Text>().text = ("Coal Frequency: " + coalFreq);
        ts.coalFreq = coalFreq;
    }
    public void SetIronFrequency(float ironFreq)
    {
        ironFreqText.GetComponent<Text>().text = ("Iron Frequency: " + ironFreq);
        ts.ironFreq = ironFreq;
    }
    public void SetGoldFrequency(float goldFreq)
    {
        goldFreqText.GetComponent<Text>().text = ("Gold Frequency: " + goldFreq);
        ts.goldFreq = goldFreq;
    }
    public void SetSilverFrequency(float silverFreq)
    {
        silverFreqText.GetComponent<Text>().text = ("Silver Frequency: " + silverFreq);
        ts.silverFreq = silverFreq;
    }
    public void SetCrystalFrequency(float crystalFreq)
    {
        crystalFreqText.GetComponent<Text>().text = ("Crystal Frequency: " + crystalFreq);
        ts.crystalFreq = crystalFreq;
    }
    public void SetRockFrequency(float rockFreq)
    {
        rockFreqText.GetComponent<Text>().text = ("Rock Frequency: " + rockFreq);
        ts.rockFreq = rockFreq;
    }
    public void WaterSwitch()
    {
        ts.waterEnabled = GameObject.Find("Water").GetComponent<Toggle>().isOn;
    }
    public void SetWaterHeight(float waterHeight)
    {
        waterHeightText.GetComponent<Text>().text = ("Water Height: " + waterHeight);
        ts.waterHeight = waterHeight;
    }
}