using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Transform Touch;
    [SerializeField] GameObject finishAnimation;
    [SerializeField] GameObject Canvas;
    [SerializeField] SceneTransitioner sceneTransitioner;
    [SerializeField] GameObject Music;
    [SerializeField] RectTransform creditText;
    [SerializeField] float screenHeight;
    [SerializeField] float creditStepSize;
    [SerializeField] float creditTime;
    settingsMenu settings;

    void Start()
    {
        Application.targetFrameRate = 60;

        if (!PlayerPrefs.HasKey("Level"))
            PlayerPrefs.SetInt("Level", 1);

        settings = GameObject.FindWithTag("settingsMenu").GetComponent<settingsMenu>();

        if (!GameObject.Find("Music2"))
        {
            Music.GetComponent<AudioSource>().volume = settings.musicVolume;
            DontDestroyOnLoad(Music);
            Music.name = "Music2";
        }
        else
            Destroy(Music);
    }

    void Update()
    {
        if (Input.touchCount > 0)
            Touch.position = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        if (Input.GetMouseButton(0))
            Touch.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void play()
    {
        StartCoroutine(sceneTransitioner.loadScene(index: 1, canvas: Canvas.GetComponent<Canvas>()));
    }

    public void Credit()
    {
        StartCoroutine(CreditAnimation());
    }

    IEnumerator CreditAnimation()
    {
        GameObject sbtn = GameObject.Find("Start Button");
        GameObject cbtn = GameObject.Find("Credit Button");
        GameObject title = GameObject.Find("Title");

        sbtn.SetActive(false);
        cbtn.SetActive(false);
        title.SetActive(false);

        float Dist = screenHeight + creditText.sizeDelta.y;
        Vector2 startPos = creditText.anchoredPosition;

        for (float i = 0; i <= Dist; i += creditStepSize)
        {
            creditText.anchoredPosition += Vector2.up * creditStepSize;

            yield return new WaitForSeconds(creditTime / (Dist / creditStepSize));

            if (Input.anyKey)
                break;
        }

        creditText.anchoredPosition = startPos;

        sbtn.SetActive(true);
        cbtn.SetActive(true);
        title.SetActive(true);
    }
}