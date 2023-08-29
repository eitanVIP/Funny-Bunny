using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBlock : MonoBehaviour
{
    public float Gravity;
    [SerializeField] float scaleFactor;

    void Start()
    {
        transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, Gravity > 0 ? -90 : 90);
        transform.GetChild(0).localScale = Vector3.Scale(transform.GetChild(0).localScale, Vector3.one * Mathf.Pow(Mathf.Abs(Gravity), scaleFactor));
    }

    void Update()
    {
        Collider2D[] colls = Physics2D.OverlapAreaAll(transform.GetChild(1).position, transform.GetChild(2).position);
        if(colls.Length == 0)
            return;

        foreach(Collider2D coll in colls)
        {
            if (!coll.GetComponent<Rigidbody2D>())
                continue;

            coll.transform.Rotate(0, 0, coll.GetComponent<Rigidbody2D>().gravityScale * coll.GetComponent<Rigidbody2D>().gravityScale * Gravity < 0 ? 180 : 0);

            if (!coll.CompareTag("Clone"))
                coll.GetComponent<Rigidbody2D>().gravityScale *= Gravity;
            else
                coll.GetComponent<Duplicant>().g *= Gravity;

            if (coll.CompareTag("Player"))
                Destroy(gameObject);
        }
    }
}