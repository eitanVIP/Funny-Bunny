using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class LevelSelectorManager : MonoBehaviour
{
    [SerializeField] SceneTransitioner sceneTransitioner;
    [SerializeField] Transform Touch;
    [SerializeField] GameObject Canvas;
    [SerializeField] GameObject lvlPrefab;
    [SerializeField] Vector2 selfDist;
    [SerializeField] Vector2 screenSize;
    [SerializeField] Vector2 Offset;
    [SerializeField] float Rate;
    [SerializeField] Slider slider;
    [SerializeField] float swipeSensitivity;
    Vector2 fingerUpPosition;
    Vector2 fingerDownPosition;
    bool Swiping = false;

    void Start()
    {
        StartCoroutine(generateButtons());
    }

    void Update()
    {
        if (Input.touchCount > 0)
            Touch.position = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        if (Input.GetMouseButton(0))
            Touch.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        touchDetection();
    }

    void touchDetection()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerDownPosition = touch.position;
                fingerUpPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;

                if (Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x) >= swipeSensitivity && !Swiping)
                    Swipe();
            }
        }
    }

    void Swipe()
    {
        Swiping = true;
        float dir = Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x) / (fingerDownPosition.x - fingerUpPosition.x) > 0 ? 1 : -1;
        StartCoroutine(ChangePage((int)dir));
        Swiping = false;
    }

    void generateButton(int maxX, int maxY, int i)
    {
        GameObject g = Instantiate(lvlPrefab, Canvas.transform.GetChild(0));
        RectTransform transform = g.GetComponent<RectTransform>();

        transform.anchoredPosition = calculatePosition(maxX, maxY, i, transform);

        setButtonText(i, transform);

        setOnClick(g.GetComponent<Button>(), i + 2);

        g.GetComponent<Button>().interactable = PlayerPrefs.GetInt("Level") >= i + 1;

        StartCoroutine(animateGeneration(g, i));
    }

    void setButtonText(int i, RectTransform transform)
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();

        TextMeshProUGUI btnScore = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        float uses = -1;
        if (PlayerPrefs.GetInt($"Uses{i + 1}") < 2147483640)
            uses = PlayerPrefs.GetInt($"Uses{i + 1}");

        float perfectUses = PlayerPrefs.GetInt($"PerfectUses{i + 1}");

        btnScore.text = "";
        if (uses != -1 && perfectUses != 0)
            btnScore.text = $"{uses}/{perfectUses}";
    }

    Vector2 calculatePosition(int maxX, int maxY, int i, RectTransform transform)
    {
        float x = Offset.x + (selfDist.x + transform.sizeDelta.x) * (i % maxX) + Mathf.FloorToInt(Mathf.FloorToInt(i / maxX) / maxY) * screenSize.x;
        float y = Offset.y + Mathf.FloorToInt(i / maxX) % maxY * -(selfDist.y + transform.sizeDelta.y);
        return new Vector2(x, y);
    }

    IEnumerator animateGeneration(GameObject g, int i)
    {
        Color c = g.GetComponent<Image>().color;
        c.a = 0;
        Color ct = g.GetComponent<Image>().color;
        ct.a = PlayerPrefs.GetInt("Level") >= i + 1 ? 1 : 0.5f;
        for (float t = 0; Mathf.Floor(t * 100) / 100 <= 1; t += 0.2f)
        {
            transform.localScale = Vector2.one * t;
            g.GetComponent<Image>().color = Color.Lerp(c, ct, t);

            yield return new WaitForSeconds(Rate / 10);
        }
    }

    IEnumerator generateButtons()
    {
        yield return new WaitForSeconds(0.5f);

        Score();

        RectTransform transform = lvlPrefab.GetComponent<RectTransform>();
        int maxX = Mathf.FloorToInt(screenSize.x / (selfDist.x + transform.sizeDelta.x));
        int maxY = Mathf.FloorToInt((screenSize.y - Offset.y) / (selfDist.y + transform.sizeDelta.y));
        Offset = new Vector2(350 + Offset.x + -screenSize.x/2 + (screenSize.x - maxX * (selfDist.x + transform.sizeDelta.x)) / 2 + selfDist.x / 2, Offset.y - 300);

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings - 2; i++)
        {
            generateButton(maxX, maxY, i);
            yield return new WaitForSeconds(Rate);
        }
    }

    void Score()
    {
        float score = 0;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings - 2; i++)
        {
            float uses = -1;
            if (PlayerPrefs.GetInt($"Uses{i + 1}") < 2147483640)
                uses = PlayerPrefs.GetInt($"Uses{i + 1}");

            float perfectUses = PlayerPrefs.GetInt($"PerfectUses{i + 1}");

            if (uses != -1 && perfectUses != 0)
                score += perfectUses / uses;

        }
        float perfect = SceneManager.sceneCountInBuildSettings - 2;
        if (perfect > 0)
        {
            StartCoroutine(sliderAnim(slider, score, perfect, 1));
            slider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"{Mathf.Floor(score / perfect * 100)}%";
        }
    }

    IEnumerator sliderAnim(Slider slider, float value, float max, float time, float stepSize=0.02f)
    {
        for (float t = 0; Mathf.Floor(t * 100) / 100 <= 1; t += stepSize)
        {
            slider.value = Mathf.Lerp(0, value / max, SmoothFunc(t));

            yield return new WaitForSeconds(time / (100 / stepSize));
        }

        slider.value = value / max;
    }

    float SmoothFunc(float t)
    {
        return 1 / (1 + Mathf.Pow(2.7182818284f, -10 * (t - 0.5f)));
    }

    void setOnClick(Button b, int lvl)
    {
        b.onClick.AddListener(delegate { play(lvl); } );
    }

    public void play(int lvl)
    {
        StartCoroutine(sceneTransitioner.loadScene(index: lvl, canvas: Canvas.GetComponent<Canvas>()));
    }

    public void back()
    {   
        StartCoroutine(sceneTransitioner.loadScene(index: 0, canvas: Canvas.GetComponent<Canvas>()));
    }

    public void Arrow(int dir)
    {
        StartCoroutine(ChangePage(-dir));
    }

    IEnumerator ChangePage(int dir)
    {
        RectTransform Buttons = Canvas.transform.GetChild(0).GetComponent<RectTransform>();
        float start = Buttons.anchoredPosition.x;
        float end = start + (dir > 0 ? screenSize.x : -screenSize.x);
        float time = 0.25f;
        float stepSize = 0.04f;

        end = Mathf.Clamp(end, Mathf.Floor((SceneManager.sceneCountInBuildSettings - 3) / 10) * -screenSize.x, 0);

        for (float t = 0; Mathf.Floor(t * 100) / 100 <= 1; t += stepSize)
        {
            Buttons.anchoredPosition = Vector2.right * Mathf.Lerp(start, end, SmoothFunc(t));

            yield return new WaitForSeconds(time / (100 / stepSize));
        }

        Buttons.anchoredPosition = Vector2.right * end;
    }
}