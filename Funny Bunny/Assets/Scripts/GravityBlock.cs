using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBlock : MonoBehaviour
{
    public float Gravity;
    [SerializeField] float scaleFactor;
    [SerializeField] bool affectsPlayer;
    [SerializeField] bool affectsCats;
    List<GameObject> Recently = new List<GameObject>();
    List<float> recentlyTimer = new List<float>();

    void Start()
    {
        transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, Gravity < 0 ? 90 : -90);
        transform.GetChild(0).localScale = Vector3.Scale(transform.GetChild(0).localScale, Vector3.one * Mathf.Clamp(Mathf.Pow(Mathf.Abs(Gravity), scaleFactor), 0.375f, 100f));
    }

    void Update()
    {
        Collider2D[] colls = Physics2D.OverlapAreaAll(transform.GetChild(1).position, transform.GetChild(2).position);
        if(colls.Length == 0)
            return;

        foreach(Collider2D coll in colls)
        {
            if (!coll.GetComponent<Rigidbody2D>() || Recently.Contains(coll.gameObject))
                continue;

            if(!(coll.CompareTag("Player") && !affectsPlayer) && !(coll.CompareTag("Cat") && affectsCats))
                coll.transform.Rotate(0, 0, coll.GetComponent<Rigidbody2D>().gravityScale * coll.GetComponent<Rigidbody2D>().gravityScale * Gravity < 0 ? 180 : 0);

            if (!coll.CompareTag("Clone"))
            {
                if (!coll.CompareTag("Player") && !coll.CompareTag("Cat"))
                    coll.GetComponent<Rigidbody2D>().gravityScale *= Gravity;
            } 
            else
                coll.GetComponent<Duplicant>().g *= Gravity;

            if (coll.CompareTag("Player") && affectsPlayer)
            {
                coll.GetComponent<Rigidbody2D>().gravityScale *= Gravity;
                Destroy(gameObject);
            }

            if (coll.CompareTag("Cat") && affectsCats)
                coll.GetComponent<Rigidbody2D>().gravityScale *= Gravity;

            Recently.Add(coll.gameObject);
            recentlyTimer.Add(1);
        }

        for(int i = 0; i < Recently.Count; i++)
        {
            recentlyTimer[i] -= Time.deltaTime;

            if (recentlyTimer[i] <= 0)
                Recently[i] = null;
        }

        Recently.RemoveAll(x => x == null);
        recentlyTimer.RemoveAll(x => x <= 0);
    }
}