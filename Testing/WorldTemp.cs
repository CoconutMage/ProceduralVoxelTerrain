using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldTemp : MonoBehaviour
{
    float tempCelsius;
    float tempFahrenheit;
    Vector3 playerPos;
    void Start()
    {
        tempCelsius = 0;
        tempFahrenheit = 0;
    }
    void Update()
    {
        playerPos = GameObject.Find("Player").transform.position;
        tempCelsius = Mathf.Cos(playerPos.z / 1000) * 43.455f - 17;
        tempFahrenheit = (tempCelsius * (9 / 5)) + 32;
        GameObject.Find("Temp Text").GetComponent<Text>().text = ("Fahrenheit: " + tempFahrenheit + "\n" + "Celsius: " + tempCelsius);
        GameObject.Find("Latitude").GetComponent<Text>().text = ("Z Pos: " + playerPos.z);
    }
}