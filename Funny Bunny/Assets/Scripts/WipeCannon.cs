using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WipeCannon : MonoBehaviour
{
    [SerializeField] GameObject wipePrefab;
    [SerializeField] float speed;
    float time = 2;

    IEnumerator Start()
    {
        GetComponent<Animator>().speed = 1 / time * 0.25f;
        yield return new WaitForSeconds(time);
        time = Random.Range(0.5f, 3f);

        GetComponent<AudioSource>().volume = GameObject.FindWithTag("settingsMenu").GetComponent<settingsMenu>().SFXVolume;
        GetComponent<AudioSource>().Play();
        GameObject newWipe = Instantiate(wipePrefab, transform.position, Quaternion.identity);
        newWipe.GetComponent<Rigidbody2D>().velocity = -transform.right * speed;
        Destroy(newWipe, 10);

        StartCoroutine(Start());
    }
}