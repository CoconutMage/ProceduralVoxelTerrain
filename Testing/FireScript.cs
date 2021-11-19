using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireScript : MonoBehaviour
{
    GameObject fireMenuUIObject;
    int tinderStockpile;
    int stickStockpile;
    int logStockpile;
    void Start()
    {
        fireMenuUIObject = GameObject.Find("Canvas").transform.GetChild(6).gameObject;
    }
    void Update()
    {
        fireMenuUIObject.transform.GetChild(0).GetComponent<Text>().text = ("Tinder: x" + tinderStockpile + "\nSticks: x" + stickStockpile + "\nLogs: x" + logStockpile);
    }
    public void AddToPile(int id)
    {
        switch (id)
        {
            case 1:
                tinderStockpile += 1;
                break;
            case 2:
                stickStockpile += 1;
                break;
            case 3:
                logStockpile += 1;
                break;
        }
    }
}