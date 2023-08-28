using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class settingsMenu : MonoBehaviour
{
    [HideInInspector] public float musicVolume = 1;
    [HideInInspector] public float SFXVolume = 1;
    public enum Graphics
    {
        Smash,
        Spash,
        Pass
    };
    [HideInInspector] public Graphics graphics = Graphics.Smash;
    [HideInInspector] public bool showFPS = false;
    [HideInInspector] public bool rainbowMode = false;
    float spotOriginalIntensity;
    float globalOriginalIntensity;
    TextMeshProUGUI FPS;

    void Awake()
    {
        if(GameObject.Find("FPS"))
            FPS = GameObject.Find("FPS").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        //If more lights are added this code won't work!
        spotOriginalIntensity = GameObject.Find("Spot").GetComponent<Light2D>().intensity;
        globalOriginalIntensity = GameObject.Find("Global").GetComponent<Light2D>().intensity;

        if (PlayerPrefs.HasKey("musicVolume"))
            musicVolume = PlayerPrefs.GetFloat("musicVolume");

        if (PlayerPrefs.HasKey("SFXVolume"))
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume");

        if (PlayerPrefs.HasKey("Graphics"))
            graphics = (Graphics)PlayerPrefs.GetInt("Graphics");

        if (PlayerPrefs.HasKey("showFPS"))
            showFPS = PlayerPrefs.GetInt("showFPS") == 1;

        if (PlayerPrefs.HasKey("rainbowMode"))
            rainbowMode = PlayerPrefs.GetInt("rainbowMode") == 1;

        setGraphics(graphics);

        if (GameObject.Find("Music2"))
            GameObject.Find("Music2").GetComponent<AudioSource>().volume = musicVolume;

        if (GameObject.Find("FPS"))
            GameObject.Find("FPS").GetComponent<TextMeshProUGUI>().enabled = showFPS;

        if (GameObject.Find("Music Volume Slider"))
            GameObject.Find("Music Volume Slider").GetComponent<Slider>().value = musicVolume;

        if (GameObject.Find("SFX Volume Slider"))
            GameObject.Find("SFX Volume Slider").GetComponent<Slider>().value = SFXVolume;

        if (GameObject.Find("Graphics Dropdown"))
            GameObject.Find("Graphics Dropdown").GetComponent<TMP_Dropdown>().value = (int)graphics;

        if (GameObject.Find("Show FPS Toggle"))
            GameObject.Find("Show FPS Toggle").GetComponent<Toggle>().isOn = showFPS;

        if (GameObject.Find("Rainbow Mode Toggle"))
            GameObject.Find("Rainbow Mode Toggle").GetComponent<Toggle>().isOn = rainbowMode;
    }

    void Update()
    {
        if(GetComponent<CanvasGroup>())
            GetComponent<CanvasGroup>().interactable = GetComponent<CanvasGroup>().alpha == 1;
    }

    public void onMusicVolumeChange(float change)
    {
        musicVolume = change;
        PlayerPrefs.SetFloat("musicVolume", change);

        if (GameObject.Find("Music2"))
            GameObject.Find("Music2").GetComponent<AudioSource>().volume = musicVolume;
    }

    public void onSFXVolumeChange(float change)
    {
        SFXVolume = change;
        PlayerPrefs.SetFloat("SFXVolume", change);
    }

    void setGraphics(Graphics graphics)
    {
        switch (graphics)
        {
            case Graphics.Smash:
                QualitySettings.SetQualityLevel(0, true);
                GameObject.Find("Global Volume").GetComponent<Volume>().enabled = true;
                //If more lights are added this code won't work!
                GameObject.Find("Global").GetComponent<Light2D>().enabled = true;
                GameObject.Find("Spot").GetComponent<Light2D>().enabled = true;
                GameObject.Find("Spot").GetComponent<Light2D>().intensity = spotOriginalIntensity;
                GameObject.Find("Global").GetComponent<Light2D>().intensity = globalOriginalIntensity;
                break;

            case Graphics.Spash:
                QualitySettings.SetQualityLevel(0, true);
                GameObject.Find("Global Volume").GetComponent<Volume>().enabled = false;
                //If more lights are added this code won't work!
                GameObject.Find("Global").GetComponent<Light2D>().enabled = true;
                GameObject.Find("Spot").GetComponent<Light2D>().enabled = true;
                GameObject.Find("Spot").GetComponent<Light2D>().intensity = spotOriginalIntensity / 2;
                GameObject.Find("Global").GetComponent<Light2D>().intensity = globalOriginalIntensity / 2;
                break;

            case Graphics.Pass:
                QualitySettings.SetQualityLevel(0, true);
                GameObject.Find("Global Volume").GetComponent<Volume>().enabled = false;
                //If more lights are added this code won't work!
                GameObject.Find("Global").GetComponent<Light2D>().enabled = false;
                GameObject.Find("Spot").GetComponent<Light2D>().enabled = false;
                break;
        }
    }

    public void onGraphicsChange(int change)
    {
        graphics = (Graphics)change;
        PlayerPrefs.SetInt("Graphics", change);

        setGraphics(graphics);
    }

    public void onShowFPSChange(bool change)
    {
        showFPS = change;
        PlayerPrefs.SetInt("showFPS", change ? 1 : 0);
        FPS.enabled = showFPS;
    }

    public void onRainbowModeChange(bool change)
    {
        rainbowMode = change;
        PlayerPrefs.SetInt("rainbowMode", change ? 1 : 0);
    }
}