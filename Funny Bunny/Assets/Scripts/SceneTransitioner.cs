using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTransitioner : MonoBehaviour
{
    [SerializeField] float animationLength;

    Animator finishAnimation;
    SpriteRenderer child1;
    SpriteRenderer child2;
    settingsMenu settings;

    [HideInInspector] public bool isTransitioning = false;

    void Start()
    {
        gatherObjects();
    }

    void gatherObjects()
    {
        finishAnimation = transform.GetComponentInChildren<Animator>();
        child1 = finishAnimation.transform.GetChild(0).GetComponent<SpriteRenderer>();
        child2 = finishAnimation.transform.GetChild(1).GetComponent<SpriteRenderer>();
        if (GameObject.FindWithTag("settingsMenu"))
            settings = GameObject.FindWithTag("settingsMenu").GetComponent<settingsMenu>();
    }

    public IEnumerator loadScene(int index=-1, string name="", Canvas canvas=null)
    {
        isTransitioning = true;

        if(settings)
            GetComponent<AudioSource>().volume = settings.SFXVolume;

        if (canvas)
            canvas.GetComponent<Canvas>().enabled = false;

        child1.enabled = true;
        child2.enabled = true;
        finishAnimation.SetTrigger("Finish");
        GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(0.5f);

        DontDestroyOnLoad(gameObject);

        if (index > -1)
            SceneManager.LoadScene(index);
        if (name != "")
            SceneManager.LoadScene(name);

        finishAnimation.SetTrigger("Start");

        Destroy(gameObject, 0.5f);
    }
}