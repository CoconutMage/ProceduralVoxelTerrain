using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    GameObject canvas;
    GameObject mainMenu;
    GameObject playGame;
    GameObject options;
    SettingsData sd;

    void Start()
    {
        canvas = GameObject.Find("Canvas");
        mainMenu = canvas.transform.GetChild(1).gameObject;
        playGame = canvas.transform.GetChild(2).gameObject;
        options = canvas.transform.GetChild(3).gameObject;
        sd = SettingsData.getInstance();
    }
    public void PlayGameClicked()
    {
        playGame.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void OptionsClicked()
    {
        options.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void BackButtonClicked(int x)
    {
        if (x == 1)
        {
            mainMenu.SetActive(true);
            playGame.SetActive(false);
            options.SetActive(false);
        }
    }
    public void RenderDistanceChanged(float x)
    {
        sd.renderDistance = (int)x;
        options.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = ("Render Distance: " + x);
    }
    public void MusicVolumeChanged(float x)
    {
        sd.musicVolume = (int)x;
        options.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = ("Music Volume: " + x);
    }
    public void SoundVolumeChanged(float x)
    {
        sd.soundVolume = (int)x;
        options.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = ("Sound Volume: " + x);
    }
    public void BrightnessChanged(float x)
    {
        sd.musicVolume = (int)x;
        options.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = ("Music Volume: " + x);
    }
    public void NewGameClicked()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadGameClicked()
    {
        Debug.Log("Load");
    }
    public void MultiplayerClicked()
    {
        Debug.Log("Multiplayer");
    }
}