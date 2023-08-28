using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicant : MonoBehaviour
{
    [SerializeField] bool Gravity = true;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Touch"))
        {
            GetComponent<HingeJoint2D>().enabled = true;
            GetComponent<HingeJoint2D>().connectedBody = GameObject.FindGameObjectWithTag("Touch").GetComponent<Rigidbody2D>();
            GetComponent<Rigidbody2D>().gravityScale = 0;
        }

        if (collider.CompareTag("Poop Trap"))
        {
            GetComponent<AudioSource>().volume = GameObject.FindWithTag("settingsMenu").GetComponent<settingsMenu>().SFXVolume;
            GetComponent<AudioSource>().Play();
            GetComponent<SpriteRenderer>().color = new Color(69/256f, 40 / 256f, 60 / 256f);
            Destroy(gameObject, 2);
        }

        if (collider.CompareTag("Cabbage"))
            Destroy(gameObject);

        if (collider.CompareTag("Wipe"))
            Destroy(collider.gameObject);
    }

    void Update()
    {
        if (Input.touchCount == 0 && !Input.GetMouseButton(0))
        {
            GetComponent<HingeJoint2D>().enabled = false;
            GetComponent<Rigidbody2D>().gravityScale = Gravity ? 2 : 0;
        }
    }
}