using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicant : MonoBehaviour
{
    [SerializeField] bool Gravity = true;
    [HideInInspector] public float g = 2;
    [HideInInspector] public bool held = false;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Touch"))
        {
            held = true;
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

        if (collider.CompareTag("Cabbage") || collider.CompareTag("Bullet") || collider.CompareTag("Cat"))
            Destroy(gameObject);

        if (collider.CompareTag("Wipe"))
            Destroy(collider.gameObject);
    }

    void Update()
    {
        if (Input.touchCount == 0 && !Input.GetMouseButton(0))
        {
            held = false;
            GetComponent<HingeJoint2D>().enabled = false;
            GetComponent<Rigidbody2D>().gravityScale = Gravity ? g : 0;
        }

        Transform Touch = GameObject.FindWithTag("Touch").transform;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(Touch.position, 0.25f);

        bool inNonMovableArea = false;

        foreach (Collider2D collider in colliders)
            if (collider.CompareTag("NonMovableArea") && collider.name[collider.name.Length - 1] == '1')
                inNonMovableArea = true;

        if (inNonMovableArea)
        {
            held = false;
            GetComponent<HingeJoint2D>().enabled = false;
            GetComponent<Rigidbody2D>().gravityScale = Gravity ? g : 0;
        }
    }
}