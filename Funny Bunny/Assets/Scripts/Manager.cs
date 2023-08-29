using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class Manager : MonoBehaviour
{
    [SerializeField] GameObject mochiPrefab;
    [SerializeField] int[] perfectUseCounts;
    [SerializeField] float finishSliderTime;
    [SerializeField] float nonMovableAreaDistThreshold;

    GameObject MenuG;
    GameObject settingsMenu;
    TextMeshProUGUI UsesText;
    [HideInInspector] public GameObject gameUI;
    [HideInInspector] public bool gameStoped = false;

    Transform Touch;
    Transform BG;

    int perfectUseCount;
    int Uses = 0;

    float fpsTimer = 0;
    float highestFPS = 0;
    float highestFPSTimer = 0;

    void Start()
    {
        Camera.main.aspect = 20 / 9f;

        getObjects();
        checkForUseKeys();

        if (Application.platform != RuntimePlatform.Android)
        {
            //gameUI.GetComponent<RectTransform>().anchoredPosition = Vector2.one * 3000;
            //GameObject.FindWithTag("NonMovableArea").SetActive(false);
        }
        else
            GameObject.Find("Buttons").SetActive(false);

        BG.position = Vector3.up * (14.72f - 4 * (SceneManager.GetActiveScene().buildIndex - 2));
    }

    void Update()
    {
        moveTouch();
        fpsText();
        checkForNonMovableArea();

        if (Input.GetKeyDown(KeyCode.C) && gameUI.activeInHierarchy)
            Duplicate();
    }

    void getObjects()
    {
        Touch = GameObject.FindWithTag("Touch").transform;
        BG = GameObject.Find("BG").transform;
        UsesText = GameObject.Find("DupsUsed").GetComponent<TextMeshProUGUI>();
        MenuG = GameObject.FindWithTag("Menu");
        settingsMenu = GameObject.FindWithTag("settingsMenu");
        gameUI = GameObject.FindWithTag("gameUI");

        MenuG.SetActive(false);
        settingsMenu.GetComponent<CanvasGroup>().alpha = 0;
    }

    void checkForUseKeys()
    {
        for (int i = 0; i < perfectUseCounts.Length; i++)
        {
            if (!PlayerPrefs.HasKey($"PerfectUses{i + 1}"))
                PlayerPrefs.SetInt($"PerfectUses{i + 1}", perfectUseCounts[i]);

            if (!PlayerPrefs.HasKey($"Uses{i + 1}"))
                PlayerPrefs.SetInt($"Uses{i + 1}", 2147483640);
        }

        perfectUseCount = perfectUseCounts[SceneManager.GetActiveScene().buildIndex - 2];
        UsesText.text = $"{Uses}/{perfectUseCount}";
    }

    void moveTouch()
    {
        if (Input.touchCount > 0)
            Touch.position = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        if (Input.GetMouseButton(0))
            Touch.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void fpsText()
    {
        fpsTimer += Time.deltaTime;
        if (fpsTimer >= 0.5f)
        {
            float fps = Mathf.Round(1 / Time.deltaTime / 10) * 10;
            if (fps > highestFPS)
                highestFPS = fps;

            if (gameUI.activeInHierarchy)
                GameObject.Find("FPS").GetComponent<TextMeshProUGUI>().text = $"{fps}/{highestFPS}";

            fpsTimer = 0;
            highestFPSTimer += 1;
            if (highestFPSTimer >= 10)
            {
                highestFPS = 0;
                highestFPSTimer = 0;
            }
        }
    }

    void checkForNonMovableArea()
    {
        foreach (object o in FindObjectsByType<Duplicant>(FindObjectsSortMode.None))
        {
            Duplicant d = (Duplicant)o;
            Vector2 joystickScreenPosition = gameUI.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition;
            Vector2 joystickPosition = gameUI.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition / 100;
            float Dist = Vector2.Distance((Vector2)d.transform.position, joystickPosition);
            bool Enable = !(d.held && Dist < nonMovableAreaDistThreshold);
            //Debug.Log($"Duplicant: {d.name}");
            //Debug.Log($"Held: {d.held}");
            //Debug.Log($"Position: {d.transform.position}");
            //Debug.Log($"Joystick Screen Position: {joystickScreenPosition}");
            //Debug.Log($"Joystick Position: {joystickPosition}");
            //Debug.Log($"Distance to Joystick: {Dist}");
            //Debug.Log($"Threshold: {nonMovableAreaDistThreshold}");
            //Debug.Log($"Enabling: {Enable}");

            string n = GameObject.FindWithTag("NonMovableArea").name;
            char newChar = Enable ? '1' : '0';
            char oldChar = n[n.Length - 1];
            char[] NCA = n.ToCharArray();
            NCA[n.Length - 1] = newChar;
            n = new string(NCA);
            //Debug.Log($"Name: {n}");
            //Debug.Log($"Old Char : {oldChar}");
            //Debug.Log($"New Char : {newChar}");
            //Debug.Log($"New Name : {n}");
            GameObject.FindWithTag("NonMovableArea").name = n;
        }
    }

    public void Duplicate()
    {
        if (!GameObject.Find("dupsHolder"))
            return;

        GameObject g = Instantiate(mochiPrefab, GameObject.FindWithTag("Player").transform.position - GameObject.FindWithTag("Player").transform.up * 0.25f, Quaternion.identity);
        g.transform.parent = GameObject.Find("dupsHolder").transform;

        Uses++;
        UsesText.text = $"{Uses}/{perfectUseCount}";

        GetComponent<AudioSource>().volume = settingsMenu.GetComponent<settingsMenu>().SFXVolume;
        GetComponent<AudioSource>().Play();
    }

    public void Menu(string button)
    {
        SceneTransitioner transitioner = GameObject.FindWithTag("SceneTransitioner").GetComponent<SceneTransitioner>();
        Canvas canvas = MenuG.transform.parent.GetComponent<Canvas>();

        switch (button)
        {
            case "reset":
                StartCoroutine(transitioner.loadScene(index: SceneManager.GetActiveScene().buildIndex, canvas: canvas));
                break;

            case "settings":
                StartCoroutine(SetActive(MenuG, !MenuG.activeInHierarchy, 0.25f, "CanvasGroup"));
                StartCoroutine(SetActive(settingsMenu, !gameUI.activeInHierarchy, 0.25f, "CanvasGroup", false));
                break;

            case "levels":
                StartCoroutine(transitioner.loadScene(index: 1, canvas: canvas));
                break;

            case "menu":
                gameStoped = !gameStoped;
                StartCoroutine(SetActive(settingsMenu, false, 0.25f, "CanvasGroup", false));
                if (!MenuG.activeInHierarchy && !gameUI.activeInHierarchy)
                {
                    StartCoroutine(SetActive(MenuG, false, 0.25f, "CanvasGroup"));
                    StartCoroutine(SetActive(gameUI, true, 0.25f, "CanvasGroup"));
                }
                else
                {
                    StartCoroutine(SetActive(MenuG, !MenuG.activeInHierarchy, 0.25f, "CanvasGroup"));
                    StartCoroutine(SetActive(gameUI, !gameUI.activeInHierarchy, 0.25f, "CanvasGroup"));
                }
                break;

            case "next":
                if (SceneManager.sceneCountInBuildSettings > SceneManager.GetActiveScene().buildIndex + 1)
                    StartCoroutine(transitioner.loadScene(index: SceneManager.GetActiveScene().buildIndex + 1, canvas: canvas));
                else
                    StartCoroutine(transitioner.loadScene(index: 1, canvas: canvas));
                break;
        }
    }

    public IEnumerator levelFinished(GameObject finishMenu)
    {
        if(PlayerPrefs.GetInt($"Uses{SceneManager.GetActiveScene().buildIndex - 1}") >= Uses)
            PlayerPrefs.SetInt($"Uses{SceneManager.GetActiveScene().buildIndex - 1}", Uses);

        GameObject.Find("menuButton").SetActive(false);
        GameObject.Find("DupsUsed").SetActive(false);

        StartCoroutine(SetActive(finishMenu, true, 0.5f, "CanvasGroup"));
        StartCoroutine(SetActive(gameUI, false, 0.5f, "CanvasGroup"));
        
        yield return new WaitForSeconds(0.5f);
        
        GameObject.Find("Used").GetComponent<TextMeshProUGUI>().text = $"{Uses}/{perfectUseCount}";
        Slider slider = GameObject.Find("Used Slider").GetComponent<Slider>();
        slider.maxValue = 1;

        StartCoroutine(sliderAnim(slider));
    }

    IEnumerator sliderAnim(Slider slider)
    {
        float stepSize = 0.02f;
        for (float t = 0; Mathf.Floor(t * 100) / 100 <= 1; t += stepSize)
        {
            slider.value = Mathf.Lerp(0, perfectUseCount / (float)Uses, SmoothFunc(t));

            yield return new WaitForSeconds(finishSliderTime / (100 / stepSize));
        }

        slider.value = perfectUseCount / (float)Uses;
    }

    float SmoothFunc(float t)
    {
        return 1 / (1 + Mathf.Pow(2.7182818284f, -10 * (t - 0.5f)));
    }

    public IEnumerator SetActive(GameObject g, bool value, float fadeTime, string T, bool setActive=true)
    {
        if (value && setActive)
            g.SetActive(true);
  
        switch(T)
        {
            case "Image":
                Image image = g.GetComponent<Image>();
                Color c = image.color;
                c.a = value ? 0 : 1;
        
                Color ct = image.color;
                ct.a = value ? 1 : 0;
        
                for (float t = 0; Mathf.Floor(t * 100) / 100 <= 1; t += 0.1f)
                {
                    image.color = Color.Lerp(c, ct, value ? t : 1 - t);
        
                    yield return new WaitForSeconds(fadeTime / 10);
                }
                break;
        
            case "SpriteRenderer":
                SpriteRenderer sprite = g.GetComponent<SpriteRenderer>();
                c = sprite.color;
                c.a = value ? 0 : 1;
        
                ct = sprite.color;
                ct.a = value ? 1 : 0;
        
                for (float t = 0; Mathf.Floor(t * 100) / 100 <= 1; t += 0.1f)
                {
                    sprite.color = Color.Lerp(c, ct, value ? t : 1 - t);
        
                    yield return new WaitForSeconds(fadeTime / 10);
                }
                break;
        
            case "CanvasGroup":
                CanvasGroup group = g.GetComponent<CanvasGroup>();
                
                if (!value && !setActive && group.alpha == 0)
                    break;

                for (float t = 0; Mathf.Floor(t * 100) / 100 <= 1; t += 0.1f)
                {
                    group.alpha = value ? t : 1 - t;
        
                    yield return new WaitForSeconds(fadeTime / 10);
                }
                break;
        }
        if (!value && setActive)
            g.SetActive(false);
    }
}